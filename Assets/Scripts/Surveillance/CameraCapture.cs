using UnityEngine;

namespace SentinelVR.Surveillance
{
    /// <summary>
    /// Gerencia a câmera de vigilância física no ambiente 3D.
    /// Controla a RenderTexture que alimenta o monitor e o AnomalyDetector.
    /// Suporta rotação de pan/tilt e configuração de campo de visão.
    /// </summary>
    public class CameraCapture : MonoBehaviour
    {
        [Header("Configuração da Câmera")]
        [Tooltip("ID único desta câmera (deve corresponder ao monitorId do monitor associado)")]
        public string cameraId = "CAM_01";

        [Tooltip("Câmera Unity usada para renderização")]
        public Camera captureCamera;

        [Tooltip("RenderTexture de saída (alimenta monitor e AnomalyDetector)")]
        public RenderTexture outputTexture;

        [Header("Controles PTZ (Pan/Tilt/Zoom)")]
        [Tooltip("Ângulo horizontal máximo de pan (graus)")]
        public float maxPanAngle = 60f;

        [Tooltip("Ângulo vertical máximo de tilt (graus)")]
        public float maxTiltAngle = 30f;

        [Tooltip("Velocidade de rotação automática")]
        public float patrolSpeed = 10f;

        [Header("Estado")]
        [Tooltip("Se verdadeiro, câmera está em modo de patrulha automática")]
        public bool isPatrolling = true;

        private void Start()
        {
            // TODO: Configurar captureCamera.targetTexture = outputTexture
            // TODO: Inicializar posição de pan/tilt
        }

        private void Update()
        {
            // TODO: Se isPatrolling, chamar UpdatePatrolRotation()
        }

        /// <summary>
        /// Rotaciona a câmera para uma posição de pan/tilt específica.
        /// </summary>
        /// <param name="pan">Ângulo de pan em graus (-maxPanAngle a +maxPanAngle)</param>
        /// <param name="tilt">Ângulo de tilt em graus (-maxTiltAngle a +maxTiltAngle)</param>
        public void SetPanTilt(float pan, float tilt)
        {
            // TODO: Clampar valores e aplicar rotação ao transform
        }

        /// <summary>
        /// Atualiza o movimento de patrulha automática da câmera.
        /// </summary>
        private void UpdatePatrolRotation()
        {
            // TODO: Implementar oscilação senoidal de pan para simular câmera PTZ
        }

        /// <summary>
        /// Ativa ou desativa o modo de patrulha automática.
        /// </summary>
        public void SetPatrolling(bool active)
        {
            isPatrolling = active;
            // TODO: Pausar/retomar animação de patrulha
        }

        /// <summary>
        /// Retorna o ID desta câmera.
        /// </summary>
        public string GetCameraId() => cameraId;
    }
}
