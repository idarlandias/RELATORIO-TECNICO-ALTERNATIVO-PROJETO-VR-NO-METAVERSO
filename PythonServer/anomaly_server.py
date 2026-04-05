"""
SentinelVR — Servidor de Deteccao de Anomalias
===============================================
Servidor WebSocket assíncrono que recebe frames das cameras de vigilancia
do Unity via protocolo binario (4 bytes indice + PNG), processa com
ResNet50 (feature extraction) + Autoencoder (deteccao de anomalias) e
retorna JSON com score e flag de anomalia.

Protocolo binario (Unity -> Python):
    bytes[0:4]  = camera index (int32 little-endian)
    bytes[4:]   = frame PNG

Protocolo JSON (Python -> Unity):
    {"camera": 0, "score": 0.087, "is_anomaly": true}

Uso:
    python anomaly_server.py
    python anomaly_server.py --host 0.0.0.0 --port 8765 --model sentinel_autoencoder.pth

Requer:
    pip install -r requirements.txt
"""

import asyncio
import websockets
import json
import struct
import logging
import numpy as np
from io import BytesIO
from PIL import Image

import torch
import torch.nn as nn
import torchvision.transforms as transforms
import torchvision.models as models

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s"
)
logger = logging.getLogger("SentinelVR.Server")

HOST             = "localhost"
PORT             = 8765
ANOMALY_THRESHOLD = 0.045  # MSE acima desse valor = anomalia detectada
MODEL_PATH       = "sentinel_autoencoder.pth"


# ─────────────────────────────────────────────────────────────────────────────
# Arquitetura: Autoencoder linear sobre features ResNet50
# Input: vetor de features 2048d extraido pelo ResNet50
# ─────────────────────────────────────────────────────────────────────────────

class AnomalyAutoencoder(nn.Module):
    """
    Autoencoder linear treinado sobre features extraidas pelo ResNet50.
    MSE de reconstrucao alto = frame anomalo.
    """

    def __init__(self, input_dim: int = 2048, latent_dim: int = 64):
        super().__init__()
        self.encoder = nn.Sequential(
            nn.Linear(input_dim, 512), nn.ReLU(),
            nn.Dropout(0.2),
            nn.Linear(512, 128),       nn.ReLU(),
            nn.Linear(128, latent_dim), nn.ReLU(),
        )
        self.decoder = nn.Sequential(
            nn.Linear(latent_dim, 128), nn.ReLU(),
            nn.Linear(128, 512),        nn.ReLU(),
            nn.Dropout(0.2),
            nn.Linear(512, input_dim),  nn.Sigmoid(),
        )

    def forward(self, x):
        return self.decoder(self.encoder(x))

    def reconstruction_error(self, x: torch.Tensor) -> float:
        with torch.no_grad():
            return nn.functional.mse_loss(self.forward(x), x).item()


# ─────────────────────────────────────────────────────────────────────────────
# Inicializacao dos modelos
# ─────────────────────────────────────────────────────────────────────────────

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
logger.info(f"Dispositivo: {device}")

# Feature extractor: ResNet50 sem a camada de classificacao final
_resnet = models.resnet50(weights=models.ResNet50_Weights.DEFAULT)
feature_extractor = nn.Sequential(*list(_resnet.children())[:-1]).eval().to(device)

# Autoencoder de deteccao de anomalias
autoencoder = AnomalyAutoencoder().to(device)
try:
    autoencoder.load_state_dict(torch.load(MODEL_PATH, map_location=device))
    logger.info(f"Autoencoder carregado: {MODEL_PATH}")
except FileNotFoundError:
    logger.warning(f"Modelo '{MODEL_PATH}' nao encontrado. Usando pesos aleatorios.")
autoencoder.eval()

# Pre-processamento de imagem para ResNet50 (ImageNet normalization)
preprocess = transforms.Compose([
    transforms.Resize((224, 224)),
    transforms.ToTensor(),
    transforms.Normalize(mean=[0.485, 0.456, 0.406],
                         std=[0.229, 0.224, 0.225]),
])


# ─────────────────────────────────────────────────────────────────────────────
# Processamento de frame
# ─────────────────────────────────────────────────────────────────────────────

def extract_features(image: Image.Image) -> torch.Tensor:
    """Extrai vetor de features 2048d usando ResNet50."""
    tensor = preprocess(image).unsqueeze(0).to(device)
    with torch.no_grad():
        features = feature_extractor(tensor)
    return features.view(1, -1)  # [1, 2048]


def compute_anomaly_score(features: torch.Tensor) -> float:
    """Calcula MSE de reconstrucao do Autoencoder. Score > ANOMALY_THRESHOLD = anomalia."""
    return autoencoder.reconstruction_error(features)


# ─────────────────────────────────────────────────────────────────────────────
# Handler WebSocket
# ─────────────────────────────────────────────────────────────────────────────

async def handle_frame(websocket):
    """
    Processa mensagens binarias do Unity.
    Protocolo: int32(camera_index) + bytes(PNG frame)
    """
    client = websocket.remote_address
    logger.info(f"Unity conectado: {client}")

    try:
        async for message in websocket:
            if not isinstance(message, bytes) or len(message) <= 4:
                continue

            # Decodificar protocolo binario
            camera_index = struct.unpack("<i", message[:4])[0]
            frame_bytes  = message[4:]

            try:
                image    = Image.open(BytesIO(frame_bytes)).convert("RGB")
                features = extract_features(image)
                score    = compute_anomaly_score(features)
                is_anom  = score > ANOMALY_THRESHOLD

                response = {
                    "camera":     camera_index,
                    "score":      round(score, 4),
                    "is_anomaly": is_anom,
                }
                await websocket.send(json.dumps(response))

                if is_anom:
                    logger.warning(
                        f"ANOMALIA | CAM {camera_index + 1} | Score: {score:.4f} "
                        f"(threshold: {ANOMALY_THRESHOLD})"
                    )

            except Exception as e:
                logger.error(f"Erro ao processar frame CAM {camera_index}: {e}")
                await websocket.send(json.dumps({
                    "camera": camera_index, "score": 0.0, "is_anomaly": False
                }))

    except websockets.exceptions.ConnectionClosed:
        logger.info(f"Unity desconectado: {client}")


# ─────────────────────────────────────────────────────────────────────────────
# Entrypoint
# ─────────────────────────────────────────────────────────────────────────────

async def main():
    logger.info(f"SentinelVR Anomaly Server iniciando em ws://{HOST}:{PORT}")
    logger.info(f"Threshold de anomalia: {ANOMALY_THRESHOLD}")
    async with websockets.serve(handle_frame, HOST, PORT, max_size=10 * 1024 * 1024):
        logger.info("Aguardando conexao do Unity...")
        await asyncio.Future()


if __name__ == "__main__":
    asyncio.run(main())
