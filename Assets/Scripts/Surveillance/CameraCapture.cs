using UnityEngine;

namespace SentinelVR.Surveillance
{
    /// <summary>
    /// Gerencia a camera de vigilancia fisica no ambiente 3D.
    /// Configura RenderTexture e controla oscilacao de pan (patrulha automatica).
    /// SentinelVR — Residencia TIC 29
    /// </summary>
    public class CameraCapture : MonoBehaviour
    {
        [Header("Configuracao da Camera")]
        public string        cameraId      = "CAM_01";
        public Camera        captureCamera;
        public RenderTexture outputTexture;

        [Header("Patrulha PTZ")]
        public float maxPanAngle  = 60f;
        public float patrolSpeed  = 10f;
        public bool  isPatrolling = true;

        private float _panTime = 0f;

        private void Start()
        {
            if (captureCamera != null && outputTexture != null)
                captureCamera.targetTexture = outputTexture;
        }

        private void Update()
        {
            if (isPatrolling) UpdatePatrolRotation();
        }

        /// <summary>Oscilacao senoidal de pan simulando camera PTZ.</summary>
        private void UpdatePatrolRotation()
        {
            _panTime += Time.deltaTime * patrolSpeed * Mathf.Deg2Rad;
            float pan = Mathf.Sin(_panTime) * maxPanAngle;
            transform.localEulerAngles = new Vector3(
                transform.localEulerAngles.x, pan, 0f);
        }

        public void SetPatrolling(bool active) => isPatrolling = active;
        public string GetCameraId()            => cameraId;
    }
}
