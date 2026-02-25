# Camera Feed Materials

Coloque aqui os materiais URP usados para exibir o feed das câmeras nos monitores da sala de controle.

## Materiais necessários:

| Arquivo | Descrição |
|---|---|
| `CameraFeed_Normal.mat` | Material padrão do feed (sem alerta) |
| `CameraFeed_Alert.mat` | Material com tinge vermelho para estado de alerta |

## Como criar no Unity Editor:

1. **CameraFeed_Normal.mat**: Assets > Create > Material → Shader: URP/Unlit, textura: RenderTexture da câmera
2. **CameraFeed_Alert.mat**: Duplicar Normal → adicionar tinge vermelho via Color tint

Esses materiais são referenciados pelo componente `MonitorController` nos campos `normalMaterial` e `alertMaterial`.
