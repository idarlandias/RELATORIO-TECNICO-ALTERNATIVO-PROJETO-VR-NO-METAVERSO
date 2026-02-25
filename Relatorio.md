# RELATÓRIO TÉCNICO ALTERNATIVO — PROJETO VR NO METAVERSO
**Web 3.0 | Residência em TIC 29 — Atividade Avaliativa — Fase 2**

***

## SEÇÃO 1 — IDENTIFICAÇÃO

| Campo | Informação |
|---|---|
| **Nome Completo** | Idarlan Rogério Dias Magalhães |
| **Turma / Residência** | Turma 7 — Residência em TIC 29 |
| **Limitação de Hardware Relatada** | Placa de vídeo integrada Intel UHD Graphics sem suporte adequado ao Vulkan API exigido pelo Unity Editor com Meta XR SDK ativo. A GPU integrada não suporta renderização estereoscópica em tempo real com taxa de quadros mínima de 72 FPS exigida pelo Meta Quest, inviabilizando a execução e testes práticos do ambiente VR diretamente na máquina disponível. |

***

## SEÇÃO 2 — CONCEITO DO PROJETO

### 2.1 Nome do Projeto
**SentinelVR: Central de Monitoramento Inteligente com Detecção de Anomalias**

### 2.2 Contexto e Objetivo no Metaverso
O projeto propõe uma central de monitoramento de segurança futurista em Realidade Virtual, onde o operador gerencia múltiplos feeds de câmeras de vigilância transmitidos em tempo real. Um modelo de Inteligência Artificial baseado em **Autoencoder Convolucional** analisa continuamente os frames das câmeras virtuais via pipeline Python e emite alertas visuais e sonoros automaticamente ao detectar comportamentos anômalos no ambiente monitorado.

O problema que resolve é a curva de aprendizado de operadores de segurança, que precisam monitorar múltiplas câmeras simultaneamente por longos períodos com alto nível de atenção. Um sistema de IA que pré-filtra eventos suspeitos e apresenta os alertas de forma imersiva em VR reduz a carga cognitiva e melhora a taxa de detecção de incidentes reais.

### 2.3 Descrição Geral do Ambiente Virtual
Uma sala de controle de segurança futurista de 12x10 metros, com estética sci-fi. As paredes são revestidas de painéis metálicos escuros com detalhes em LED azul elétrico. O ambiente possui iluminação ambiente baixa (atmosfera de trabalho noturno), com fontes de luz pontuais sobre cada estação de trabalho.

O centro da sala conta com uma **mesa hexagonal de vidro translúcido** sobre a qual flutuam 6 monitores virtuais (Canvas World Space) dispostos em semicírculo, cada um exibindo o feed ao vivo de uma câmera de vigilância renderizada em RenderTexture. Dois painéis de alerta maiores flanqueiam o semicírculo, piscando em vermelho quando o modelo de IA detecta anomalia.

Na periferia da sala, há prateleiras com equipamentos eletrônicos decorativos, um painel de status do sistema ao fundo (mostrando métricas do modelo de IA em tempo real) e partículas de "faísca" holográfica flutuando no ar para reforçar a atmosfera tecnológica. O Skybox customizado simula uma vista externa de cidade à noite.

***

## SEÇÃO 3 — CONFIGURAÇÃO TÉCNICA DO PROJETO

### 3.1 Versão do Unity e Porquê
**Unity 6000.0.68f1 (Unity 6)**

Escolhida por ser a versão mais recente da geração Unity 6, que traz suporte nativo ao **Universal Render Pipeline (URP) 17.x**, necessário para os efeitos visuais sci-fi planejados (bloom, vinheta, aberração cromática via URP Volume). O Unity 6 oferece compatibilidade total com o **Meta XR SDK 85.x** e o **OpenXR 1.16**, além de melhorias significativas no sistema de RenderTexture com URP — fundamental para a captura de frames das câmeras virtuais. A versão também inclui o pacote `com.unity.ai.navigation` integrado, utilizado para o sistema de patrulha do agente NPC via NavMesh.

### 3.2 Instalação do Meta XR SDK (Passo a Passo)

1. Abrir Unity Hub → criar projeto 3D (URP Template), nome: `SentinelVR`
2. Acessar `Window > Package Manager`
3. Dropdown superior: selecionar **"My Assets"** (requer login na Unity Asset Store)
4. Buscar por **"Meta XR All-in-One SDK"**
5. Clicar em **"Download"** (aguardar ~500MB)
6. Após download, clicar em **"Import"**
7. Na janela de importação, manter marcados:
   - Core SDK
   - Interaction SDK
   - Building Blocks
   - XR Interaction Toolkit Integration
8. Desmarcar: Sample Scenes (reduzir tamanho do projeto)
9. Clicar em **"Import"** e aguardar compilação
10. Verificar no Console: ausência de erros críticos
11. Verificar criação da pasta `Assets/Oculus` no Project Window

### 3.3 Configurações de Build para Android/Meta Quest

**Switch de Plataforma:**
1. `File > Build Settings > Android > Switch Platform`
2. Aguardar recompilação de shaders (10-15 min)

**Player Settings (aba Android):**
- **Package Name:** `com.TIC29.SentinelVR`
- **Minimum API Level:** Android 10.0 (API Level 29) — compatibilidade Quest 2/3
- **Target API Level:** Android 13.0 (API Level 33)
- **Scripting Backend:** IL2CPP (obrigatório para ARM64)
- **Target Architectures:** ARM64 apenas (desmarcar ARMv7)
- **Texture Compression:** ASTC (melhor qualidade/tamanho para GPU Adreno)
- **Graphics API:** Vulkan (primário), OpenGLES3 (fallback)

**Quality Settings:**
- `Edit > Project Settings > Quality > Android`
- VSync Count: Don't Sync
- Anti Aliasing: 2x Multi Sampling
- Shadow Resolution: Medium

### 3.4 Configuração do XR Plugin Management

1. `Edit > Project Settings > XR Plugin Management`
2. Selecionar aba **Android** (ícone do robô)
3. Marcar **"Oculus"** na lista de Plug-in Providers
4. Aguardar instalação automática de dependências
5. Expandir **"Oculus"** nas configurações:
   - **Stereo Rendering Mode:** Multiview (melhor performance no Quest)
   - **Target Devices:** Quest 2 + Quest 3
   - **Phase Sync:** Habilitado (reduz latência de renderização)
   - **Application SpaceWarp:** Habilitado (mantém 72 FPS em cenas pesadas)
   - **Tracking Origin Mode:** Floor Level

### 3.5 Movimentação no PC (Editor)

**Solução:** XR Device Simulator do XR Interaction Toolkit

1. `Window > Package Manager > Unity Registry`
2. Buscar **"XR Interaction Toolkit"** → instalar versão 2.5.2+
3. Após instalação, em **Samples**, importar: **"XR Device Simulator"**
4. Na cena, criar `GameObject > Create Empty`, nomear **"XR Device Simulator"**
5. `Add Component > XR Device Simulator`
6. Criar asset de configuração: `Create > XR > Device Simulator Settings`, arrastar para o campo

**Controles no Editor:**
| Ação | Controle |
|---|---|
| Mover câmera (headset) | Botão direito do mouse + WASD |
| Subir / Descer | Q / E |
| Rotacionar visão | Mouse (com botão direito pressionado) |
| Mover controle esquerdo | Shift + Mouse |
| Mover controle direito | Ctrl + Mouse |
| Trigger esquerdo | T |
| Trigger direito | N |
| Grip direito | B |

***

## SEÇÃO 4 — ASSETS E ELEMENTOS DA CENA

| # | Nome | Tipo | Origem | Função |
|---|---|---|---|---|
| **ASSET 1** | Dark Metal Panel Texture | Textura PBR (PNG 2048x2048) | Poly Haven — `metal_plate_002` (CC0) | Revestimento das paredes e teto da sala de controle. Normal map para profundidade. |
| **ASSET 2** | Hexagonal Control Desk | Objeto 3D (FBX) | Primitivos Unity (Cylinder + Cubes) com ProBuilder | Mesa central hexagonal de vidro translúcido onde ficam posicionados os monitores virtuais. ~800 triângulos. |
| **ASSET 3** | Monitor Screen Frame | Objeto 3D (FBX) | Unity Asset Store — "Simple Monitor Pack" (Gratuito) | 6 frames de monitores dispostos em semicírculo. Cada frame recebe RenderTexture da câmera correspondente. ~500 tri cada. |
| **ASSET 4** | Surveillance Cameras (x6) | Objeto 3D + Component Camera | Primitivos Unity (Cylinders) + Unity Camera Component | Câmeras de segurança posicionadas em salas secundárias. Cada câmera renderiza para sua própria RenderTexture (512x512). |
| **ASSET 5** | Scifi LED Emissive Material | Material (URP Lit) | Criado no Unity com Emission habilitada | Material azul elétrico emissivo aplicado em detalhes das paredes, mesas e rodapés. Cor: #00BFFF, Intensity: 2.0. |
| **ASSET 6** | Alert Panel Texture | Textura 2D (PNG 1024x1024) | Criada no Canva (uso educacional) | Textura de painel de alerta com ícones de câmera, termômetro e status. Alternada programaticamente entre estado normal e alerta. |
| **ASSET 7** | Alarm Sound Effect | Áudio (WAV estéreo) | Freesound.org — `alarm_short_beep` (CC0) | Reproduzido ao detectar anomalia. Pitch alto (880Hz), 0.5 segundos, volume 60%. Evita fadiga auditiva. |
| **ASSET 8** | Ambient Hum Loop | Áudio (WAV mono, loop) | Freesound.org — `server_room_hum` (CC0) | Som ambiente de sala de servidores (ventoinhas, equipamentos). Volume 15%, loop contínuo. |
| **ASSET 9** | Procedural Scifi Skybox | Material Skybox | Unity Asset Store — "Skybox Series Free" (Gratuito) | Skybox de cidade futurista à noite, visível através das janelas virtuais da sala. |
| **ASSET 10** | Holographic Particles | Particle System | Unity Built-in Particle System | Partículas azuis translúcidas flutuando no ar da sala (~200 partículas ativas). Reforça atmosfera sci-fi sem impacto de performance. |

***

## SEÇÃO 5 — HIERARQUIA DE GAME OBJECTS

**Scene:** SentinelVR_ControlRoom

```
SentinelVR_ControlRoom
│
├── [--- MANAGEMENT ---]
│   ├── EventSystem
│   │     └── (Component: EventSystem + XRUIInputModule)
│   ├── XR Interaction Manager
│   │     └── (Component: XR Interaction Manager)
│   ├── AudioManager
│   │     ├── (Component: Audio Source — Ambient Hum Loop, loop: true, vol: 0.15)
│   │     └── AlertAudioSource
│   │           └── (Component: Audio Source — Alarm Beep, playOnAwake: false)
│   └── AnomalySystem
│         ├── (Script: WebSocketClient.cs — conexão com Python)
│         └── (Script: AnomalyAlertController.cs — gerencia alertas visuais)
│
├── [--- PLAYER ---]
│   └── XR Origin (Action-based)
│         └── Camera Offset
│               ├── Main Camera
│               │     └── (Component: Camera, Audio Listener, URP Camera Data)
│               ├── LeftHand Controller
│               │     ├── (Component: XR Controller Action-based)
│               │     ├── (Component: XR Ray Interactor)
│               │     └── (Component: Line Renderer — Ray Visual, cor branca)
│               └── RightHand Controller
│                     ├── (Component: XR Controller Action-based)
│                     ├── (Component: XR Ray Interactor)
│                     └── (Component: Line Renderer — Ray Visual, cor branca)
│
├── [--- ENVIRONMENT ---]
│   ├── Room_Structure
│   │     ├── Floor
│   │     │     └── (Plane 12x10, Material: Dark_Metal_Panel)
│   │     ├── Ceiling
│   │     │     └── (Plane 12x10, Material: Dark_Metal_Panel)
│   │     ├── Wall_North
│   │     │     ├── (Cube scaled, Material: Dark_Metal_Panel)
│   │     │     └── LED_Strips_North (x3 Quads, Material: Scifi_LED_Blue)
│   │     ├── Wall_South
│   │     │     ├── (Cube scaled, Material: Dark_Metal_Panel)
│   │     │     └── StatusBoard_Main
│   │     │           ├── (Canvas World Space — exibe métricas do modelo IA)
│   │     │           └── (Script: AIMetricsDisplay.cs)
│   │     ├── Wall_East
│   │     │     └── Window_Frame_East
│   │     │           └── Window_Glass (Material: Glass_Transparent)
│   │     └── Wall_West
│   │           └── Window_Frame_West
│   │                 └── Window_Glass (Material: Glass_Transparent)
│   │
│   ├── Lighting
│   │     ├── Ambient_Light (Directional, Intensity: 0.15, cor: #0A0A2E — azul noturno)
│   │     ├── PointLight_Station01 (Intensity: 1.2, Range: 3m, cor: branco frio)
│   │     ├── PointLight_Station02 (Intensity: 1.2, Range: 3m)
│   │     ├── PointLight_Station03 (Intensity: 1.2, Range: 3m)
│   │     └── LED_Ambient_Fill (Point, Intensity: 0.4, cor: #00BFFF — azul LED)
│   │
│   ├── Equipment_Decorative
│   │     ├── ServerRack_Left (FBX ou Primitivos empilhados)
│   │     ├── ServerRack_Right
│   │     └── Cable_Conduits (Cylinders finos nas paredes)
│   │
│   └── Atmosphere
│         ├── Skybox_Controller (Material: Scifi_Night_City)
│         └── Holographic_Particles
│               └── (Particle System, 200 particulas, cor: #00BFFF, alpha: 0.3)
│
├── [--- MAIN_CONTENT — CONTROL_CENTER ---]
│   ├── HexDesk_Central
│   │     ├── Desk_Surface (Cylinder achatado, Material: Glass_Translucent)
│   │     └── Desk_Base (Cylinder, Material: Dark_Metal_Panel)
│   │
│   ├── Monitor_Array (semicirculo de 6 monitores)
│   │     ├── Monitor_01
│   │     │     ├── Screen_Frame (FBX, Material: Monitor_Frame_Dark)
│   │     │     ├── Screen_Display (Quad, Material usa RenderTexture_Cam01)
│   │     │     ├── AlertBorder_01 (Quad borda, Material: Alert_Border)
│   │     │     └── (Script: MonitorController.cs — indice: 0)
│   │     ├── Monitor_02
│   │     │     ├── Screen_Display (Quad, Material usa RenderTexture_Cam02)
│   │     │     ├── AlertBorder_02
│   │     │     └── (Script: MonitorController.cs — indice: 1)
│   │     ├── Monitor_03 ... Monitor_06
│   │     │     └── (mesma estrutura dos anteriores, indices 2-5)
│   │     └── AlertPanel_Master (Canvas World Space, central, grande)
│   │           ├── Text_AlertStatus (TextMeshPro)
│   │           ├── Text_AnomalyScore (TextMeshPro)
│   │           └── Icon_AlertSymbol (Image)
│   │
│   └── Teleportation_Hub
│         ├── TeleportArea_MainStation (Component: Teleportation Area)
│         └── TeleportArea_SecondaryView (Component: Teleportation Area)
│
├── [--- SURVEILLANCE_ROOMS ---]
│   ├── Room_A (sala monitorada — objetos se movem)
│   │     ├── Geometry_RoomA (paredes, piso)
│   │     ├── Camera_01
│   │     │     ├── (Component: Camera, Target Texture: RenderTexture_Cam01)
│   │     │     └── (Script: CameraCapture.cs — captura frames para Python)
│   │     ├── MovingObject_A1 (Script: PatrolMovement.cs — movimento normal)
│   │     └── AnomalyObject_A (Script: AnomalyTrigger.cs — ativado randomicamente)
│   │
│   ├── Room_B ... Room_F
│   │     └── (mesma estrutura de Room_A, cameras 02-06)
│   └── (Rooms ficam "escondidas" do player — apenas cameras as capturam)
│
└── [--- INTERACTABLES ---]
      ├── DismissButton_01 ... DismissButton_06
      │     ├── (Sphere primitivo pequeno, ao lado de cada monitor)
      │     ├── (Component: XR Simple Interactable)
      │     ├── (Script: DismissAlert.cs — desativa alerta do monitor correspondente)
      │     └── Tag: "Interagivel"
      └── SystemToggle_Button
            ├── (Cube primitivo, na mesa central)
            ├── (Component: XR Simple Interactable)
            └── (Script: ToggleAISystem.cs — liga/desliga o pipeline de IA)
```

**Lógica de Ocultação das Salas:**
As `Surveillance_Rooms` são posicionadas fora do alcance visual do player (deslocadas no eixo X a +50 metros). Apenas as câmeras virtuais as capturam, gerando o efeito de monitoramento real sem que o usuário possa "entrar" nesses espaços diretamente.

***

## SEÇÃO 6 — SISTEMA DE INTERAÇÃO
*(Apenas na Atividade da Fase 2)*

### 6.1 Descrição da Interação Principal
O usuário está imerso na sala de controle, com 6 monitores exibindo feeds ao vivo. Ao apontar o raio do controlador para uma esfera **"DismissButton"** posicionada abaixo de cada monitor, ela emite um brilho verde de hover. Ao pressionar o **Trigger Button**, o alerta daquele monitor é **dispensado**: a borda vermelha apaga, o painel de alerta central atualiza o status e um som de confirmação é reproduzido. O botão central **SystemToggle** permite pausar e retomar o pipeline de IA (com visual de feedback no StatusBoard da parede).

### 6.2 Lógica da Interação (Exemplo de Passos)

**Fluxo A — Alerta Emitido pelo Modelo de IA:**

1. `CameraCapture.cs` captura frame da `Camera_01` a cada 0.5s via `RenderTexture` e converte para bytes PNG.
2. Frame é enviado via WebSocket para o servidor Python (porta 8765) como mensagem binária.
3. Python recebe o frame → decodifica → extrai features via **ResNet50 feature extractor** (camada anterior ao classificador).
4. **Autoencoder** reconstrói o frame a partir das features e calcula o erro de reconstrução (MSE).
5. Se `MSE > threshold` (0.045 determinado em calibração), considera anomalia.
6. Python envia de volta JSON: `{"camera": 0, "score": 0.087, "is_anomaly": true}`.
7. `WebSocketClient.cs` recebe a mensagem → dispara evento `OnAnomalyDetected(cameraIndex, score)`.
8. `AnomalyAlertController.cs` ouve o evento e chama `MonitorController.TriggerAlert(score)`.
9. `MonitorController` ativa a borda vermelha (`AlertBorder_01.SetActive(true)`) com animação de piscar.
10. `AlertAudioSource` reproduz o beep de alarme.
11. `AlertPanel_Master` exibe: "ANOMALIA — CAM 01 | Score: 0.087".

**Fluxo B — Usuário Dispensa o Alerta:**

1. Usuário aponta o raio do controlador para `DismissButton_01`.
2. `XR Ray Interactor` detecta hover → `OnHoverEntered` → botão aumenta escala 10% e brilha verde.
3. Usuário pressiona Trigger → `OnSelectEntered`.
4. `DismissAlert.cs` chama `MonitorController.DismissAlert()`.
5. `AlertBorder_01` desativa (sem piscar).
6. `AlertPanel_Master` atualiza: "ALERTA CAM 01 DISPENSADO".
7. Som curto de confirmação (pitch alto positivo).
8. Log gravado no `StatusBoard` com timestamp.

### 6.3 Scripts C# e Python do Projeto

***

**`AnomalyDetector.cs` — Captura de Frames e Comunicação WebSocket**

```csharp
using UnityEngine;
using UnityEngine.Events;
using NativeWebSocket;
using System;
using System.Collections;

/// <summary>
/// Captura frames das câmeras de vigilância e envia para o servidor Python
/// de detecção de anomalias via WebSocket.
/// SentinelVR — Residência TIC 29
/// </summary>
public class AnomalyDetector : MonoBehaviour
{
    [Header("Câmeras de Vigilância")]
    [SerializeField] private Camera[] surveillanceCameras;
    [SerializeField] private RenderTexture[] cameraRenderTextures;
    [SerializeField] private float captureInterval = 0.5f;

    [Header("WebSocket")]
    [SerializeField] private string serverUrl = "ws://localhost:8765";

    [Header("Eventos")]
    public UnityEvent<int, float> OnAnomalyDetected;
    public UnityEvent<int> OnAnomalyCleared;

    private WebSocket websocket;
    private Texture2D captureBuffer;
    private bool isConnected = false;
    private int currentCameraIndex = 0;

    private async void Start()
    {
        captureBuffer = new Texture2D(512, 512, TextureFormat.RGB24, false);

        websocket = new WebSocket(serverUrl);

        websocket.OnOpen += () => {
            isConnected = true;
            Debug.Log("[SentinelVR] WebSocket conectado ao servidor Python.");
        };

        websocket.OnMessage += (bytes) => {
            string json = System.Text.Encoding.UTF8.GetString(bytes);
            ProcessAnomalyResponse(json);
        };

        websocket.OnError += (error) => {
            Debug.LogWarning($"[SentinelVR] Erro WebSocket: {error}");
        };

        websocket.OnClose += (code) => {
            isConnected = false;
            Debug.Log($"[SentinelVR] WebSocket fechado. Código: {code}");
        };

        await websocket.Connect();
        InvokeRepeating(nameof(CaptureNextCamera), 2f, captureInterval);
    }

    private void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
            websocket?.DispatchMessageQueue();
        #endif
    }

    private void CaptureNextCamera()
    {
        if (!isConnected || surveillanceCameras.Length == 0) return;
        currentCameraIndex = (currentCameraIndex + 1) % surveillanceCameras.Length;
        StartCoroutine(CaptureAndSend(currentCameraIndex));
    }

    private IEnumerator CaptureAndSend(int cameraIndex)
    {
        yield return new WaitForEndOfFrame();

        RenderTexture rt = cameraRenderTextures[cameraIndex];
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        captureBuffer.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        captureBuffer.Apply();

        RenderTexture.active = previous;

        byte[] frameBytes = captureBuffer.EncodeToPNG();

        byte[] cameraIndexBytes = BitConverter.GetBytes(cameraIndex);
        byte[] payload = new byte[4 + frameBytes.Length];
        Buffer.BlockCopy(cameraIndexBytes, 0, payload, 0, 4);
        Buffer.BlockCopy(frameBytes, 0, payload, 4, frameBytes.Length);

        if (websocket.State == WebSocketState.Open)
        {
            await websocket.Send(payload);
        }
    }

    private void ProcessAnomalyResponse(string json)
    {
        try
        {
            AnomalyResult result = JsonUtility.FromJson<AnomalyResult>(json);

            if (result.is_anomaly)
            {
                Debug.LogWarning($"[SentinelVR] Anomalia na CAM {result.camera + 1}! Score: {result.score:F3}");
                OnAnomalyDetected?.Invoke(result.camera, result.score);
            }
            else
            {
                OnAnomalyCleared?.Invoke(result.camera);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SentinelVR] Erro ao parsear resposta: {e.Message}");
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
            await websocket.Close();
    }

    [Serializable]
    private class AnomalyResult
    {
        public int camera;
        public float score;
        public bool is_anomaly;
    }
}
```

***

**`MonitorController.cs` — Controle Visual dos Monitores**

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Controla o estado visual de cada monitor de vigilância:
/// feed normal, alerta ativo, piscar, dismiss.
/// </summary>
public class MonitorController : MonoBehaviour
{
    [Header("Componentes Visuais")]
    [SerializeField] private GameObject alertBorder;
    [SerializeField] private TextMeshProUGUI scoreLabel;
    [SerializeField] private Material normalBorderMaterial;
    [SerializeField] private Material alertBorderMaterial;

    [Header("Configurações")]
    [SerializeField] private int monitorIndex;
    [SerializeField] private float blinkInterval = 0.4f;

    private bool isAlerting = false;
    private Coroutine blinkCoroutine;
    private MeshRenderer borderRenderer;

    private void Awake()
    {
        borderRenderer = alertBorder.GetComponent<MeshRenderer>();
        alertBorder.SetActive(false);
    }

    public void TriggerAlert(float anomalyScore)
    {
        if (isAlerting) return;

        isAlerting = true;
        alertBorder.SetActive(true);
        scoreLabel.text = $"SCORE: {anomalyScore:F3}";
        scoreLabel.gameObject.SetActive(true);

        blinkCoroutine = StartCoroutine(BlinkBorder());
        Debug.Log($"[Monitor {monitorIndex + 1}] Alerta ativado — Score: {anomalyScore:F3}");
    }

    public void DismissAlert()
    {
        if (!isAlerting) return;

        isAlerting = false;

        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        alertBorder.SetActive(false);
        scoreLabel.gameObject.SetActive(false);
        Debug.Log($"[Monitor {monitorIndex + 1}] Alerta dispensado pelo operador.");
    }

    private IEnumerator BlinkBorder()
    {
        bool visible = true;
        while (isAlerting)
        {
            alertBorder.SetActive(visible);
            visible = !visible;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
```

***

**`anomaly_server.py` — Servidor Python de Detecção de Anomalias**

```python
"""
SentinelVR - Servidor de Detecção de Anomalias
Residência TIC 29 - Web 3.0

Recebe frames PNG de câmeras virtuais Unity via WebSocket,
extrai features com ResNet50 e detecta anomalias via Autoencoder.
"""

import asyncio
import websockets
import numpy as np
import json
import struct
from io import BytesIO
from PIL import Image
import torch
import torch.nn as nn
import torchvision.transforms as transforms
import torchvision.models as models

HOST = "localhost"
PORT = 8765
ANOMALY_THRESHOLD = 0.045
IMG_SIZE = (224, 224)
FEATURE_DIM = 2048
LATENT_DIM = 64


class AnomalyAutoencoder(nn.Module):
    def __init__(self, input_dim=FEATURE_DIM, latent_dim=LATENT_DIM):
        super().__init__()

        self.encoder = nn.Sequential(
            nn.Linear(input_dim, 512),
            nn.ReLU(),
            nn.Dropout(0.2),
            nn.Linear(512, 128),
            nn.ReLU(),
            nn.Linear(128, latent_dim),
            nn.ReLU()
        )

        self.decoder = nn.Sequential(
            nn.Linear(latent_dim, 128),
            nn.ReLU(),
            nn.Linear(128, 512),
            nn.ReLU(),
            nn.Dropout(0.2),
            nn.Linear(512, input_dim),
            nn.Sigmoid()
        )

    def forward(self, x):
        latent = self.encoder(x)
        reconstructed = self.decoder(latent)
        return reconstructed

    def reconstruction_error(self, x):
        with torch.no_grad():
            reconstructed = self.forward(x)
            mse = nn.functional.mse_loss(reconstructed, x).item()
        return mse


device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

resnet = models.resnet50(weights=models.ResNet50_Weights.DEFAULT)
feature_extractor = nn.Sequential(*list(resnet.children())[:-1])
feature_extractor.eval().to(device)

autoencoder = AnomalyAutoencoder(FEATURE_DIM, LATENT_DIM).to(device)

try:
    autoencoder.load_state_dict(torch.load("sentinel_autoencoder.pth", map_location=device))
    autoencoder.eval()
except FileNotFoundError:
    print("[SentinelVR] Pesos nao encontrados — usando modelo nao treinado (demo).")

preprocess = transforms.Compose([
    transforms.Resize(IMG_SIZE),
    transforms.ToTensor(),
    transforms.Normalize(mean=[0.485, 0.456, 0.406],
                         std=[0.229, 0.224, 0.225])
])


def extract_features(image):
    tensor = preprocess(image).unsqueeze(0).to(device)
    with torch.no_grad():
        features = feature_extractor(tensor)
    return features.view(1, -1)


def detect_anomaly(camera_index, image):
    features = extract_features(image)
    score = autoencoder.reconstruction_error(features)

    result = {
        "camera": camera_index,
        "score": round(score, 4),
        "is_anomaly": score > ANOMALY_THRESHOLD
    }

    status = "ANOMALIA" if result["is_anomaly"] else "Normal"
    print(f"[CAM {camera_index + 1}] {status} | MSE Score: {score:.4f}")

    return result


async def handle_frame(websocket):
    print(f"[SentinelVR] Cliente conectado: {websocket.remote_address}")

    try:
        async for message in websocket:
            if isinstance(message, bytes) and len(message) > 4:
                camera_index = struct.unpack('<i', message[:4])[0]
                image_bytes = message[4:]
                image = Image.open(BytesIO(image_bytes)).convert("RGB")
                result = detect_anomaly(camera_index, image)
                await websocket.send(json.dumps(result))

    except websockets.exceptions.ConnectionClosedOK:
        print("[SentinelVR] Cliente desconectado normalmente.")
    except Exception as e:
        print(f"[SentinelVR] Erro no handler: {e}")


async def main():
    async with websockets.serve(handle_frame, HOST, PORT):
        print(f"[SentinelVR] Servidor ativo em ws://{HOST}:{PORT}")
        await asyncio.Future()


if __name__ == "__main__":
    asyncio.run(main())
```

***

## SEÇÃO 7 — PLANEJAMENTO DO REPOSITÓRIO GITHUB

### 7.1 Nome do Repositório
`SentinelVR-TIC29`

### 7.2 Estrutura de Pastas

```
SentinelVR-TIC29/
│
├── Assets/
│   ├── Scenes/
│   │   └── SentinelVR_ControlRoom.unity
│   │
│   ├── Scripts/
│   │   ├── AI/
│   │   │   ├── AnomalyDetector.cs
│   │   │   └── AnomalyAlertController.cs
│   │   ├── Monitors/
│   │   │   ├── MonitorController.cs
│   │   │   └── DismissAlert.cs
│   │   ├── Surveillance/
│   │   │   ├── CameraCapture.cs
│   │   │   ├── PatrolMovement.cs
│   │   │   └── AnomalyTrigger.cs
│   │   └── UI/
│   │       ├── AIMetricsDisplay.cs
│   │       └── ToggleAISystem.cs
│   │
│   ├── Prefabs/
│   ├── Materials/
│   ├── RenderTextures/
│   ├── Audio/
│   └── Textures/
│
├── PythonServer/
│   ├── anomaly_server.py
│   ├── train_autoencoder.py
│   ├── requirements.txt
│   └── README_server.md
│
├── ProjectSettings/
├── Packages/
│   ├── manifest.json
│   └── packages-lock.json
│
├── .gitignore
└── Relatorio.md
```

### 7.3 Conteúdo do README.md

```markdown
# SentinelVR: Central de Monitoramento Inteligente

Projeto de Realidade Virtual com detecção de anomalias por IA
desenvolvido para a disciplina de Web 3.0 — Residência em TIC 29.

## Descrição
Central de monitoramento VR futurista onde um Autoencoder Convolucional
analisa feeds de câmeras em tempo real e alerta o operador sobre
comportamentos anômalos detectados.

## Stack
- Unity 6000.0.68f1 + Meta XR SDK
- Python 3.10 + PyTorch + WebSockets
- ResNet50 (feature extractor) + Autoencoder (anomaly detection)
- NativeWebSocket (Unity x Python bridge)

## Como rodar o servidor Python
cd PythonServer/
pip install -r requirements.txt
python anomaly_server.py

## Como abrir no Unity
1. Clone este repositório
2. Abra no Unity Hub (versão 6000.0.68f1)
3. Abra a cena: Assets/Scenes/SentinelVR_ControlRoom.unity
4. Inicie o servidor Python primeiro
5. Pressione Play no Editor

**Autor:** Idarlan Rogério Dias Magalhães | Turma 7 — Residência TIC 29 — 2026
```

***

## SEÇÃO 8 — PLANO DE EXECUÇÃO PASSO A PASSO

1. **Etapa 1 — Setup do Projeto Unity (URP):** Criar novo projeto 3D URP no Unity 6000.0.68f1, configurar pastas base (`Scripts`, `Materials`, `Scenes`, `RenderTextures`), definir nome de pacote e identidade do projeto em Player Settings.

2. **Etapa 2 — Importação do Meta XR SDK:** Via Package Manager, importar Meta XR All-in-One SDK e XR Interaction Toolkit 2.5.2. Configurar XR Plugin Management para Android com Oculus Provider habilitado em modo Multiview.

3. **Etapa 3 — Configuração do XR Origin e Mãos do Player:** Usar Meta Building Blocks para adicionar XR Origin com controladores configurados. Adicionar XR Ray Interactor em ambas as mãos com Line Renderer para visualização do raio. Configurar XR Device Simulator para testes no Editor.

4. **Etapa 4 — Construção da Sala de Controle:** Modelar ambiente com primitivos Unity (cubos/planos escalonados para paredes, piso e teto). Aplicar materiais PBR metálicos escuros. Construir mesa central hexagonal com ProBuilder. Configurar iluminação ambiente azul escuro + point lights sobre estações de trabalho.

5. **Etapa 5 — Sistema de RenderTextures e Câmeras:** Criar 6 `RenderTexture` assets (512x512, RGB24). Criar 6 câmeras Unity nas Surveillance Rooms e apontar `Target Texture` de cada uma para sua respectiva RenderTexture. Criar materiais para os monitores usando as RenderTextures como albedo. Posicionar monitores em semicírculo na cena principal.

6. **Etapa 6 — Desenvolvimento do Servidor Python:** Implementar `anomaly_server.py` com WebSocket assíncrono, ResNet50 feature extractor e Autoencoder. Treinar Autoencoder com frames "normais" capturados das câmeras Unity (script `train_autoencoder.py`). Calibrar threshold de anomalia. Testar pipeline com frames sintéticos antes de integrar ao Unity.

7. **Etapa 7 — Integração Unity x Python via WebSocket:** Importar NativeWebSocket via Package Manager (endel/NativeWebSocket). Implementar `AnomalyDetector.cs` para capturar frames via `ReadPixels`, serializar como PNG + índice de câmera e enviar via WebSocket. Implementar `MonitorController.cs` para controle visual dos alertas. Conectar eventos `OnAnomalyDetected` aos monitores via Inspector.

8. **Etapa 8 — Interativos, Testes e Build:** Adicionar `DismissButton` para cada monitor com `XR Simple Interactable` e script `DismissAlert.cs`. Configurar teleporte entre estações. Testar fluxo completo no Editor com XR Device Simulator + servidor Python rodando. Verificar FPS > 72 no Stats. Realizar Build Android (APK) para Meta Quest.

***

## SEÇÃO 9 — REFLEXÃO FINAL

### 9.1 Aprendizado

O projeto SentinelVR consolidou a compreensão de um conceito fundamental em sistemas de IA modernos: a **separação de responsabilidades entre o ambiente de execução e o módulo de inferência**. O Unity atua como camada de renderização, interface e captura de dados; o Python atua como servidor de processamento de IA — cada um fazendo o que faz melhor, comunicando-se via WebSocket com baixa latência.

Esse padrão arquitetural — chamado de *inference-as-a-service* — é exatamente o mesmo utilizado em sistemas de produção reais (câmeras de segurança comerciais, sistemas ADAS em veículos, monitoramento industrial). Projetar um ambiente VR sobre ele torna o aprendizado tangível e imediatamente aplicável.

A utilização do **Autoencoder para detecção de anomalias** reforçou o entendimento de um paradigma importante em Machine Learning: aprender a representação do "normal" para detectar o "anômalo", sem necessidade de dados rotulados de anomalias — abordagem essencial em cenários onde falhas são raras e imprevisíveis.

### 9.2 Dificuldades Previstas

**Latência do Pipeline de IA:** O gargalo mais crítico será o tempo de processamento entre a captura do frame no Unity e o retorno do resultado Python. A cadeia completa (captura PNG → WebSocket → feature extraction → Autoencoder → resposta) deve ficar abaixo de 200ms para não degradar a experiência. Estratégias: usar `asyncio` no servidor, batch processing de múltiplos frames simultaneamente, e reduzir resolução de captura de 512x512 para 224x224 direto.

**Calibração do Threshold de Anomalia:** Um threshold muito baixo gerará falsos positivos constantes (sistema "gritando lobo"); muito alto deixará anomalias reais passarem. A calibração adequada exige um conjunto de frames normais suficientemente diverso para cobrir todos os estados possíveis da câmera. Plano: capturar pelo menos 2000 frames normais por câmera antes de treinar.

**Performance de RenderTextures:** Seis câmeras ativas renderizando simultaneamente a 72 FPS no Quest é inviável. Solução: usar Coroutine para alternar câmeras em sequência (uma por frame), reduzindo a carga de 6x para 1x o custo de render por frame.

### 9.3 Melhorias Futuras

**LSTM para Anomalias Temporais:** Substituir o Autoencoder estático por um **LSTM Autoencoder** que analisa sequências de frames, permitindo detectar anomalias comportamentais que se manifestam ao longo do tempo (ex: objeto se movendo em trajetória incomum), não apenas em frames isolados.

**Explicabilidade com GradCAM:** Integrar mapas de calor **GradCAM** gerados pelo Python e reprojetá-los sobre o feed da câmera no Unity, destacando visualmente em vermelho as regiões do frame que mais contribuíram para o score de anomalia — tornando a decisão da IA interpretável para o operador.

**Dashboard Analytics Externo:** Exportar logs de anomalias para um dashboard em **Streamlit ou Dash**, visualizando série temporal de scores por câmera, histograma de falsos positivos/negativos, e mapa de calor de frequência de anomalias por local — fechando o ciclo de análise além do VR.

***

**Data de Entrega:** 08/03/2026
**Versão:** 1.0
**Status:** Planejamento Técnico Completo
**Autor:** Idarlan Rogério Dias Magalhães | Turma 7 — Residência TIC 29
