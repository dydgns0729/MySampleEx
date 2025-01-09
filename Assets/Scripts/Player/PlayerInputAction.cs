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
        #endregion

        #region NewInput SendMessage
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }
        #endregion

        public void MoveInput(Vector2 newMoveDirection)
        {
            Move = newMoveDirection;
        }
    }
}