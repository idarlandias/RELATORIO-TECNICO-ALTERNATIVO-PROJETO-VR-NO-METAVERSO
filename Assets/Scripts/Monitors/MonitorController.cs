using UnityEngine;
using TMPro;
using System.Collections;

namespace SentinelVR.Monitors
{
    /// <summary>
    /// Controla estado visual + audio espacial de cada monitor de vigilancia.
    /// AlertAudioSource deve ter OVRSpatialAudioSource do Meta XR SDK para
    /// emitir o alarme na posicao 3D do monitor no espaco VR.
    /// SentinelVR — Residencia TIC 29
    /// </summary>
    public class MonitorController : MonoBehaviour
    {
        [Header("Identificacao")]
        public string monitorId    = "CAM_01";
        public int    monitorIndex = 0;

        [Header("Componentes Visuais")]
        public Renderer      feedRenderer;
        public RenderTexture cameraFeedTexture;
        public GameObject    alertBorder;
        public TextMeshPro   scoreLabel;
        public TextMeshPro   cameraLabel;

        [Header("Materials")]
        public Material normalMaterial;
        public Material alertMaterial;

        [Header("Audio Espacial — Meta XR SDK")]
        [Tooltip("AudioSource com OVRSpatialAudioSource. O som sera emitido na posicao 3D " +
                 "deste monitor no espaco VR, guiando a atencao do operador direcionalmente.")]
        public AudioSource alertAudioSource;

        [Header("Configuracoes")]
        public float blinkInterval = 0.4f;

        private bool      _isAlerting = false;
        private Coroutine _blinkCoroutine;
        private MaterialPropertyBlock _propBlock;

        private void Awake()
        {
            _propBlock = new MaterialPropertyBlock();

            if (feedRenderer != null && cameraFeedTexture != null)
            {
                feedRenderer.GetPropertyBlock(_propBlock);
                _propBlock.SetTexture("_BaseMap", cameraFeedTexture);
                feedRenderer.SetPropertyBlock(_propBlock);
            }

            if (cameraLabel != null) cameraLabel.text = monitorId;
            if (alertBorder  != null) alertBorder.SetActive(false);
            if (scoreLabel   != null) scoreLabel.gameObject.SetActive(false);
        }

        /// <summary>
        /// Ativa borda de alerta piscante e audio espacial 3D neste monitor.
        /// O operador ouve o alarme vindo da direcao desta camera no espaco VR.
        /// </summary>
        public void TriggerAlert(float anomalyScore)
        {
            if (_isAlerting) return;
            _isAlerting = true;

            if (feedRenderer != null && alertMaterial != null)
                feedRenderer.material = alertMaterial;

            if (alertBorder != null) alertBorder.SetActive(true);

            if (scoreLabel != null)
            {
                scoreLabel.text = $"SCORE: {anomalyScore:F3}";
                scoreLabel.gameObject.SetActive(true);
            }

            // Audio espacial: emite o som na posicao 3D do monitor (Meta OVRSpatialAudioSource)
            if (alertAudioSource != null) alertAudioSource.Play();

            _blinkCoroutine = StartCoroutine(BlinkBorder());
            Debug.Log($"[Monitor {monitorIndex + 1}] Alerta ativado — Score: {anomalyScore:F3}");
        }

        /// <summary>
        /// Dispensa o alerta: para piscar, silencia audio espacial, restaura visual normal.
        /// </summary>
        public void DismissAlert()
        {
            if (!_isAlerting) return;
            _isAlerting = false;

            if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
            if (alertBorder     != null) alertBorder.SetActive(false);
            if (scoreLabel      != null) scoreLabel.gameObject.SetActive(false);
            if (alertAudioSource != null) alertAudioSource.Stop();

            if (feedRenderer != null && normalMaterial != null)
                feedRenderer.material = normalMaterial;

            Debug.Log($"[Monitor {monitorIndex + 1}] Alerta dispensado pelo operador.");
        }

        private IEnumerator BlinkBorder()
        {
            bool visible = true;
            while (_isAlerting)
            {
                if (alertBorder != null) alertBorder.SetActive(visible);
                visible = !visible;
                yield return new WaitForSeconds(blinkInterval);
            }
        }

        public bool   IsInAlert()        => _isAlerting;
        public string GetMonitorId()     => monitorId;
        public float  GetAnomalyScore()  => scoreLabel != null && scoreLabel.gameObject.activeSelf
                                            ? float.Parse(scoreLabel.text.Replace("SCORE: ", "")) : 0f;
    }
}
