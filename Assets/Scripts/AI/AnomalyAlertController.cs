using UnityEngine;
using SentinelVR.Monitors;

namespace SentinelVR.AI
{
    /// <summary>
    /// Gerencia a exibição e descarte de alertas de anomalia em todos os monitores da sala de controle VR.
    /// Responsável por coordenar notificações visuais e sonoras quando uma anomalia é detectada.
    /// </summary>
    public class AnomalyAlertController : MonoBehaviour
    {
        [Header("Monitores")]
        [Tooltip("Lista de monitores gerenciados por este controlador")]
        public MonitorController[] monitors;

        [Header("Audio")]
        [Tooltip("AudioSource para tocar o som de alerta")]
        public AudioSource alertAudioSource;

        [Tooltip("Clipe de áudio tocado ao detectar anomalia")]
        public AudioClip alertSound;

        [Header("Configurações")]
        [Tooltip("Duração do alerta em segundos antes de auto-dismiss (0 = sem auto-dismiss)")]
        public float autoDismissDelay = 0f;

        private void Start()
        {
            // TODO: Inicializar referências e validar configuração
        }

        /// <summary>
        /// Dispara um alerta de anomalia no monitor correspondente à câmera detectora.
        /// </summary>
        /// <param name="anomalyScore">Score de anomalia retornado pelo servidor Python (0-1)</param>
        /// <param name="location">Coordenadas 2D normalizadas da anomalia no frame</param>
        public void TriggerAlert(float anomalyScore, Vector2 location)
        {
            // TODO:
            // 1. Identificar qual monitor corresponde à localização
            // 2. Chamar monitor.SetAlert(anomalyScore)
            // 3. Tocar alertAudioSource.PlayOneShot(alertSound)
            // 4. Se autoDismissDelay > 0, agendar DismissAlert via corrotina
            // 5. Emitir evento/log de auditoria
        }

        /// <summary>
        /// Descarta o alerta ativo no monitor especificado.
        /// </summary>
        /// <param name="monitorId">ID do monitor a ser resetado</param>
        public void DismissAlert(string monitorId)
        {
            // TODO:
            // 1. Encontrar monitor com ID correspondente em monitors[]
            // 2. Chamar monitor.DismissAlert()
            // 3. Atualizar estado de auditoria
        }

        /// <summary>
        /// Descarta todos os alertas ativos em todos os monitores.
        /// </summary>
        public void DismissAllAlerts()
        {
            // TODO: Iterar sobre todos os monitors e chamar DismissAlert()
        }

        /// <summary>
        /// Retorna verdadeiro se há algum monitor em estado de alerta ativo.
        /// </summary>
        public bool HasActiveAlerts()
        {
            // TODO: Verificar monitors[] e retornar true se algum IsInAlert()
            return false;
        }
    }
}
