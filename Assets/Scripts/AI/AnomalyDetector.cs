using System;
using System.Collections;
using UnityEngine;
using NativeWebSocket;

namespace SentinelVR.AI
{
    /// <summary>
    /// Captura frames da câmera de vigilância e envia via WebSocket para o servidor Python de detecção de anomalias.
    /// Recebe a resposta com score de anomalia e coordenadas para visualização em VR.
    /// </summary>
    public class AnomalyDetector : MonoBehaviour
    {
        [Header("Configuração WebSocket")]
        [Tooltip("Endereço do servidor Python de detecção de anomalias")]
        public string serverUrl = "ws://localhost:8765";

        [Header("Câmera de Captura")]
        [Tooltip("Câmera de vigilância usada para captura de frames")]
        public Camera surveillanceCamera;

        [Tooltip("Resolução de captura (width x height)")]
        public int captureWidth = 224;
        public int captureHeight = 224;

        [Header("Configuração de Detecção")]
        [Tooltip("Intervalo entre capturas (segundos)")]
        public float captureInterval = 0.5f;

        [Tooltip("Limiar acima do qual uma anomalia é reportada")]
        [Range(0f, 1f)]
        public float anomalyThreshold = 0.7f;

        [Header("Referências")]
        [Tooltip("Controlador de alerta de anomalia para notificação visual")]
        public AnomalyAlertController alertController;

        // WebSocket connection
        private WebSocket _webSocket;
        private bool _isConnected = false;
        private float _captureTimer = 0f;

        // Render texture para captura
        private RenderTexture _captureRT;
        private Texture2D _captureTexture;

        // Último score recebido
        private float _lastAnomalyScore = 0f;

        // Eventos públicos
        public event Action<float, Vector2> OnAnomalyDetected;
        public event Action OnConnectionEstablished;
        public event Action OnConnectionLost;

        private void Start()
        {
            InitializeCaptureResources();
            ConnectWebSocket();
        }

        private void Update()
        {
            if (_webSocket != null)
            {
#if !UNITY_WEBGL || UNITY_EDITOR
                _webSocket.DispatchMessageQueue();
#endif
            }

            if (_isConnected && surveillanceCamera != null)
            {
                _captureTimer += Time.deltaTime;
                if (_captureTimer >= captureInterval)
                {
                    _captureTimer = 0f;
                    CaptureAndSendFrame();
                }
            }
        }

        private void InitializeCaptureResources()
        {
            _captureRT = new RenderTexture(captureWidth, captureHeight, 24);
            _captureTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        }

        private async void ConnectWebSocket()
        {
            _webSocket = new WebSocket(serverUrl);

            _webSocket.OnOpen += () =>
            {
                Debug.Log("[AnomalyDetector] Conectado ao servidor de anomalias.");
                _isConnected = true;
                OnConnectionEstablished?.Invoke();
            };

            _webSocket.OnError += (error) =>
            {
                Debug.LogError($"[AnomalyDetector] Erro WebSocket: {error}");
            };

            _webSocket.OnClose += (code) =>
            {
                Debug.Log($"[AnomalyDetector] Conexão encerrada: {code}");
                _isConnected = false;
                OnConnectionLost?.Invoke();
                StartCoroutine(ReconnectAfterDelay(5f));
            };

            _webSocket.OnMessage += (bytes) =>
            {
                string json = System.Text.Encoding.UTF8.GetString(bytes);
                ProcessServerResponse(json);
            };

            await _webSocket.Connect();
        }

        private IEnumerator ReconnectAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (!_isConnected)
            {
                Debug.Log("[AnomalyDetector] Tentando reconectar...");
                ConnectWebSocket();
            }
        }

        private void CaptureAndSendFrame()
        {
            if (surveillanceCamera == null || !_isConnected) return;

            // Renderiza a câmera para a RenderTexture
            RenderTexture previousRT = surveillanceCamera.targetTexture;
            surveillanceCamera.targetTexture = _captureRT;
            surveillanceCamera.Render();
            surveillanceCamera.targetTexture = previousRT;

            // Lê os pixels da RenderTexture
            RenderTexture.active = _captureRT;
            _captureTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
            _captureTexture.Apply();
            RenderTexture.active = null;

            // Converte para JPEG e envia
            byte[] jpegBytes = _captureTexture.EncodeToJPG(75);
            string base64Frame = Convert.ToBase64String(jpegBytes);

            string payload = $"{{\"frame\":\"{base64Frame}\",\"threshold\":{anomalyThreshold}}}";
            _webSocket.SendText(payload);
        }

        private void ProcessServerResponse(string json)
        {
            try
            {
                ServerResponse response = JsonUtility.FromJson<ServerResponse>(json);
                _lastAnomalyScore = response.anomaly_score;

                if (response.anomaly_score >= anomalyThreshold)
                {
                    Vector2 location = new Vector2(response.location_x, response.location_y);
                    OnAnomalyDetected?.Invoke(response.anomaly_score, location);

                    if (alertController != null)
                    {
                        alertController.TriggerAlert(response.anomaly_score, location);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AnomalyDetector] Erro ao processar resposta: {ex.Message}");
            }
        }

        /// <summary>Retorna o último score de anomalia recebido do servidor.</summary>
        public float GetLastAnomalyScore() => _lastAnomalyScore;

        /// <summary>Verifica se a conexão WebSocket está ativa.</summary>
        public bool IsConnected() => _isConnected;

        private async void OnApplicationQuit()
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                await _webSocket.Close();
            }
        }

        private void OnDestroy()
        {
            if (_captureRT != null) _captureRT.Release();
            if (_captureTexture != null) Destroy(_captureTexture);
        }

        [Serializable]
        private class ServerResponse
        {
            public float anomaly_score;
            public float location_x;
            public float location_y;
            public string message;
        }
    }
}
