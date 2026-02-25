# Audio

Coloque aqui os clipes de áudio do sistema SentinelVR.

## Arquivos necessários:

| Arquivo | Descrição | Formato recomendado |
|---|---|---|
| `alert_anomaly.wav` | Som tocado ao detectar anomalia | WAV 44.1kHz mono |
| `alert_dismiss.wav` | Som de confirmação ao dispensar alerta | WAV 44.1kHz mono |
| `system_on.wav` | Som ao ativar o sistema de IA | WAV 44.1kHz mono |
| `system_off.wav` | Som ao desativar o sistema de IA | WAV 44.1kHz mono |
| `ambient_control_room.wav` | Áudio ambiente da sala de controle | WAV 44.1kHz stereo, loop |

## Como usar:

Os clipes de áudio são referenciados pelo componente `AnomalyAlertController` no campo `alertSound`.
Configure o componente `AudioSource` no mesmo GameObject com `spatialBlend = 0` (2D) para os alertas.
