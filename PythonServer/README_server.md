# SentinelVR — Servidor Python de Detecção de Anomalias

## Visão Geral

Servidor WebSocket assíncrono que recebe frames das câmeras de vigilância do Unity, processa com **ResNet50 + Autoencoder** e retorna score de anomalia em tempo real.

### Protocolo

| Direção | Formato |
|---|---|
| Unity → Python | `int32(camera_index)` + `bytes(PNG frame)` |
| Python → Unity | `{"camera": 0, "score": 0.087, "is_anomaly": true}` |

## Setup

```bash
cd PythonServer/

# Criar ambiente virtual
python -m venv venv
venv\Scripts\activate        # Windows
source venv/bin/activate     # Linux/Mac

# Instalar dependências
pip install -r requirements.txt
```

## Executar o Servidor

```bash
# Modo padrão (ws://localhost:8765)
python anomaly_server.py

# Personalizado
python anomaly_server.py --host 0.0.0.0 --port 8765 --model sentinel_autoencoder.pth
```

## Treinar o Autoencoder

```bash
# 1. Capture frames normais via Unity e salve em data/normal_frames/
# 2. Execute o treinamento
python train_autoencoder.py --data_dir ./data/normal_frames --epochs 50

# O modelo é salvo como sentinel_autoencoder.pth
# O script exibe o threshold recomendado ao final do treino
```

## Arquitetura do Modelo

```
Frame PNG (512x512)
     |
ResNet50 (ImageNet, frozen)
     |
Feature Vector [2048d]
     |
Autoencoder:
  Encoder: 2048 → 512 → 128 → 64
  Decoder: 64 → 128 → 512 → 2048
     |
MSE Score > 0.045 → ANOMALIA
```

## Dependências

| Pacote | Versão | Uso |
|---|---|---|
| `websockets` | ≥12.0 | Servidor WebSocket assíncrono |
| `torch` | ≥2.0.0 | Autoencoder e inferência |
| `torchvision` | ≥0.15.0 | ResNet50 + transforms |
| `Pillow` | ≥10.0.0 | Decodificação de PNG |
| `numpy` | ≥1.24.0 | Utilitários numéricos |
