using UnityEngine;
using TMPro;
using SentinelVR.Monitors;

namespace SentinelVR.AI
{
    /// <summary>
    /// Orquestra alertas visuais em todos os monitores ao receber evento OnAnomalyDetected.
    /// Conectar OnAnomalyDetected e OnAnomalyCleared do AnomalyDetector via Inspector.
    /// SentinelVR — Residencia TIC 29
    /// </summary>
    public class AnomalyAlertController : MonoBehaviour
    {
        [Header("Monitores (indices 0-5 = CAM 1-6)")]
        public MonitorController[] monitors;

        [Header("Painel Central de Alerta")]
        public TextMeshPro alertStatusText;
        public TextMeshPro anomalyScoreText;
        public GameObject  alertIconActive;

        [Header("Audio Geral")]
        public AudioSource generalAlertAudio;

        private void Start()
        {
            if (alertIconActive != null) alertIconActive.SetActive(false);
            UpdateStatusPanel(-1, 0f, false);
        }

        /// <summary>
        /// Chamado pelo evento OnAnomalyDetected do AnomalyDetector (via Inspector).
        /// </summary>
        public void OnAnomalyDetected(int cameraIndex, float score)
        {
            if (cameraIndex < 0 || cameraIndex >= monitors.Length) return;

            monitors[cameraIndex].TriggerAlert(score);

            UpdateStatusPanel(cameraIndex, score, true);

            // Audio geral (complementar ao audio espacial de cada monitor)
            if (generalAlertAudio != null && !generalAlertAudio.isPlaying)
                generalAlertAudio.Play();

            Debug.LogWarning($"[AlertController] Anomalia CAM {cameraIndex + 1} | Score: {score:F3}");
        }

        /// <summary>
        /// Chamado pelo evento OnAnomalyCleared do AnomalyDetector (via Inspector).
        /// </summary>
        public void OnAnomalyCleared(int cameraIndex)
        {
            if (cameraIndex < 0 || cameraIndex >= monitors.Length) return;
            if (!monitors[cameraIndex].IsInAlert()) return;

            monitors[cameraIndex].DismissAlert();
            UpdateStatusPanel(cameraIndex, 0f, false);
        }

        /// <summary>
        /// Dispensa manualmente o alerta de um monitor (chamado pelo DismissAlert.cs).
        /// </summary>
        public void DismissAlert(int cameraIndex)
        {
            if (cameraIndex < 0 || cameraIndex >= monitors.Length) return;
            monitors[cameraIndex].DismissAlert();
            UpdateStatusPanel(cameraIndex, 0f, false);

            if (generalAlertAudio != null) generalAlertAudio.Stop();
        }

        public void DismissAllAlerts()
        {
            for (int i = 0; i < monitors.Length; i++) monitors[i].DismissAlert();
            UpdateStatusPanel(-1, 0f, false);
            if (generalAlertAudio != null) generalAlertAudio.Stop();
        }

        public bool HasActiveAlerts()
        {
            foreach (var m in monitors) if (m.IsInAlert()) return true;
            return false;
        }

        private void UpdateStatusPanel(int camIdx, float score, bool isAlert)
        {
            if (alertStatusText != null)
                alertStatusText.text = isAlert
                    ? $"ANOMALIA — CAM {camIdx + 1:00}"
                    : "SISTEMA NORMAL";

            if (anomalyScoreText != null)
                anomalyScoreText.text = isAlert
                    ? $"Score: {score:F3}"
                    : "Score: ---";

            if (alertIconActive != null)
                alertIconActive.SetActive(isAlert);
        }
    }
}
