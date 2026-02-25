# SentinelVR — Servidor Python de Detecção de Anomalias

Servidor WebSocket assíncrono que processa frames de câmeras de vigilância com um autoencoder PyTorch para detectar anomalias em tempo real.

## Requisitos

- Python 3.10+
- CUDA (opcional, para GPU acceleration)

## Instalação

```bash
cd PythonServer
python -m venv venv
# Windows:
venv\Scripts\activate
# Linux/Mac:
source venv/bin/activate

pip install -r requirements.txt
```

## Execução do Servidor

```bash
# Sem modelo treinado (scores aleatórios, apenas para testes de conexão):
python anomaly_server.py

# Com modelo treinado:
python anomaly_server.py --model sentinel_model.pth

# Com host e porta customizados:
python anomaly_server.py --host 0.0.0.0 --port 8765 --model sentinel_model.pth
```

O servidor escuta em `ws://localhost:8765` por padrão.

## Protocolo WebSocket

### Mensagem de Entrada (Unity → Servidor)
```json
{
  "frame": "<base64 JPEG>",
  "threshold": 0.7
}
```

### Mensagem de Saída (Servidor → Unity)
```json
{
  "anomaly_score": 0.85,
  "location_x": 0.42,
  "location_y": 0.67,
  "message": "anomalia detectada",
  "threshold": 0.7
}
```

- `anomaly_score`: valor entre 0 (normal) e 1 (anomalia severa)
- `location_x`, `location_y`: coordenadas normalizadas (0-1) da anomalia no frame
- `message`: `"anomalia detectada"` se score >= threshold, `"normal"` caso contrário

## Treinamento do Modelo

### Preparar dados

Colete frames de vídeo normais (sem anomalias) das câmeras e salve como JPEGs:

```
data/
  normal_frames/
    frame_0001.jpg
    frame_0002.jpg
    ...
```

### Treinar

```bash
python train_autoencoder.py \
  --data_dir ./data/normal_frames \
  --epochs 50 \
  --batch_size 32 \
  --output sentinel_model.pth
```

### Validar limiar

Após treinar, teste o limiar com frames anômalos rotulados:
- Abaixo de 0.5: muito sensível (muitos falsos positivos)
- Entre 0.6 e 0.8: recomendado para ambientes controlados
- Acima de 0.8: menos sensível (pode perder anomalias sutis)

## Configuração no Unity

No componente `AnomalyDetector` do Unity:
- **Server URL**: `ws://localhost:8765` (ou IP da máquina com o servidor)
- **Anomaly Threshold**: usar o mesmo limiar configurado no servidor
- **Capture Interval**: intervalo entre frames (0.5s recomendado)

## Estrutura de Arquivos

```
PythonServer/
├── anomaly_server.py       # Servidor WebSocket principal
├── train_autoencoder.py    # Script de treinamento
├── requirements.txt        # Dependências Python
├── README_server.md        # Este arquivo
└── sentinel_model.pth      # Modelo treinado (gerado após treino)
```
