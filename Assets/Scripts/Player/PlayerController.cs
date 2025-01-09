using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 플레이어 캐릭터 제어(이동, 애니메이션, ..) 관리 클레스
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        public float maxForwardSpeed = 8f;          //플레이어 최고 이동 속도
        public float minTurnSpeed = 400f;           //플레이어 최저 회전 속도
        public float maxTurnSpeed = 1200f;          //플레이어 최고 회전 속도

        protected PlayerInputAction m_Input;
        protected CharacterController m_CharCtrl;
        protected Animator m_Animator;

        //이동,회전
        protected bool m_IsGrounded = true;
        protected float m_DesiredForwardSpeed;      //플레이어 입력값에 따라 낼수 있는 최고 이동 속도
        protected float m_ForwardSpeed;             //플레이어 현재 이동 속도
        protected Quaternion m_TragetRotation;      //타겟을 향한 회전값
        protected float m_AngleDiff;                //플레이어의 회전값과 타겟의 회전값의 차이 각도

        //이동 입력값 체크
        protected bool IsMoveInput
        {
            get { return !Mathf.Approximately(m_Input.Move.sqrMagnitude, 0f); }
        }

        //상수
        const float k_GroundAcceleration = 20f;   //이동 가속도값
        const float k_GroundDeceleration = 25f;   //이동 감속도값
        const float k_GroundedRayDistance = 1f;   //그라운드 체크시 레이 거리값

        //애니메이션 상태
        protected bool m_IsAnimatorTransitioning;               //상태 전환 중이냐?
        protected AnimatorStateInfo m_CurrentStateInfo;         //현재 애니 상태 정보
        protected AnimatorStateInfo m_NxetStateInfo;            //다음 애니 상태 정보
        protected AnimatorStateInfo m_PreviousCurrentStateInfo; //현재 애니 상태 정보 저장
        protected AnimatorStateInfo m_PreviousNxetStateInfo;    //다음 애니 상태 정보 저장
        protected bool m_PreviousIsAnimatorTransitioning;       //상태 전환 중이냐? 저장

        //애니메이션 파라미터
        readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        readonly int m_HashAngleDeltaRad = Animator.StringToHash("AngleDeltaRad");
        readonly int m_HashInputDetected = Animator.StringToHash("InputDetected");

        //애니메이션 상태 해시값
        readonly int m_HashLocomotion = Animator.StringToHash("Locomotion");
        #endregion

        private void Awake()
        {
            //참조
            m_Input = GetComponent<PlayerInputAction>();
            m_Animator = GetComponent<Animator>();
            m_CharCtrl = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            CacheAnimatorState();
            CalculateForwardMovement();

            SetTargetRotation();
            if(IsOrientationUpdate() && IsMoveInput)
            {
                UpdateOrientation();
            }

            TimoutToIdle();
        }

        //애니메이션 상태값 구하기
        void CacheAnimatorState()
        {
            m_PreviousCurrentStateInfo = m_CurrentStateInfo;
            m_PreviousNxetStateInfo = m_NxetStateInfo;
            m_PreviousIsAnimatorTransitioning = m_IsAnimatorTransitioning;

            m_CurrentStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            m_NxetStateInfo = m_Animator.GetNextAnimatorStateInfo(0);
            m_IsAnimatorTransitioning = m_Animator.IsInTransition(0);
        }

        bool IsOrientationUpdate()
        {
            bool updateOrientationForLocomotion = (!m_IsAnimatorTransitioning && m_CurrentStateInfo.shortNameHash == m_HashLocomotion || m_NxetStateInfo.shortNameHash == m_HashLocomotion);

            return updateOrientationForLocomotion;
        }

        //(Forward)이동 속도 계산
        void CalculateForwardMovement()
        {
            Vector2 moveInput = m_Input.Move;
            if (moveInput.sqrMagnitude > 1f)
            {
                moveInput.Normalize();
            }

            m_DesiredForwardSpeed = moveInput.magnitude * maxForwardSpeed;

            //입력에 따라서 가속, 감속 결정
            float acceleration = IsMoveInput ? k_GroundAcceleration : k_GroundDeceleration;

            //현재 이동속도를 구한다
            m_ForwardSpeed = Mathf.MoveTowards(m_ForwardSpeed, m_DesiredForwardSpeed, acceleration * Time.deltaTime);

            //애니 입력값 설정
            m_Animator.SetFloat(m_HashForwardSpeed, m_ForwardSpeed);
        }

        //이동 방향 계산
        void SetTargetRotation()
        {
            Vector2 moveInput = m_Input.Move;
            Vector3 localMovementDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            //TODO: Camera forward 구하기
            Vector3 forward = Vector3.forward;

            Quaternion targetRotation;
            if (Mathf.Approximately(Vector3.Dot(localMovementDirection, forward), -1.0f))
            {
                targetRotation = Quaternion.LookRotation(-forward);
            }
            else
            {
                Quaternion cameraToInputOffest = Quaternion.FromToRotation(Vector3.forward, localMovementDirection);
                targetRotation = Quaternion.LookRotation(cameraToInputOffest * forward);
            }

            Vector3 resultingForward = targetRotation * Vector3.forward;

            float angleCurrent = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
            float angleTarget = Mathf.Atan2(resultingForward.x, resultingForward.z) * Mathf.Rad2Deg;

            m_AngleDiff = Mathf.DeltaAngle(angleCurrent, angleTarget);

            m_TragetRotation = targetRotation;
        }

        void UpdateOrientation()
        {
            //애니 입력값 설정
            m_Animator.SetFloat(m_HashAngleDeltaRad, m_AngleDiff * Mathf.Deg2Rad);

            //회전 구현
            Vector3 localInput = new Vector3(m_Input.Move.x, 0f, m_Input.Move.y);
            float groundedTurnSpeed = Mathf.Lerp(maxTurnSpeed, minTurnSpeed, m_ForwardSpeed / m_DesiredForwardSpeed);
            //공중회전속도, 그라운드회전 속도 구분해서 적용
            float acutalTurnSpeed = groundedTurnSpeed;
            m_TragetRotation = Quaternion.RotateTowards(transform.rotation, m_TragetRotation, acutalTurnSpeed * Time.deltaTime);

            transform.rotation = m_TragetRotation;
        }

        //시간이 경과되면 아이들 상태로 보낸다
        void TimoutToIdle()
        {
            //입력값 체크(이동, 공격)
            bool inputDetected = IsMoveInput;

            //애니 입력값 설정
            m_Animator.SetBool(m_HashInputDetected, inputDetected);
        }

        private void OnAnimatorMove()
        {
            Vector3 movement;

            if(m_IsGrounded)
            {
                RaycastHit hit;
                Ray ray = new Ray(transform.position + Vector3.up * k_GroundedRayDistance * 0.5f, -Vector3.up);
                if(Physics.Raycast(ray, out hit, k_GroundedRayDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    movement = Vector3.ProjectOnPlane(m_Animator.deltaPosition, hit.normal);
                }
                else
                {
                    movement = m_Animator.deltaPosition;
                }
            }
            else
            {
                movement = m_ForwardSpeed * transform.forward * Time.deltaTime;
            }

            //회전값 설정
            m_CharCtrl.transform.rotation *= m_Animator.deltaRotation;

            //이동
            m_CharCtrl.Move(movement);
        }

    }
}