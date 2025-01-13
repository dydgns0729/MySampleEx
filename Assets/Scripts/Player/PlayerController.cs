using Unity.Cinemachine;
using Unity.VisualScripting;
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

        //점프
        public float gravity = 20f;                 //중력값, 아래로 끌어당기는 가속도 값
        public float jumpSpeed = 10f;               //점프 속도

        protected bool m_ReadyToJump = false;       //점프 준비 단계
        protected float m_VerticalSpeed;            //위아래 이동 속도

        //대기 상태로 보내기
        public float idleTimeout = 5f;              //이동상태 대기에서 5초가 지나면 대기 상태로 보낸다
        protected float m_idleTimer = 0f;           //타이머 카운드

        //카메라(프리룩)
        public CameraSettings cameraSettings;

        //공격
        protected bool m_InAttack;                  //공격 여부 판단

        //상수
        const float k_GroundAcceleration = 20f;   //이동 가속도값
        const float k_GroundDeceleration = 25f;   //이동 감속도값
        const float k_GroundedRayDistance = 1f;   //그라운드 체크시 레이 거리값
        const float k_JumpAbortSpeed = 10f;       //공중에서 감속시키는 속도
        const float k_StickingGravityPropotion = 0.3f;  

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
        readonly int m_HashGrounded = Animator.StringToHash("Grounded");
        readonly int m_HashAirborneVerticalSpeed = Animator.StringToHash("AirborneVerticalSpeed");
        readonly int m_HashTimeoutIdle = Animator.StringToHash("TimeoutIdle");
        readonly int m_HashMeleeAttack = Animator.StringToHash("MeleeAttack");
        readonly int m_HashStateTime = Animator.StringToHash("StateTime");

        //애니메이션 상태 해시값
        readonly int m_HashLocomotion = Animator.StringToHash("Locomotion");
        readonly int m_HashAirborne = Animator.StringToHash("Airborne");
        readonly int m_HashLanding = Animator.StringToHash("Landing");
        #endregion

        private void Awake()
        {
            //참조
            m_Input = GetComponent<PlayerInputAction>();
            m_Animator = GetComponent<Animator>();
            m_CharCtrl = GetComponent<CharacterController>();

            cameraSettings = FindAnyObjectByType<CameraSettings>();
            if (cameraSettings != null)
            {
                if (cameraSettings.follow == null)
                    cameraSettings.follow = this.transform;
                if (cameraSettings.lookAt == null)
                    cameraSettings.lookAt = this.transform.Find("HeadTarget");
            }
        }

        private void FixedUpdate()
        {
            CacheAnimatorState();

            AttackState();

            CalculateForwardMovement();
            CalculateVerticalMovement();

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
            bool updateOrientationForAirbon = (!m_IsAnimatorTransitioning && m_CurrentStateInfo.shortNameHash == m_HashAirborne || m_NxetStateInfo.shortNameHash == m_HashAirborne);
            bool updateOrientationForLanding = (!m_IsAnimatorTransitioning && m_CurrentStateInfo.shortNameHash == m_HashLanding || m_NxetStateInfo.shortNameHash == m_HashLanding);

            return updateOrientationForLocomotion || updateOrientationForAirbon || updateOrientationForLanding;
        }

        //공격 처리
        void AttackState()
        {
            m_Animator.ResetTrigger(m_HashMeleeAttack);

            m_Animator.SetFloat(m_HashStateTime, 
                Mathf.Repeat(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
            if (m_Input.Attack)
                m_Animator.SetTrigger(m_HashMeleeAttack);
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
        
        //위, 아래 이동 속도 구하기
        void CalculateVerticalMovement()
        {
            if(m_IsGrounded)
            {
                m_ReadyToJump = true;
            }
            else
            {
                m_Input.Jump = false;
            }


            if (m_IsGrounded)
            {
                m_VerticalSpeed = -gravity * k_StickingGravityPropotion;

                if(m_Input.Jump && m_ReadyToJump)
                {
                    m_VerticalSpeed = jumpSpeed;

                    m_IsGrounded = false;
                    m_ReadyToJump = false;
                    m_Input.Jump = false;
                }
            }
            else
            {
                if(!m_Input.Jump && m_VerticalSpeed > 0f)
                {
                    m_VerticalSpeed -= k_JumpAbortSpeed * Time.deltaTime;
                }

                if(Mathf.Approximately(m_VerticalSpeed, 0f))
                {
                    m_VerticalSpeed = 0f;
                }

                m_VerticalSpeed -= gravity * Time.deltaTime;
            }
        }


        //이동 방향 계산
        void SetTargetRotation()
        {
            Vector2 moveInput = m_Input.Move;
            Vector3 localMovementDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            //TODO: Camera forward 구하기
            //Vector3 forward = Vector3.forward;
            Vector3 forward = Quaternion.Euler(0f,
                cameraSettings.freeLookCamera.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Value,
                0f) * Vector3.forward;
            forward.y = 0f;
            forward.Normalize();

            Quaternion targetRotation;
            if (Mathf.Approximately(Vector3.Dot(localMovementDirection, Vector3.forward), -1.0f))
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

        //이동상태의 대기에서 대기시간(5초)이 지나면 대기 상태로 보낸다
        void TimoutToIdle()
        {
            //입력값 체크(이동, 공격)
            bool inputDetected = IsMoveInput || m_Input.Jump || m_Input.Attack;

            //타이머 카운트
            if (m_IsGrounded && !inputDetected)
            {
                m_idleTimer += Time.deltaTime;
                if (m_idleTimer >= idleTimeout)
                {
                    m_Animator.SetTrigger(m_HashTimeoutIdle);

                    //초기화
                    m_idleTimer = 0;
                }
            }
            else
            {
                //초기화
                m_idleTimer = 0;
                m_Animator.ResetTrigger(m_HashTimeoutIdle);
            }

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

            //Y축 이동속도 적용
            movement += m_VerticalSpeed * Vector3.up * Time.deltaTime;

            //회전값 설정
            m_CharCtrl.transform.rotation *= m_Animator.deltaRotation;

            //이동
            m_CharCtrl.Move(movement);

            //애니메이션 적용
            m_IsGrounded = m_CharCtrl.isGrounded;

            if(m_IsGrounded == false)
                m_Animator.SetFloat(m_HashAirborneVerticalSpeed, m_VerticalSpeed);

            m_Animator.SetBool(m_HashGrounded, m_IsGrounded);
        }

    }
}