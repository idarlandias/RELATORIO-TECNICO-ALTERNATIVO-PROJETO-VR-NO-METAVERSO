"""
SentinelVR — Servidor de Detecção de Anomalias
===============================================
Servidor WebSocket assíncrono que recebe frames da câmera de vigilância via Unity,
processa com um autoencoder treinado e retorna score de anomalia.

Uso:
    python anomaly_server.py [--host HOST] [--port PORT] [--model MODEL_PATH]

Requer:
    pip install -r requirements.txt
"""

import asyncio
import websockets
import json
import base64
import argparse
import logging
import numpy as np
from io import BytesIO
from PIL import Image
import torch
import torch.nn as nn

# Configuração de logging
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s"
)
logger = logging.getLogger("SentinelVR.Server")

# ─────────────────────────────────────────────
# Modelo: Autoencoder Convolucional
# ─────────────────────────────────────────────

class ConvAutoencoder(nn.Module):
    """
    Autoencoder convolucional para detecção de anomalias em frames de câmera.
    Treinado em frames normais; frames anômalos produzem erro de reconstrução alto.
    Input: imagem RGB 224x224 normalizada
    """

    def __init__(self):
        super(ConvAutoencoder, self).__init__()

        # Encoder
        self.encoder = nn.Sequential(
            nn.Conv2d(3, 32, kernel_size=3, stride=2, padding=1),   # 112x112
            nn.ReLU(),
            nn.Conv2d(32, 64, kernel_size=3, stride=2, padding=1),  # 56x56
            nn.ReLU(),
            nn.Conv2d(64, 128, kernel_size=3, stride=2, padding=1), # 28x28
            nn.ReLU(),
            nn.Conv2d(128, 256, kernel_size=3, stride=2, padding=1), # 14x14
            nn.ReLU(),
        )

        # Decoder
        self.decoder = nn.Sequential(
            nn.ConvTranspose2d(256, 128, kernel_size=3, stride=2, padding=1, output_padding=1), # 28x28
            nn.ReLU(),
            nn.ConvTranspose2d(128, 64, kernel_size=3, stride=2, padding=1, output_padding=1),  # 56x56
            nn.ReLU(),
            nn.ConvTranspose2d(64, 32, kernel_size=3, stride=2, padding=1, output_padding=1),   # 112x112
            nn.ReLU(),
            nn.ConvTranspose2d(32, 3, kernel_size=3, stride=2, padding=1, output_padding=1),    # 224x224
            nn.Sigmoid(),
        )

    def forward(self, x):
        encoded = self.encoder(x)
        decoded = self.decoder(encoded)
        return decoded


# ─────────────────────────────────────────────
# Processamento de frames
# ─────────────────────────────────────────────

def decode_frame(base64_data: str) -> np.ndarray:
    """Decodifica frame base64 JPEG e retorna array numpy RGB normalizado."""
    img_bytes = base64.b64decode(base64_data)
    img = Image.open(BytesIO(img_bytes)).convert("RGB").resize((224, 224))
    return np.array(img, dtype=np.float32) / 255.0


def compute_anomaly_score(model: ConvAutoencoder, frame_array: np.ndarray, device: str) -> float:
    """
    Calcula o score de anomalia como o erro de reconstrução MSE normalizado.
    Score próximo de 0 = frame normal; próximo de 1 = anomalia.
    """
    # Transpor para (C, H, W) e adicionar batch dimension
    tensor = torch.tensor(frame_array).permute(2, 0, 1).unsqueeze(0).to(device)

    with torch.no_grad():
        reconstructed = model(tensor)

    mse = torch.mean((tensor - reconstructed) ** 2).item()
    # Normalizar: MSE típico normal ~0.01, anomalias ~0.1+
    # Clampar para [0, 1]
    normalized_score = min(mse * 10.0, 1.0)
    return normalized_score


def detect_anomaly_location(frame_array: np.ndarray, model: ConvAutoencoder, device: str) -> tuple:
    """
    Detecta a localização aproximada da anomalia no frame usando mapa de saliência.
    Retorna coordenadas (x, y) normalizadas entre 0 e 1.
    """
    # Implementação simplificada: retorna centro do frame
    # TODO: Implementar Grad-CAM ou mapa de erro de reconstrução por região
    return 0.5, 0.5


# ─────────────────────────────────────────────
# Handler WebSocket
# ─────────────────────────────────────────────

async def handle_client(websocket, model: ConvAutoencoder, device: str):
    """Processa mensagens de um cliente Unity conectado via WebSocket."""
    client_addr = websocket.remote_address
    logger.info(f"Cliente conectado: {client_addr}")

    try:
        async for message in websocket:
            try:
                data = json.loads(message)
                frame_b64 = data.get("frame", "")
                threshold = data.get("threshold", 0.7)

                if not frame_b64:
                    await websocket.send(json.dumps({"error": "frame vazio"}))
                    continue

                # Processar frame
                frame_array = decode_frame(frame_b64)
                anomaly_score = compute_anomaly_score(model, frame_array, device)
                loc_x, loc_y = detect_anomaly_location(frame_array, model, device)

                # Montar resposta
                response = {
                    "anomaly_score": float(anomaly_score),
                    "location_x": float(loc_x),
                    "location_y": float(loc_y),
                    "message": "anomalia detectada" if anomaly_score >= threshold else "normal",
                    "threshold": threshold,
                }

                await websocket.send(json.dumps(response))

                if anomaly_score >= threshold:
                    logger.warning(f"ANOMALIA detectada! Score: {anomaly_score:.3f} | Loc: ({loc_x:.2f}, {loc_y:.2f})")

            except json.JSONDecodeError as e:
                logger.error(f"JSON inválido: {e}")
                await websocket.send(json.dumps({"error": f"JSON inválido: {str(e)}"}))
            except Exception as e:
                logger.error(f"Erro ao processar frame: {e}")
                await websocket.send(json.dumps({"error": str(e)}))

    except websockets.exceptions.ConnectionClosed:
        logger.info(f"Cliente desconectado: {client_addr}")


# ─────────────────────────────────────────────
# Inicialização do servidor
# ─────────────────────────────────────────────

def load_model(model_path: str, device: str) -> ConvAutoencoder:
    """Carrega o modelo treinado do arquivo ou inicializa com pesos aleatórios."""
    model = ConvAutoencoder().to(device)

    if model_path:
        try:
            model.load_state_dict(torch.load(model_path, map_location=device))
            logger.info(f"Modelo carregado de: {model_path}")
        except FileNotFoundError:
            logger.warning(f"Modelo não encontrado em '{model_path}'. Usando pesos aleatórios.")
    else:
        logger.warning("Nenhum modelo especificado. Usando pesos aleatórios (scores não confiáveis).")

    model.eval()
    return model


async def main(host: str, port: int, model_path: str):
    device = "cuda" if torch.cuda.is_available() else "cpu"
    logger.info(f"Usando dispositivo: {device}")

    model = load_model(model_path, device)

    logger.info(f"Servidor SentinelVR iniciando em ws://{host}:{port}")
    async with websockets.serve(
        lambda ws: handle_client(ws, model, device),
        host,
        port,
        max_size=10 * 1024 * 1024  # 10 MB max por mensagem
    ):
        logger.info("Servidor pronto. Aguardando conexões do Unity...")
        await asyncio.Future()  # Rodar indefinidamente


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="SentinelVR Anomaly Detection Server")
    parser.add_argument("--host", default="localhost", help="Endereço de bind (default: localhost)")
    parser.add_argument("--port", type=int, default=8765, help="Porta WebSocket (default: 8765)")
    parser.add_argument("--model", default="", help="Caminho para o modelo .pth treinado")
    args = parser.parse_args()

    asyncio.run(main(args.host, args.port, args.model))
