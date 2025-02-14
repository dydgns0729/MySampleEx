using UnityEngine;

namespace MySampleEx
{
    public class AimAtMouse : MonoBehaviour
    {
        #region Variables
        [SerializeField] private MouseToWorldPostion m_MouseToWorldPostion;

        [SerializeField] private Transform panTransform;
        [SerializeField] private float panSpeed = 5f;

        [SerializeField] private Vector3 m_AimOffset;   //목표 위치 보정값
        private ScreenDeadZone m_ScreenDeadZone;
        #endregion

        private void Start()
        {
            //참조
            if(m_MouseToWorldPostion != null)
            {
                m_ScreenDeadZone = m_MouseToWorldPostion.ScreenDeadZone;
            }
        }

        private void Update()
        {
            if (m_MouseToWorldPostion == null)
                return;
            if (m_ScreenDeadZone.IsMouseInDeadZone())
                return;

            RotatePanTowards(m_MouseToWorldPostion.Position);
        }

        //타겟 위치를 바라보도록 회전
        private void RotatePanTowards(Vector3 targetPosition)
        {
            Vector3 targetDirection = m_AimOffset + targetPosition - panTransform.position;
            targetDirection.y = 0f;

            //targetDirection을 바라보는 회전값 구하기
            Quaternion targetRotaion = Quaternion.LookRotation(targetDirection);
            panTransform.rotation = Quaternion.Slerp(panTransform.rotation, targetRotaion, panSpeed * Time.deltaTime);
        }
    }
}
