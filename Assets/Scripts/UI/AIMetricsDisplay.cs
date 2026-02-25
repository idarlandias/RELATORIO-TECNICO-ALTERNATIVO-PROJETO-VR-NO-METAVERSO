using UnityEngine;
using TMPro;
using SentinelVR.AI;

namespace SentinelVR.UI
{
    /// <summary>
    /// Exibe métricas em tempo real do sistema de IA na interface VR da sala de controle.
    /// Mostra score de anomalia atual, latência do servidor, status da conexão e histórico.
    /// </summary>
    public class AIMetricsDisplay : MonoBehaviour
    {
        [Header("Referências do Sistema")]
        [Tooltip("Detector de anomalias para leitura de métricas")]
        public AnomalyDetector anomalyDetector;

        [Header("Elementos de UI")]
        [Tooltip("Texto com score de anomalia atual")]
        public TextMeshPro anomalyScoreText;

        [Tooltip("Texto com status da conexão WebSocket")]
        public TextMeshPro connectionStatusText;

        [Tooltip("Texto com latência média do servidor (ms)")]
        public TextMeshPro latencyText;

        [Tooltip("Texto com número total de anomalias detectadas na sessão")]
        public TextMeshPro totalAnomaliesText;

        [Tooltip("Barra de progresso visual do score (fill image)")]
        public UnityEngine.UI.Image scoreProgressBar;

        [Header("Cores")]
        [Tooltip("Cor do score quando normal (abaixo do limiar)")]
        public Color normalScoreColor = Color.green;

        [Tooltip("Cor do score quando em alerta (acima do limiar)")]
        public Color alertScoreColor = Color.red;

        [Header("Configurações")]
        [Tooltip("Intervalo de atualização da UI (segundos)")]
        public float updateInterval = 0.1f;

        private int _totalAnomaliesCount = 0;
        private float _updateTimer = 0f;

        private void Start()
        {
            // TODO: Registrar no evento anomalyDetector.OnAnomalyDetected para incrementar contador
            // TODO: Inicializar todos os textos com valores padrão
        }

        private void Update()
        {
            // TODO: Atualizar UI a cada updateInterval segundos:
            //   - anomalyScoreText com anomalyDetector.GetLastAnomalyScore()
            //   - connectionStatusText com anomalyDetector.IsConnected()
            //   - scoreProgressBar.fillAmount com o score atual
            //   - Ajustar cor dos textos baseado no limiar
        }

        /// <summary>
        /// Incrementa o contador de anomalias detectadas nesta sessão.
        /// Chamado pelo evento OnAnomalyDetected do AnomalyDetector.
        /// </summary>
        private void OnAnomalyDetectedCallback(float score, Vector2 location)
        {
            _totalAnomaliesCount++;
            // TODO: Atualizar totalAnomaliesText
        }

        /// <summary>
        /// Reseta as métricas da sessão atual (score, contadores, latência).
        /// </summary>
        public void ResetMetrics()
        {
            _totalAnomaliesCount = 0;
            // TODO: Resetar todos os valores exibidos para estado inicial
        }
    }
}
