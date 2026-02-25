using UnityEngine;
using TMPro;
using SentinelVR.AI;

namespace SentinelVR.UI
{
    /// <summary>
    /// Botão interativo VR para ativar/desativar o sistema de IA de detecção de anomalias.
    /// Exibe o estado atual do sistema e permite controle pelo operador na sala de controle.
    /// </summary>
    public class ToggleAISystem : MonoBehaviour
    {
        [Header("Referências")]
        [Tooltip("Detector de anomalias a ser controlado")]
        public AnomalyDetector anomalyDetector;

        [Header("UI do Botão")]
        [Tooltip("Texto exibido no botão (ON/OFF)")]
        public TextMeshPro buttonLabel;

        [Tooltip("Renderer do botão para feedback visual")]
        public Renderer buttonRenderer;

        [Tooltip("Texto de status do sistema abaixo do botão")]
        public TextMeshPro statusText;

        [Header("Cores")]
        [Tooltip("Cor do botão quando sistema está ativo")]
        public Color activeColor = Color.green;

        [Tooltip("Cor do botão quando sistema está desativado")]
        public Color inactiveColor = Color.gray;

        [Header("Estado Inicial")]
        [Tooltip("Se verdadeiro, sistema inicia ativo")]
        public bool startEnabled = true;

        private bool _isSystemEnabled = false;
        private MaterialPropertyBlock _propBlock;

        private void Awake()
        {
            _propBlock = new MaterialPropertyBlock();
        }

        private void Start()
        {
            // TODO: Inicializar estado com startEnabled
            // TODO: Atualizar UI para refletir estado inicial
        }

        /// <summary>
        /// Alterna o estado do sistema de IA (ativa/desativa).
        /// Chamado pela interação XR com este botão.
        /// </summary>
        public void Toggle()
        {
            _isSystemEnabled = !_isSystemEnabled;
            ApplySystemState(_isSystemEnabled);
        }

        /// <summary>
        /// Define o estado do sistema diretamente.
        /// </summary>
        /// <param name="enabled">True para ativar, false para desativar</param>
        public void SetSystemEnabled(bool enabled)
        {
            _isSystemEnabled = enabled;
            ApplySystemState(enabled);
        }

        private void ApplySystemState(bool enabled)
        {
            // TODO:
            // 1. Ativar/desativar anomalyDetector.enabled
            // 2. Atualizar buttonLabel.text ("SISTEMA: ON" / "SISTEMA: OFF")
            // 3. Atualizar cor do buttonRenderer via _propBlock
            // 4. Atualizar statusText com descrição do estado
            // 5. Tocar feedback háptico no controller VR
        }

        /// <summary>
        /// Retorna se o sistema de IA está atualmente ativo.
        /// </summary>
        public bool IsSystemEnabled() => _isSystemEnabled;
    }
}
