using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace MySampleEx
{
    public class TouchManager : MonoBehaviour
    {
        #region Variables
        public GameObject touchCanvas;
        public CameraSettings cameraSettings;
        #endregion

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }

        private void Start()
        {
#if TOUCH_MODE
            touchCanvas.SetActive(true);
#else
            touchCanvas.SetActive(false);
#endif
        }

        private void Update()
        {
#if TOUCH_MODE
            if (Touch.activeTouches.Count > 0)
            {
                var touch = Touch.activeTouches[0];
                switch (touch.phase)
                {
                    case UnityEngine.InputSystem.TouchPhase.Began:
                        if (EventSystem.current.IsPointerOverGameObject())
                        {
                            cameraSettings.SetCinemachineInputAxisController(false);
                        }

                        break;
                    case UnityEngine.InputSystem.TouchPhase.Moved:
                        break;
                    case UnityEngine.InputSystem.TouchPhase.Ended:
                        cameraSettings.SetCinemachineInputAxisController(true);
                        break;

                }
            }
#else

#endif
        }
    }
}
