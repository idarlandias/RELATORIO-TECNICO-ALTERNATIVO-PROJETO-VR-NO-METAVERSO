"""
SentinelVR — Treinamento do Autoencoder de Deteccao de Anomalias
=================================================================
Treina o AnomalyAutoencoder com frames normais capturados das cameras
de vigilancia. O modelo aprendido e salvo como sentinel_autoencoder.pth.

Fluxo:
    1. Capturar frames normais (sem anomalias) e salvar em data/normal_frames/
    2. Executar este script
    3. O modelo salvo e usado pelo anomaly_server.py

Uso:
    python train_autoencoder.py
    python train_autoencoder.py --data_dir ./data/normal_frames --epochs 100

Estrutura esperada de dados:
    data/normal_frames/
        cam01_frame_0001.png
        cam01_frame_0002.png
        cam02_frame_0001.png
        ...
"""

import os
import argparse
import logging
import torch
import torch.nn as nn
from torch.utils.data import DataLoader, Dataset
from torchvision import transforms, models
from PIL import Image

from anomaly_server import AnomalyAutoencoder, feature_extractor, preprocess, device

logging.basicConfig(level=logging.INFO, format="%(asctime)s [%(levelname)s] %(message)s")
logger = logging.getLogger("SentinelVR.Trainer")


# ─────────────────────────────────────────────────────────────────────────────
# Dataset
# ─────────────────────────────────────────────────────────────────────────────

class NormalFramesDataset(Dataset):
    """Dataset de frames normais para treinamento do Autoencoder."""

    EXTENSIONS = {".png", ".jpg", ".jpeg"}

    def __init__(self, data_dir: str):
        self.paths = [
            os.path.join(data_dir, f)
            for f in sorted(os.listdir(data_dir))
            if os.path.splitext(f)[1].lower() in self.EXTENSIONS
        ]
        if not self.paths:
            raise ValueError(f"Nenhuma imagem encontrada em: {data_dir}")
        logger.info(f"Dataset: {len(self.paths)} frames em '{data_dir}'")

    def __len__(self):
        return len(self.paths)

    def __getitem__(self, idx: int) -> torch.Tensor:
        image    = Image.open(self.paths[idx]).convert("RGB")
        tensor   = preprocess(image).unsqueeze(0).to(device)
        with torch.no_grad():
            features = feature_extractor(tensor).view(1, -1)
        return features.squeeze(0)  # [2048]


# ─────────────────────────────────────────────────────────────────────────────
# Treinamento
# ─────────────────────────────────────────────────────────────────────────────

def train(data_dir: str, epochs: int, batch_size: int, lr: float, output: str):
    dataset    = NormalFramesDataset(data_dir)
    dataloader = DataLoader(dataset, batch_size=batch_size, shuffle=True, drop_last=False)

    model     = AnomalyAutoencoder().to(device)
    optimizer = torch.optim.Adam(model.parameters(), lr=lr)
    criterion = nn.MSELoss()

    logger.info(f"Iniciando treinamento | Epocas: {epochs} | Batch: {batch_size} | LR: {lr}")

    for epoch in range(1, epochs + 1):
        model.train()
        total_loss = 0.0

        for batch in dataloader:
            batch = batch.to(device)
            optimizer.zero_grad()
            recon = model(batch)
            loss  = criterion(recon, batch)
            loss.backward()
            optimizer.step()
            total_loss += loss.item() * batch.size(0)

        avg_loss = total_loss / len(dataset)

        if epoch % 10 == 0 or epoch == 1:
            logger.info(f"Epoca {epoch:03d}/{epochs} | MSE medio: {avg_loss:.6f}")

    torch.save(model.state_dict(), output)
    logger.info(f"Modelo salvo em: {output}")
    logger.info(f"MSE final: {avg_loss:.6f} | Threshold recomendado: {avg_loss * 3:.4f}")


# ─────────────────────────────────────────────────────────────────────────────
# Entrypoint
# ─────────────────────────────────────────────────────────────────────────────

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="SentinelVR Autoencoder Training")
    parser.add_argument("--data_dir",   default="./data/normal_frames", help="Pasta com frames normais")
    parser.add_argument("--epochs",     type=int,   default=50,    help="Numero de epocas")
    parser.add_argument("--batch_size", type=int,   default=32,    help="Tamanho do batch")
    parser.add_argument("--lr",         type=float, default=1e-3,  help="Learning rate")
    parser.add_argument("--output",     default="sentinel_autoencoder.pth", help="Arquivo de saida")
    args = parser.parse_args()

    train(args.data_dir, args.epochs, args.batch_size, args.lr, args.output)
