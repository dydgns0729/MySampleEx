using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 플레이어 공격 이펙트 플레이
    /// </summary>
    public class EllenStaffEffectSMB1 : StateMachineBehaviour
    {
        public int effectIndex;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //지정 이펙트 플레이

        }
    }
}