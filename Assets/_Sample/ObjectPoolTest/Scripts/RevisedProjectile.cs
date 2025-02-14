using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

namespace MySampleEx
{
    public class RevisedProjectile : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float timeoutDelay = 3f; //지연 시간이후 해체

        private IObjectPool<RevisedProjectile> objectPool; //지금 오브젝트가 소속 되어 있는 풀
        public IObjectPool<RevisedProjectile> ObjectPool { get => objectPool; set => objectPool = value;}
        #endregion

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
            ObjectPool.Release(this);
        }
    }
}
