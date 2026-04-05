using UnityEngine;
using Oculus.Interaction;
using SentinelVR.AI;

namespace SentinelVR.Monitors
{
    /// <summary>
    /// Dispensa o alerta do monitor via Meta Interaction SDK.
    /// Requer PointableUnityEventWrapper (Meta XR SDK) neste GameObject.
    /// NAO usa XR Simple Interactable (XR Interaction Toolkit).
    /// 
    /// Setup no Inspector:
    ///   1. Adicionar componente PointableUnityEventWrapper
    ///   2. Em WhenPointerEventRaised, referenciar este DismissAlert.OnPointerEvent
    ///   3. Referenciar targetMonitor e alertController
    /// SentinelVR — Residencia TIC 29
    /// </summary>
    [RequireComponent(typeof(PointableUnityEventWrapper))]
    public class DismissAlert : MonoBehaviour
    {
        [Header("Referencias")]
        [Tooltip("Monitor associado a este botao de dismiss")]
        public MonitorController targetMonitor;

        [Tooltip("Controlador geral de alertas")]
        public AnomalyAlertController alertController;

        [Header("Feedback Visual")]
        public Renderer buttonRenderer;
        public Color    normalColor = Color.white;
        public Color    hoverColor  = new Color(0f, 1f, 0f, 1f);
        public Color    pressColor  = new Color(0f, 0.8f, 0f, 1f);

        private PointableUnityEventWrapper _wrapper;
        private MaterialPropertyBlock      _propBlock;

        private void Awake()
        {
            _propBlock = new MaterialPropertyBlock();
            _wrapper   = GetComponent<PointableUnityEventWrapper>();
            _wrapper.WhenPointerEventRaised.AddListener(OnPointerEvent);
        }

        /// <summary>
        /// Chamado pelo PointableUnityEventWrapper do Meta Interaction SDK.
        /// Trata hover e selecao (trigger press).
        /// </summary>
        public void OnPointerEvent(PointerArgs args)
        {
            switch (args.PointerEvent)
            {
                case PointerEvent.Hover:
                    SetButtonColor(hoverColor);
                    break;

                case PointerEvent.Unhover:
                    SetButtonColor(normalColor);
                    break;

                case PointerEvent.Select:
                    SetButtonColor(pressColor);
                    DismissTargetAlert();
                    break;

                case PointerEvent.Unselect:
                    SetButtonColor(normalColor);
                    break;
            }
        }

        private void DismissTargetAlert()
        {
            if (targetMonitor == null || !targetMonitor.IsInAlert()) return;

            alertController?.DismissAlert(targetMonitor.monitorIndex);
            Debug.Log($"[DismissAlert] Alerta dispensado: {targetMonitor.monitorId}");
        }

        private void SetButtonColor(Color color)
        {
            if (buttonRenderer == null) return;
            buttonRenderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_BaseColor", color);
            buttonRenderer.SetPropertyBlock(_propBlock);
        }
    }
}
