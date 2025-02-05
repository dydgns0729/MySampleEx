using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 데미지 메세지 정의 클래스
    /// </summary>
    public partial class Damageable : MonoBehaviour
    {
        //메세지 리시버를 통해 전달할 데미지 데이터
        public struct DamageMessage
        {
            public MonoBehaviour damager;
            public int amount;

            public Vector3 direction;
            public Vector3 damgeSource;
            public bool throwing;

            public bool stopCamera;
        }
    }
}