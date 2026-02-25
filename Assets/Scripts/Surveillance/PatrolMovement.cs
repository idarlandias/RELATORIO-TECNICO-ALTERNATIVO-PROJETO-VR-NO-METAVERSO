using UnityEngine;
using UnityEngine.AI;

namespace SentinelVR.Surveillance
{
    /// <summary>
    /// Controla o movimento de patrulha de um agente de segurança (NPC) pela área monitorada.
    /// Usa NavMesh para navegação e define waypoints de patrulha.
    /// Requer NavMeshAgent no mesmo GameObject.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class PatrolMovement : MonoBehaviour
    {
        [Header("Waypoints de Patrulha")]
        [Tooltip("Lista de pontos de patrulha na ordem de visita")]
        public Transform[] waypoints;

        [Tooltip("Se verdadeiro, repete a rota em loop; se falso, inverte ao chegar no fim")]
        public bool loopRoute = true;

        [Header("Configurações de Movimento")]
        [Tooltip("Velocidade de caminhada normal")]
        public float walkSpeed = 1.5f;

        [Tooltip("Velocidade de corrida em caso de alerta")]
        public float runSpeed = 4f;

        [Tooltip("Distância para considerar que chegou ao waypoint")]
        public float waypointTolerance = 0.5f;

        [Tooltip("Tempo de espera em cada waypoint (segundos)")]
        public float waitTimeAtWaypoint = 2f;

        [Header("Estado")]
        [Tooltip("Se verdadeiro, agente está em modo de alerta (se move mais rápido)")]
        public bool isAlerted = false;

        private NavMeshAgent _agent;
        private int _currentWaypointIndex = 0;
        private bool _isWaiting = false;
        private float _waitTimer = 0f;

        private void Start()
        {
            // TODO: Inicializar _agent = GetComponent<NavMeshAgent>()
            // TODO: Definir velocidade inicial e mover para primeiro waypoint
        }

        private void Update()
        {
            // TODO: Verificar se chegou ao waypoint atual (distância <= waypointTolerance)
            // TODO: Se sim, iniciar espera; após waitTimeAtWaypoint, avançar para próximo
            // TODO: Atualizar _agent.speed baseado em isAlerted
        }

        /// <summary>
        /// Avança para o próximo waypoint na sequência de patrulha.
        /// </summary>
        private void MoveToNextWaypoint()
        {
            // TODO: Calcular próximo índice (loop ou pingpong) e chamar _agent.SetDestination()
        }

        /// <summary>
        /// Envia o agente para uma posição de emergência específica.
        /// </summary>
        /// <param name="emergencyPosition">Posição de destino de emergência</param>
        public void RespondToAlert(Vector3 emergencyPosition)
        {
            isAlerted = true;
            // TODO: Interromper patrulha e mover para emergencyPosition
        }

        /// <summary>
        /// Retorna o agente à rota de patrulha normal.
        /// </summary>
        public void ResumePatrol()
        {
            isAlerted = false;
            // TODO: Retomar patrulha pelo waypoint mais próximo
        }
    }
}
