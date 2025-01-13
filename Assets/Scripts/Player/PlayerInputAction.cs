using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MySampleEx
{
    /// <summary>
    /// 플레이어 인풋 관리 클래스
    /// </summary>
    public class PlayerInputAction : MonoBehaviour
    {
        #region Variables
        public Vector2 Move { get; private set; }       //이동 입력값
        public bool Jump { get; set; }                  //점프 입력값
        public bool Attack { get; private set; }                //공격 입력값

        private Coroutine m_AttackWaitCoroutine;        //공격 코루틴
        #endregion

        #region NewInput SendMessage
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnAttack(InputValue value)
        {
            AttackInput(value.isPressed);
        }
        #endregion

        public void MoveInput(Vector2 newMoveDirection)
        {
            Move = newMoveDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            Jump = newJumpState;
        }

        public void AttackInput(bool newAttackState)
        {
            Attack = newAttackState;

            if (m_AttackWaitCoroutine != null)
            {
                StopCoroutine(m_AttackWaitCoroutine);
            }
            m_AttackWaitCoroutine = StartCoroutine(AttackWait());
        }

        IEnumerator AttackWait()
        {
            yield return new WaitForSeconds(0.03f);
            Attack = false;
        }
    }
}