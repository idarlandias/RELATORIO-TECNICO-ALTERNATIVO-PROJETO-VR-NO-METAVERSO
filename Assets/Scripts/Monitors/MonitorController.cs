using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SentinelVR.Monitors
{
    /// <summary>
    /// Controla o estado visual de um monitor de vigilância na sala de controle VR.
    /// Gerencia o feed de câmera, indicadores de status e alertas visuais.
    /// </summary>
    public class MonitorController : MonoBehaviour
    {
        [Header("Configuração do Monitor")]
        [Tooltip("ID único deste monitor (ex: CAM_01)")]
        public string monitorId = "CAM_01";

        [Tooltip("RenderTexture com o feed da câmera associada")]
        public RenderTexture cameraFeedTexture;

        [Header("Referências de UI")]
        [Tooltip("Renderer do quad que exibe o feed de câmera")]
        public Renderer feedRenderer;

        [Tooltip("Texto com ID/nome da câmera")]
        public TextMeshPro cameraLabel;

        [Tooltip("Indicador de status (verde = ok, vermelho = alerta)")]
        public Renderer statusIndicator;

        [Tooltip("Borda de alerta que pisca durante anomalia")]
        public Renderer alertBorder;

        [Tooltip("Texto com score de anomalia atual")]
        public TextMeshPro anomalyScoreText;

        [Header("Configurações Visuais")]
        [Tooltip("Material normal do feed (sem alerta)")]
        public Material normalMaterial;

        [Tooltip("Material de alerta (tinge o feed de vermelho)")]
        public Material alertMaterial;

        [Tooltip("Cor do indicador de status normal")]
        public Color statusNormalColor = Color.green;

        [Tooltip("Cor do indicador de status em alerta")]
        public Color statusAlertColor = Color.red;

        [Tooltip("Velocidade de piscada do alerta (Hz)")]
        public float blinkFrequency = 2f;

        // Estado interno
        private bool _isInAlert = false;
        private float _currentAnomalyScore = 0f;
        private float _blinkTimer = 0f;
        private bool _blinkState = false;
        private MaterialPropertyBlock _propBlock;

        private void Awake()
        {
            _propBlock = new MaterialPropertyBlock();
            InitializeMonitor();
        }

        private void Update()
        {
            if (_isInAlert)
            {
                UpdateAlertBlink();
            }
        }

        private void InitializeMonitor()
        {
            // Aplica o feed de câmera ao renderer
            if (feedRenderer != null && cameraFeedTexture != null)
            {
                feedRenderer.GetPropertyBlock(_propBlock);
                _propBlock.SetTexture("_BaseMap", cameraFeedTexture);
                feedRenderer.SetPropertyBlock(_propBlock);
            }

            // Define o label
            if (cameraLabel != null)
                cameraLabel.text = monitorId;

            // Estado inicial: normal
            SetNormalState();
        }

        /// <summary>
        /// Ativa o estado de alerta no monitor com o score fornecido.
        /// </summary>
        /// <param name="anomalyScore">Score de anomalia entre 0 e 1</param>
        public void SetAlert(float anomalyScore)
        {
            _isInAlert = true;
            _currentAnomalyScore = anomalyScore;

            // Aplica material de alerta
            if (feedRenderer != null && alertMaterial != null)
                feedRenderer.material = alertMaterial;

            // Atualiza indicador de status
            if (statusIndicator != null)
            {
                statusIndicator.GetPropertyBlock(_propBlock);
                _propBlock.SetColor("_BaseColor", statusAlertColor);
                statusIndicator.SetPropertyBlock(_propBlock);
            }

            // Exibe score
            if (anomalyScoreText != null)
                anomalyScoreText.text = $"ANOMALIA: {anomalyScore:P0}";

            // Mostra borda de alerta
            if (alertBorder != null)
                alertBorder.enabled = true;
        }

        /// <summary>
        /// Retorna o monitor ao estado normal (sem alerta).
        /// </summary>
        public void DismissAlert()
        {
            _isInAlert = false;
            _currentAnomalyScore = 0f;
            SetNormalState();
        }

        private void SetNormalState()
        {
            if (feedRenderer != null && normalMaterial != null)
                feedRenderer.material = normalMaterial;

            if (statusIndicator != null)
            {
                statusIndicator.GetPropertyBlock(_propBlock);
                _propBlock.SetColor("_BaseColor", statusNormalColor);
                statusIndicator.SetPropertyBlock(_propBlock);
            }

            if (anomalyScoreText != null)
                anomalyScoreText.text = "STATUS: OK";

            if (alertBorder != null)
                alertBorder.enabled = false;
        }

        private void UpdateAlertBlink()
        {
            _blinkTimer += Time.deltaTime;
            if (_blinkTimer >= 1f / blinkFrequency)
            {
                _blinkTimer = 0f;
                _blinkState = !_blinkState;

                if (alertBorder != null)
                    alertBorder.enabled = _blinkState;
            }
        }

        /// <summary>Retorna true se o monitor está em estado de alerta.</summary>
        public bool IsInAlert() => _isInAlert;

        /// <summary>Retorna o score de anomalia atual.</summary>
        public float GetAnomalyScore() => _currentAnomalyScore;

        /// <summary>Retorna o ID deste monitor.</summary>
        public string GetMonitorId() => monitorId;
    }
}
