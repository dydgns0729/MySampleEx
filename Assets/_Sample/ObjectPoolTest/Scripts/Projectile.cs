using System.Collections;
using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 발사체를 관리하는 클래스: 풀 해제 기능
    /// </summary>
    [RequireComponent(typeof(PooledObejct))]
    public class Projectile : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float timeoutDelay = 3f; //지연 시간이후 해체

        private PooledObejct pooledObejct;
        #endregion

        private void Awake()
        {
            //참조
            pooledObejct = GetComponent<PooledObejct>();
        }

        //Projectile 오브젝트 생성 후 바로 호출되는 함수
        public void Deactivate()
        {
            StartCoroutine(DeactiveRoutine(timeoutDelay));
        }

        IEnumerator DeactiveRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            //Rigidbody 이동 값 리셋
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

            //풀에 반납
            pooledObejct.Release();
            this.gameObject.SetActive(false);
        }
    }
}