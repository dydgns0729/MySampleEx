using Unity.Cinemachine;
using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 프리룩 카메라 셋팅 설정
    /// </summary>
    public class CameraSettings : MonoBehaviour
    {
        #region Variables
        public CinemachineCamera freeLookCamera;
        public Transform follow;
        public Transform lookAt;
        #endregion

        private void Awake()
        {
            UpdateCameraSettings();
        }

        void UpdateCameraSettings()
        {
            freeLookCamera.Follow = follow;
            freeLookCamera.LookAt = lookAt;
        }
    }
}