using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using NativeWebSocket;

namespace SentinelVR.AI
{
    /// <summary>
    /// Captura frames das cameras de vigilancia via RenderTexture e envia ao servidor
    /// Python de deteccao de anomalias via WebSocket.
    /// Utiliza exclusivamente Meta XR SDK — sem XR Interaction Toolkit.
    /// SentinelVR — Residencia TIC 29
    /// </summary>
    public class AnomalyDetector : MonoBehaviour
    {
        [Header("Cameras de Vigilancia")]
        public Camera[]         surveillanceCameras;
        public RenderTexture[]  cameraRenderTextures;
        public float            captureInterval = 0.5f;

        [Header("WebSocket")]
        public string serverUrl = "ws://localhost:8765";

        [Header("Eventos")]
        public UnityEvent<int, float> OnAnomalyDetected;
        public UnityEvent<int>        OnAnomalyCleared;

        private WebSocket _websocket;
        private Texture2D _captureBuffer;
        private bool      _isConnected     = false;
        private int       _currentCamIndex = 0;
        private float     _lastScore       = 0f;

        private async void Start()
        {
            _captureBuffer = new Texture2D(512, 512, TextureFormat.RGB24, false);
            _websocket     = new WebSocket(serverUrl);

            _websocket.OnOpen    += () => { _isConnected = true;
                Debug.Log("[SentinelVR] WebSocket conectado."); };
            _websocket.OnMessage += (bytes) =>
                ProcessAnomalyResponse(System.Text.Encoding.UTF8.GetString(bytes));
            _websocket.OnError   += (e) => Debug.LogWarning($"[SentinelVR] Erro WS: {e}");
            _websocket.OnClose   += (code) => { _isConnected = false;
                StartCoroutine(ReconnectAfterDelay(5f)); };

            await _websocket.Connect();
            InvokeRepeating(nameof(CaptureNextCamera), 2f, captureInterval);
        }

        private void Update()
        {
            // Meta XR SDK: despachar mensagens WebSocket no loop principal
            #if !UNITY_WEBGL || UNITY_EDITOR
            _websocket?.DispatchMessageQueue();
            #endif
        }

        private IEnumerator ReconnectAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (!_isConnected) await _websocket.Connect();
        }

        private void CaptureNextCamera()
        {
            if (!_isConnected || surveillanceCameras.Length == 0) return;
            _currentCamIndex = (_currentCamIndex + 1) % surveillanceCameras.Length;
            StartCoroutine(CaptureAndSend(_currentCamIndex));
        }

        private IEnumerator CaptureAndSend(int idx)
        {
            yield return new WaitForEndOfFrame();
            RenderTexture rt   = cameraRenderTextures[idx];
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;
            _captureBuffer.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            _captureBuffer.Apply();
            RenderTexture.active = prev;

            byte[] frameBytes = _captureBuffer.EncodeToPNG();
            byte[] payload    = new byte[4 + frameBytes.Length];
            Buffer.BlockCopy(BitConverter.GetBytes(idx), 0, payload, 0, 4);
            Buffer.BlockCopy(frameBytes, 0, payload, 4, frameBytes.Length);

            if (_websocket.State == WebSocketState.Open)
                await _websocket.Send(payload);
        }

        private void ProcessAnomalyResponse(string json)
        {
            try
            {
                AnomalyResult r = JsonUtility.FromJson<AnomalyResult>(json);
                _lastScore = r.score;
                if (r.is_anomaly) OnAnomalyDetected?.Invoke(r.camera, r.score);
                else              OnAnomalyCleared?.Invoke(r.camera);
            }
            catch (Exception e) { Debug.LogError($"[SentinelVR] Parse error: {e.Message}"); }
        }

        public float GetLastAnomalyScore() => _lastScore;
        public bool  IsConnected()         => _isConnected;

        private async void OnApplicationQuit()
        {
            if (_websocket != null && _websocket.State == WebSocketState.Open)
                await _websocket.Close();
        }

        [Serializable]
        private class AnomalyResult
        {
            public int   camera;
            public float score;
            public bool  is_anomaly;
        }
    }
}
