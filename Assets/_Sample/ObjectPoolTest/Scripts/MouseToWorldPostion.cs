using UnityEngine;
using UnityEngine.InputSystem;

namespace MySampleEx
{
    public class MouseToWorldPostion : MonoBehaviour
    {
        #region Variables
        //마우스 보여주기
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        [SerializeField] private Vector3 m_Offset;

        //마우스 위치 얻어 오기
        private Camera m_MainCamera;
        [SerializeField] private LayerMask m_LayerMask;
        [SerializeField] private ScreenDeadZone m_ScreenDeadZone;
        public ScreenDeadZone ScreenDeadZone => m_ScreenDeadZone;

        //마우스 위치
        private Vector3 m_Position;
        public Vector3 Position => m_Position;
        #endregion

        private void Awake()
        {
            if(m_MainCamera == null)
                m_MainCamera = Camera.main;
        }

        private void Update()
        {
            if(m_ScreenDeadZone.IsMouseInDeadZone())
            {
                if (m_SpriteRenderer != null)
                {
                    m_SpriteRenderer.enabled = false;
                }
                return;
            }    

            m_Position = GetMouseWorldPosition();

            if(m_SpriteRenderer != null)
            {
                m_SpriteRenderer.enabled = true;
                m_SpriteRenderer.transform.position = Position + m_Offset;
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            if(m_MainCamera == null)
                return Vector3.zero;
            
            RaycastHit hit;
            Ray ray = m_MainCamera.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, m_LayerMask))
            {
                //Transform hitObject = hit.transform;
                return new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }

            return Vector3.zero;
        }
    }
}
