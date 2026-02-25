# RenderTextures

Coloque aqui as RenderTextures que recebem o output de cada câmera de vigilância.

## RenderTextures necessárias:

| Arquivo | Câmera | Resolução recomendada |
|---|---|---|
| `RT_Camera01.renderTexture` | CAM_01 (entrada principal) | 512x512 |
| `RT_Camera02.renderTexture` | CAM_02 (corredor) | 512x512 |
| `RT_Camera03.renderTexture` | CAM_03 (área externa) | 512x512 |
| `RT_Camera04.renderTexture` | CAM_04 (cofre) | 512x512 |

## Como criar no Unity Editor:

Assets > Create > Render Texture → configurar resolução 512x512, formato ARGB32

Cada RenderTexture deve ser:
1. Atribuída ao campo `targetTexture` da Camera de vigilância correspondente (`CameraCapture.cs`)
2. Atribuída ao campo `cameraFeedTexture` do `MonitorController` correspondente
3. Atribuída ao campo `outputTexture` do `AnomalyDetector` que monitora aquela câmera
