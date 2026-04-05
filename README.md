<h1 align="center">
  🛡️ SentinelVR
</h1>

<p align="center">
  <strong>Central de Monitoramento Inteligente com Detecção de Anomalias por IA</strong><br/>
  <em>Web 3.0 · Residência em TIC 29 · Atividade Avaliativa — Fase 2</em>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6000.0.68f1-000000?style=for-the-badge&logo=unity&logoColor=white"/>
  <img src="https://img.shields.io/badge/Meta%20XR%20SDK-68.x-0064FF?style=for-the-badge&logo=meta&logoColor=white"/>
  <img src="https://img.shields.io/badge/Python-3.10+-3776AB?style=for-the-badge&logo=python&logoColor=white"/>
  <img src="https://img.shields.io/badge/PyTorch-2.x-EE4C2C?style=for-the-badge&logo=pytorch&logoColor=white"/>
  <img src="https://img.shields.io/badge/Platform-Meta%20Quest%202%2F3-1C1C1C?style=for-the-badge&logo=oculus&logoColor=white"/>
</p>

---

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Por que VR?](#-por-que-vr)
- [Stack Técnico](#-stack-técnico)
- [Arquitetura do Sistema](#-arquitetura-do-sistema)
- [Estrutura do Repositório](#-estrutura-do-repositório)
- [Como Executar](#-como-executar)
- [Scripts Principais](#-scripts-principais)
- [Assets e Cena](#-assets-e-cena)
- [Autor](#-autor)

---

## 🔍 Sobre o Projeto

**SentinelVR** é uma central de segurança em **Realidade Virtual** onde a imersão em VR é o núcleo funcional da solução — não apenas sua interface.

Um modelo de **Inteligência Artificial** baseado em **Autoencoder Convolucional** analisa continuamente os frames de 6 câmeras de vigilância virtuais via pipeline Python (WebSocket) e emite alertas visuais e sonoros automaticamente ao detectar comportamentos anômalo.

> ⚠️ **Limitação de Hardware:** Placa de vídeo integrada Intel UHD Graphics sem suporte ao Vulkan API exigido pelo Meta Quest. O projeto foi desenvolvido como planejamento técnico completo e documentado (Relatório Técnico Alternativo — Fase 2).

---

## 🥽 Por que VR?

A VR não é decorativa neste projeto — ela resolve o problema de forma estrutural:

| Propriedade VR | Como Resolve o Problema |
|:---|:---|
| 🌐 **Espacialidade 360°** | Os 6 feeds de câmera envolvem o operador em semicírculo imersivo. Ele gira o corpo naturalmente — sem varredura forçada de múltiplas telas 2D planas |
| 🔊 **Áudio Espacial Direcional** | O alarme soa na direção 3D exata da câmera com anomalia (`OVRSpatialAudioSource`). O operador *ouve de onde vem a ameaça* antes de olhar |
| 🧠 **Presença e Memória Motora** | A sensação de presença física mantém o operador cognitivamente ativo. Dispensar alertas via gesto físico cria memória motora que reforça treinamento real |

---

## 🛠️ Stack Técnico

```
🎮  Unity 6000.0.68f1          Universal Render Pipeline (URP 17.x)
🥽  Meta XR SDK 68.x           OVRCameraRig · Meta Interaction SDK · OVRSpatialAudio
🔌  NativeWebSocket            Ponte de comunicação Unity ↔ Python em tempo real
🐍  Python 3.10+               Servidor assíncrono de IA via asyncio + websockets
🤖  ResNet50 + Autoencoder     Feature extraction + detecção de anomalias (MSE)
🔥  PyTorch 2.x                Inferência em CPU ou GPU (CUDA automático)
📦  Android / Meta Quest       Target: Quest 2 e Quest 3 (ARM64, ASTC, Vulkan)
```

---

## 🏗️ Arquitetura do Sistema

```
┌─────────────────────────────────────────────────────┐
│                  UNITY (Meta XR SDK)                │
│                                                     │
│  OVRCameraRig ──► Sala de Controle VR (12×10m)      │
│       │                                             │
│  6 × Camera ──► RenderTexture ──► Monitor (Canvas)  │
│       │                              │              │
│  CameraCapture.cs                AlertBorder +      │
│       │                         OVRSpatialAudio     │
│       ▼                                             │
│  WebSocket Client (NativeWebSocket)                 │
└──────────────────────┬──────────────────────────────┘
                       │  ws://localhost:8765
                       │  [frame PNG + índice câmera]
┌──────────────────────▼──────────────────────────────┐
│              PYTHON (anomaly_server.py)              │
│                                                     │
│  ResNet50 ──► Feature Vector (2048d)                │
│       │                                             │
│  Autoencoder ──► Reconstrução ──► MSE Score         │
│                                      │              │
│            MSE > 0.045? ──► {"is_anomaly": true}    │
└──────────────────────┬──────────────────────────────┘
                       │  JSON response
                       ▼
            AnomalyAlertController.cs
            ► Borda vermelha piscante
            ► Áudio espacial 3D direcional
            ► AlertPanel_Master atualizado
```

---

## 📁 Estrutura do Repositório

```
SentinelVR-TIC29/
│
├── 📂 Assets/
│   ├── 🎬 Scenes/
│   │   └── SentinelVR_ControlRoom.unity
│   │
│   ├── 💻 Scripts/
│   │   ├── AI/             → AnomalyDetector.cs · AnomalyAlertController.cs
│   │   ├── Monitors/       → MonitorController.cs · DismissAlert.cs
│   │   ├── Surveillance/   → CameraCapture.cs · PatrolMovement.cs · AnomalyTrigger.cs
│   │   ├── UI/             → AIMetricsDisplay.cs · ToggleAISystem.cs
│   │   └── Player/         → PlayerController.cs
│   │
│   ├── 🎨 Materials/       → Dark_Metal_Panel · Scifi_LED_Blue · Glass_Translucent
│   ├── 📷 RenderTextures/  → RT_Camera_01..06.renderTexture
│   ├── 🔊 Audio/           → alarm_beep.wav · server_room_hum.wav
│   └── 🖼️ Textures/        → metal_plate_002 · alert_panel_ui
│
├── 🐍 PythonServer/
│   ├── anomaly_server.py       → Servidor WebSocket principal
│   ├── train_autoencoder.py    → Treinamento do Autoencoder
│   ├── requirements.txt        → Dependências Python
│   └── README_server.md        → Instruções detalhadas do servidor
│
├── ⚙️ ProjectSettings/         → Configurações Unity (XR, URP, Android)
├── 📦 Packages/
│   ├── manifest.json           → Meta XR SDK · NativeWebSocket
│   └── packages-lock.json
│
├── 📄 SentinelVR Relatório Final.pdf   → Relatório técnico completo (9 seções)
└── 📖 README.md
```

---

## 🚀 Como Executar

### Pré-requisitos

- [Unity Hub](https://unity.com/download) com versão **6000.0.68f1**
- **Python 3.10+**
- **Meta XR SDK** (instalado via Package Manager — veja abaixo)

---

### 1️⃣ Servidor Python (IA)

```bash
# Acesse a pasta do servidor
cd PythonServer/

# Crie e ative o ambiente virtual
python -m venv venv
venv\Scripts\activate        # Windows
source venv/bin/activate     # Linux/Mac

# Instale as dependências
pip install -r requirements.txt

# Inicie o servidor
python anomaly_server.py
```

> ✅ O servidor estará disponível em `ws://localhost:8765`

---

### 2️⃣ Projeto Unity

```
1. Abra o Unity Hub → "Add project from disk" → selecione esta pasta
2. Aguarde a importação dos pacotes (Package Manager resolve automaticamente)
3. Inicie o servidor Python PRIMEIRO
4. Abra a cena: Assets > Scenes > SentinelVR_ControlRoom
5. Pressione Play — o AnomalyDetector conectará automaticamente via WebSocket
```

---

### 3️⃣ Meta XR SDK (primeira configuração)

```
Window > Package Manager > My Assets > "Meta XR All-in-One SDK"
  ✅ Core SDK
  ✅ Interaction SDK
  ✅ Building Blocks
  ✅ Spatial Audio
  ❌ Sample Scenes (desmarcar)
  ❌ XR Interaction Toolkit Integration (desmarcar — evita conflitos)
```

---

### 4️⃣ Build para Meta Quest

```
File > Build Settings > Android > Switch Platform
Player Settings:
  Package Name:        com.TIC29.SentinelVR
  Minimum API Level:   Android 10 (Level 29)
  Scripting Backend:   IL2CPP
  Target Architecture: ARM64
  Texture Compression: ASTC
  Graphics API:        Vulkan (primário) · OpenGLES3 (fallback)
```

---

## 📜 Scripts Principais

| Script | Linguagem | Função |
|:---|:---:|:---|
| `AnomalyDetector.cs` | C# | Captura frames via RenderTexture e envia ao servidor Python via WebSocket |
| `MonitorController.cs` | C# | Controla borda de alerta piscante e `OVRSpatialAudioSource` por monitor |
| `DismissAlert.cs` | C# | Detecta interação via `PointableUnityEventWrapper` (Meta Interaction SDK) e dispensa alerta |
| `AnomalyAlertController.cs` | C# | Orquestra todos os monitores ao receber evento `OnAnomalyDetected` |
| `CameraCapture.cs` | C# | Captura frames da câmera de vigilância e encaminha ao detector |
| `anomaly_server.py` | Python | Servidor WebSocket assíncrono com ResNet50 + Autoencoder para detecção de anomalias |
| `train_autoencoder.py` | Python | Treinamento supervisionado do Autoencoder com frames normais das câmeras |

---

## 🎨 Assets e Cena

### Ambiente Virtual
- 🏢 Sala de controle **12×10m** com estética sci-fi
- 💡 Iluminação ambiente azul escuro `#0A0A2E` + point lights sobre estações
- 🌃 Skybox de cidade futurista noturna
- ✨ ~200 partículas holográficas azuis (`#00BFFF`, alpha 0.3)

### Sistema de Monitoramento
- 📺 **6 monitores** em semicírculo de 180° (Canvas World Space com RenderTexture 512×512)
- 🚨 Bordas de alerta vermelhas piscantes (`blinkInterval: 0.4s`)
- 🔊 Áudio espacial 3D direcional por câmera (`OVRSpatialAudioSource`)
- 📊 Painel de status com métricas do modelo de IA em tempo real

### Modelo de IA
```
ResNet50 (ImageNet) ──► Feature Vector [2048d]
        │
Autoencoder:   Encoder: 2048 → 512 → 128 → 64
               Decoder: 64 → 128 → 512 → 2048
        │
MSE Score  ──► Threshold 0.045 ──► Anomalia Detectada
```

---

## 👤 Autor

<table>
  <tr>
    <td align="center">
      <b>Idarlan Rogério Dias Magalhães</b><br/>
      Turma 7 — Residência em TIC 29<br/>
      Web 3.0 · 2026
    </td>
  </tr>
</table>

---

<p align="center">
  <sub>Projeto desenvolvido para a disciplina de Web 3.0 · Residência em TIC 29 · Prof. Ana Beatriz</sub>
</p>
