using UnityEngine;
using TMPro;
using Oculus.Interaction;
using SentinelVR.AI;

namespace SentinelVR.UI
{
    /// <summary>
    /// Botao interativo VR para ativar/desativar o pipeline de IA.
    /// Usa PointableUnityEventWrapper do Meta Interaction SDK.
    /// NAO usa XR Simple Interactable (XR Interaction Toolkit).
    /// SentinelVR — Residencia TIC 29
    /// </summary>
    [RequireComponent(typeof(PointableUnityEventWrapper))]
    public class ToggleAISystem : MonoBehaviour
    {
        [Header("Referencias")]
        public AnomalyDetector anomalyDetector;

        [Header("UI")]
        public TextMeshPro buttonLabel;
        public TextMeshPro statusText;
        public Renderer    buttonRenderer;

        [Header("Cores — Meta XR SDK")]
        public Color activeColor   = new Color(0f, 0.8f, 0f);
        public Color inactiveColor = new Color(0.4f, 0.4f, 0.4f);

        [Header("Estado Inicial")]
        public bool startEnabled = true;

        private bool                   _isEnabled;
        private PointableUnityEventWrapper _wrapper;
        private MaterialPropertyBlock  _propBlock;

        private void Awake()
        {
            _propBlock = new MaterialPropertyBlock();
            _wrapper   = GetComponent<PointableUnityEventWrapper>();
            _wrapper.WhenPointerEventRaised.AddListener(OnPointerEvent);
        }

        private void Start()
        {
            _isEnabled = startEnabled;
            ApplyState(_isEnabled);
        }

        public void OnPointerEvent(PointerArgs args)
        {
            if (args.PointerEvent == PointerEvent.Select)
                Toggle();
        }

        public void Toggle()
        {
            _isEnabled = !_isEnabled;
            ApplyState(_isEnabled);
        }

        private void ApplyState(bool enabled)
        {
            if (anomalyDetector != null) anomalyDetector.enabled = enabled;

            if (buttonLabel != null)
                buttonLabel.text = enabled ? "SISTEMA: ON" : "SISTEMA: OFF";

            if (statusText != null)
                statusText.text = enabled
                    ? "Pipeline de IA ativo — monitorando"
                    : "Pipeline de IA pausado pelo operador";

            if (buttonRenderer != null)
            {
                buttonRenderer.GetPropertyBlock(_propBlock);
                _propBlock.SetColor("_BaseColor", enabled ? activeColor : inactiveColor);
                buttonRenderer.SetPropertyBlock(_propBlock);
            }

            Debug.Log($"[ToggleAISystem] Sistema de IA: {(enabled ? "ATIVO" : "PAUSADO")}");
        }

        public bool IsSystemEnabled() => _isEnabled;
    }
}
