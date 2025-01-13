using UnityEngine;

namespace MySampleEx
{
    public class RandomStateSMB : StateMachineBehaviour
    {
        #region Variables

        public int numbersOfState = 3;          //랜덤상태의 개수
        public float minNormalTime = 0f;        //일반 상태의 최소 대기 시간
        public float maxNormalTime = 5f;        //일반 상태의 최대 대기 시간

        protected float m_RandomNormalTime;     //일반 상태의 대기 시간

        readonly int m_HashRandomIdle = Animator.StringToHash("RandomIdle");
        #endregion

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //대기 시간을 0~5초 사이로 설정
            m_RandomNormalTime = Random.Range(minNormalTime, maxNormalTime);

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
            {
                animator.SetInteger(m_HashRandomIdle, -1);
            }

            //타이머 체크 (stateInfo.normalizedTime 상태에 들어온 경과시간을 알려줌)
            if (stateInfo.normalizedTime > m_RandomNormalTime && !animator.IsInTransition(0))
            {
                int randNum = Random.Range(0, numbersOfState);
                animator.SetInteger(m_HashRandomIdle, randNum);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}
    }
}