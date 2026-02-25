====================================================
  SentinelVR: Central de Monitoramento Inteligente
  Web 3.0 | Residencia em TIC 29 — Atividade Avaliativa Fase 2
====================================================

AUTOR
-----
Nome:   Idarlan Rogerio Dias Magalhaes
Turma:  Turma 7 — Residencia em TIC 29
Data:   08/03/2026


DESCRICAO DO PROJETO
--------------------
Central de monitoramento de seguranca em Realidade Virtual onde o
operador gerencia multiplos feeds de cameras de vigilancia em tempo
real. Um modelo de Inteligencia Artificial baseado em Autoencoder
Convolucional analisa continuamente os frames das cameras virtuais
via pipeline Python (WebSocket) e emite alertas visuais e sonoros
automaticamente ao detectar comportamentos anomalos.

Limitacao de hardware: placa de video integrada Intel UHD Graphics
sem suporte ao Vulkan API exigido pelo Meta Quest. O projeto foi
desenvolvido como planejamento tecnico completo e documentado.


STACK TECNICO
-------------
- Unity 6000.0.68f1 (Unity 6) + Universal Render Pipeline (URP 17.x)
- Meta XR SDK 85.x + OpenXR 1.16 + XR Management 4.5
- NativeWebSocket (endel/NativeWebSocket) — comunicacao Unity-Python
- Python 3.10 + PyTorch + WebSockets
- ResNet50 (feature extractor) + Autoencoder (deteccao de anomalias)


COMO ABRIR O PROJETO NO UNITY
------------------------------
1. Instale o Unity Hub e a versao Unity 6000.0.68f1
2. No Unity Hub, clique em "Add project from disk"
3. Selecione esta pasta (onde esta este README.txt)
4. Aguarde a importacao dos pacotes (Package Manager resolve automaticamente)
5. Abra a cena: Assets > Scenes > SampleScene


COMO RODAR O SERVIDOR PYTHON
-----------------------------
1. Acesse a pasta PythonServer/
2. Crie e ative um ambiente virtual:
     python -m venv venv
     venv\Scripts\activate        (Windows)
     source venv/bin/activate     (Linux/Mac)
3. Instale as dependencias:
     pip install -r requirements.txt
4. Inicie o servidor:
     python anomaly_server.py
5. O servidor ficara disponivel em ws://localhost:8765
6. No Unity, pressione Play — o AnomalyDetector conectara automaticamente


ESTRUTURA DO REPOSITORIO
------------------------
Assets/
  Scripts/
    AI/           → AnomalyDetector.cs, AnomalyAlertController.cs
    Monitors/     → MonitorController.cs, DismissAlert.cs
    Surveillance/ → CameraCapture.cs, PatrolMovement.cs, AnomalyTrigger.cs
    UI/           → AIMetricsDisplay.cs, ToggleAISystem.cs
    Player/       → PlayerController.cs
  Scenes/         → SampleScene.unity
  Materials/      → (adicionar materials de camera feed e alertas)
  RenderTextures/ → (adicionar RT_Camera_01..06.renderTexture)
  Audio/          → (adicionar alarm_beep.wav, server_room_hum.wav)
  Textures/       → (adicionar texturas do ambiente sci-fi)
  Prefabs/        → (adicionar prefabs de monitor e sala de vigilancia)

PythonServer/
  anomaly_server.py     → Servidor WebSocket principal (codigo completo)
  train_autoencoder.py  → Script de treinamento do autoencoder (stub)
  requirements.txt      → Dependencias Python
  README_server.md      → Instrucoes detalhadas do servidor

ProjectSettings/        → Configuracoes do Unity (XR, URP, Android)
Packages/
  manifest.json         → Dependencias do projeto (Meta XR SDK, NativeWebSocket)

Relatorio.md            → Relatorio tecnico completo (9 secoes)
Relatorio.docx          → Relatorio tecnico em formato Word
README.txt              → Este arquivo


REQUISITOS TECNICOS
-------------------
- Unity 6000.0.68f1
- Universal Render Pipeline (URP) 17.x
- Meta XR SDK 85.0.0
- com.unity.xr.openxr 1.16.1
- com.unity.xr.management 4.5.0
- com.unity.inputsystem 1.18.0
- com.unity.ai.navigation 2.0.10
- NativeWebSocket (via git: endel/NativeWebSocket#upm)
- Android Build Support (para build no Meta Quest)
- Python 3.10+ com PyTorch (servidor de IA)


====================================================
