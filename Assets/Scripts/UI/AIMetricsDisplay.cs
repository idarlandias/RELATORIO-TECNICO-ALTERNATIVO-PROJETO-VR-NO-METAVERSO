using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SentinelVR.AI;

namespace SentinelVR.UI
{
    /// <summary>
    /// Exibe metricas em tempo real do sistema de IA no painel StatusBoard da sala VR.
    /// SentinelVR — Residencia TIC 29
    /// </summary>
    public class AIMetricsDisplay : MonoBehaviour
    {
        [Header("Referencias do Sistema")]
        public AnomalyDetector anomalyDetector;

        [Header("Elementos de UI")]
        public TextMeshPro anomalyScoreText;
        public TextMeshPro connectionStatusText;
        public TextMeshPro totalAnomaliesText;
        public Image       scoreProgressBar;

        [Header("Cores")]
        public Color normalScoreColor = Color.green;
        public Color alertScoreColor  = Color.red;

        [Header("Configuracoes")]
        public float updateInterval   = 0.1f;
        public float anomalyThreshold = 0.045f;

        private int   _totalAnomalies = 0;
        private float _updateTimer    = 0f;

        private void Start()
        {
            if (anomalyDetector != null)
                anomalyDetector.OnAnomalyDetected.AddListener((cam, score) => _totalAnomalies++);

            RefreshUI(0f, false);
        }

        private void Update()
        {
            _updateTimer += Time.deltaTime;
            if (_updateTimer < updateInterval) return;
            _updateTimer = 0f;

            if (anomalyDetector == null) return;

            float score     = anomalyDetector.GetLastAnomalyScore();
            bool  connected = anomalyDetector.IsConnected();
            bool  isAlert   = score > anomalyThreshold;

            RefreshUI(score, isAlert);

            if (connectionStatusText != null)
                connectionStatusText.text = connected ? "WS: CONECTADO" : "WS: DESCONECTADO";

            if (totalAnomaliesText != null)
                totalAnomaliesText.text = $"Anomalias: {_totalAnomalies}";
        }

        private void RefreshUI(float score, bool isAlert)
        {
            if (anomalyScoreText != null)
            {
                anomalyScoreText.text  = $"Score: {score:F3}";
                anomalyScoreText.color = isAlert ? alertScoreColor : normalScoreColor;
            }

            if (scoreProgressBar != null)
                scoreProgressBar.fillAmount = Mathf.Clamp01(score / anomalyThreshold);
        }

        public void ResetMetrics()
        {
            _totalAnomalies = 0;
            RefreshUI(0f, false);
        }
    }
}
