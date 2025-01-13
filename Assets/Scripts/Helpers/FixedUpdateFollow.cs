using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 현재 오브젝트를 특정 오브젝트의 위치에 부착
    /// </summary>
    [DefaultExecutionOrder(9999)]
    public class FixedUpdateFollow : MonoBehaviour
    {
        public Transform toFollow;      //부착할 대상

        private void FixedUpdate()
        {
            transform.position = toFollow.position;
            transform.rotation = toFollow.rotation;
        }
    }
}