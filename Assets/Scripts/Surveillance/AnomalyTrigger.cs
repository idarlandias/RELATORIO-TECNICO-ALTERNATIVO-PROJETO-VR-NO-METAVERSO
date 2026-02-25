using UnityEngine;
using SentinelVR.AI;

namespace SentinelVR.Surveillance
{
    /// <summary>
    /// Trigger físico no ambiente 3D que detecta presença de objetos/pessoas em zonas restritas.
    /// Complementa a detecção baseada em IA com detecção por física (colliders).
    /// Deve ter um Collider marcado como IsTrigger no mesmo GameObject.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AnomalyTrigger : MonoBehaviour
    {
        [Header("Configuração")]
        [Tooltip("Tags dos objetos que ativam este trigger (ex: Player, NPC)")]
        public string[] detectedTags = { "Player", "NPC" };

        [Tooltip("Score de anomalia gerado por este trigger (0-1)")]
        [Range(0f, 1f)]
        public float triggerAnomalyScore = 0.85f;

        [Tooltip("Descrição da zona monitorada (ex: 'Cofre Principal')")]
        public string zoneName = "Zona Restrita";

        [Header("Referências")]
        [Tooltip("Controlador de alertas para notificar quando trigger é ativado")]
        public AnomalyAlertController alertController;

        [Header("Debug")]
        [Tooltip("Exibe gizmos da área do trigger no editor")]
        public bool showGizmos = true;

        private void Start()
        {
            // TODO: Validar que o Collider associado é IsTrigger = true
            // TODO: Registrar esta zona no sistema de auditoria
        }

        private void OnTriggerEnter(Collider other)
        {
            // TODO: Verificar se other.tag está em detectedTags[]
            // TODO: Se sim, chamar alertController.TriggerAlert(triggerAnomalyScore, GetNormalizedPosition(other))
            // TODO: Logar evento de intrusão com timestamp
        }

        private void OnTriggerExit(Collider other)
        {
            // TODO: Logar saída da zona restrita
        }

        /// <summary>
        /// Converte a posição 3D do objeto para coordenadas 2D normalizadas (0-1).
        /// </summary>
        private Vector2 GetNormalizedPosition(Collider other)
        {
            // TODO: Projetar posição 3D no plano 2D da zona e normalizar
            return Vector2.zero;
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos) return;
            // TODO: Desenhar wireframe da área do trigger com cor de alerta
        }
    }
}
