using UnityEngine;
using SentinelVR.AI;

namespace SentinelVR.Monitors
{
    /// <summary>
    /// Componente interativo VR que permite ao usuário dispensar alertas de anomalia
    /// tocando/apertando o botão correspondente na sala de controle.
    /// Deve ser colocado em um GameObject com Collider para interação com raycasting XR.
    /// </summary>
    public class DismissAlert : MonoBehaviour
    {
        [Header("Referências")]
        [Tooltip("Monitor associado a este botão de dismiss")]
        public MonitorController targetMonitor;

        [Tooltip("Controlador geral de alertas")]
        public AnomalyAlertController alertController;

        [Header("Feedback Visual")]
        [Tooltip("Renderer do botão para feedback de hover/press")]
        public Renderer buttonRenderer;

        [Tooltip("Cor normal do botão")]
        public Color normalColor = Color.white;

        [Tooltip("Cor ao fazer hover com o controller VR")]
        public Color hoverColor = Color.yellow;

        [Tooltip("Cor ao pressionar")]
        public Color pressColor = Color.green;

        private void Start()
        {
            // TODO: Registrar nos eventos do XR Interaction Toolkit (OnSelectEntered, etc.)
        }

        /// <summary>
        /// Chamado quando o usuário seleciona/pressiona este botão no ambiente VR.
        /// Descarta o alerta do monitor associado.
        /// </summary>
        public void OnButtonPressed()
        {
            // TODO:
            // 1. Verificar se targetMonitor.IsInAlert()
            // 2. Chamar alertController.DismissAlert(targetMonitor.GetMonitorId())
            // 3. Tocar feedback háptico no controller
            // 4. Animar botão (pressColor por 0.2s, voltar a normalColor)
        }

        /// <summary>
        /// Chamado quando o controller VR entra na área de hover deste botão.
        /// </summary>
        public void OnHoverEnter()
        {
            // TODO: Mudar cor do buttonRenderer para hoverColor
        }

        /// <summary>
        /// Chamado quando o controller VR sai da área de hover deste botão.
        /// </summary>
        public void OnHoverExit()
        {
            // TODO: Retornar cor do buttonRenderer para normalColor
        }
    }
}
