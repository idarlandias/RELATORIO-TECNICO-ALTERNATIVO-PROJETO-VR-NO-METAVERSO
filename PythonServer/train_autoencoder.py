"""
SentinelVR — Treinamento do Autoencoder de Detecção de Anomalias
=================================================================
Script para treinar o autoencoder convolucional com frames normais capturados
das câmeras de vigilância.

Uso:
    python train_autoencoder.py --data_dir ./data/normal_frames --epochs 50 --output model.pth

Estrutura esperada de dados:
    data/normal_frames/
        frame_0001.jpg
        frame_0002.jpg
        ...
"""

import argparse
import logging
import os
import torch
import torch.nn as nn
from torch.utils.data import DataLoader, Dataset
from torchvision import transforms
from PIL import Image

# Importar arquitetura do servidor
from anomaly_server import ConvAutoencoder

logging.basicConfig(
    level=logging.INFO, format="%(asctime)s [%(levelname)s] %(message)s"
)
logger = logging.getLogger("SentinelVR.Train")


# ─────────────────────────────────────────────
# Dataset
# ─────────────────────────────────────────────


class SurveillanceFrameDataset(Dataset):
    """
    Dataset de frames normais de câmeras de vigilância para treino do autoencoder.
    """

    SUPPORTED_EXTENSIONS = {".jpg", ".jpeg", ".png", ".bmp"}

    def __init__(self, data_dir: str, image_size: int = 224):
        self.data_dir = data_dir
        self.transform = transforms.Compose(
            [
                transforms.Resize((image_size, image_size)),
                transforms.ToTensor(),
            ]
        )

        self.image_paths = []
        if os.path.exists(data_dir):
            self._load_image_paths()
        else:
            logger.warning(
                f"Diretório de dados não encontrado: {data_dir}. Crie-o e adicione imagens para treinar."
            )

    def _load_image_paths(self):
        """Carrega caminhos de todas as imagens suportadas no diretório."""
        for root, _, files in os.walk(self.data_dir):
            for file in files:
                if any(file.lower().endswith(ext) for ext in self.SUPPORTED_EXTENSIONS):
                    self.image_paths.append(os.path.join(root, file))

        logger.info(f"Encontradas {len(self.image_paths)} imagens em {self.data_dir}")

    def __len__(self):
        return len(self.image_paths)

    def __getitem__(self, idx):
        img_path = self.image_paths[idx]
        try:
            image = Image.open(img_path).convert("RGB")
            if self.transform:
                image = self.transform(image)
            return image
        except Exception as e:
            logger.error(f"Erro ao carregar imagem {img_path}: {e}")
            # Em caso de erro, retorna um tensor vazio ou a próxima imagem
            return torch.zeros(3, 224, 224)


# ─────────────────────────────────────────────
# Treinamento
# ─────────────────────────────────────────────


def train(
    model: ConvAutoencoder,
    dataloader: DataLoader,
    epochs: int,
    learning_rate: float,
    device: str,
    output_path: str,
):
    """
    Treina o autoencoder usando reconstrução MSE como função de perda.
    Salva checkpoint ao final de cada época e o modelo final em output_path.
    """
    optimizer = torch.optim.Adam(model.parameters(), lr=learning_rate)
    criterion = nn.MSELoss()

    model.train()

    if len(dataloader) == 0:
        logger.error("Dataloader vazio. Não há imagens para treinar.")
        return

    for epoch in range(epochs):
        total_loss = 0.0
        num_batches = 0

        for batch in dataloader:
            # 1. Mover batch para device
            batch = batch.to(device)

            # 2. Forward pass: output = model(batch)
            output = model(batch)

            # 3. Calcular loss = criterion(output, batch)
            loss = criterion(output, batch)

            # 4. Backward pass e optimizer.step()
            optimizer.zero_grad()
            loss.backward()
            optimizer.step()

            # 5. Acumular total_loss
            total_loss += loss.item()
            num_batches += 1

        avg_loss = total_loss / max(num_batches, 1)
        logger.info(f"Época [{epoch+1}/{epochs}] Loss: {avg_loss:.6f}")

        # Salvar checkpoint a cada 10 épocas
        if (epoch + 1) % 10 == 0:
            checkpoint_path = f"{output_path.split('.pth')[0]}_epoch_{epoch+1}.pth"
            torch.save(model.state_dict(), checkpoint_path)
            logger.info(f"Checkpoint salvo: {checkpoint_path}")

    # Salvar modelo final em output_path
    torch.save(model.state_dict(), output_path)
    logger.info(f"Treinamento concluído. Modelo salvo em: {output_path}")


# ─────────────────────────────────────────────
# Avaliação
# ─────────────────────────────────────────────


def evaluate_threshold(
    model: ConvAutoencoder, normal_dir: str, anomaly_dir: str, device: str
):
    """
    Avalia o limiar ideal de detecção comparando frames normais e anômalos.
    Calcula métricas: precisão, recall, F1-score para diferentes thresholds.
    """
    # TODO: Implementar avaliação com exemplos de anomalias rotuladas
    pass


# ─────────────────────────────────────────────
# Main
# ─────────────────────────────────────────────


def main():
    parser = argparse.ArgumentParser(description="Treinar autoencoder SentinelVR")
    parser.add_argument(
        "--data_dir", required=True, help="Diretório com frames normais de treinamento"
    )
    parser.add_argument(
        "--epochs", type=int, default=50, help="Número de épocas (default: 50)"
    )
    parser.add_argument(
        "--batch_size", type=int, default=32, help="Tamanho do batch (default: 32)"
    )
    parser.add_argument(
        "--lr", type=float, default=1e-3, help="Learning rate (default: 0.001)"
    )
    parser.add_argument(
        "--output", default="sentinel_model.pth", help="Caminho do modelo salvo"
    )
    parser.add_argument(
        "--image_size", type=int, default=224, help="Tamanho das imagens (default: 224)"
    )
    args = parser.parse_args()

    device = "cuda" if torch.cuda.is_available() else "cpu"
    logger.info(f"Dispositivo: {device}")

    # 1. Dataset e Dataloader
    logger.info(f"Carregando dataset de {args.data_dir}...")
    dataset = SurveillanceFrameDataset(args.data_dir, image_size=args.image_size)

    if len(dataset) == 0:
        logger.error("Nenhuma imagem encontrada. Abortando treinamento.")
        return

    dataloader = DataLoader(
        dataset,
        batch_size=args.batch_size,
        shuffle=True,
        num_workers=2,
        pin_memory=True if device == "cuda" else False,
    )

    # 2. Instanciar modelo
    logger.info("Inicializando modelo Autoencoder Convolucional...")
    model = ConvAutoencoder().to(device)

    # 3. Treinar
    logger.info(
        f"Iniciando treinamento ({args.epochs} épocas, batch size {args.batch_size})..."
    )
    train(
        model=model,
        dataloader=dataloader,
        epochs=args.epochs,
        learning_rate=args.lr,
        device=device,
        output_path=args.output,
    )


if __name__ == "__main__":
    main()
