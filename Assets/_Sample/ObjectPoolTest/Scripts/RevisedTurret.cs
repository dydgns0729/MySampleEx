using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Events;

namespace MySampleEx
{
    public class RevisedTurret : MonoBehaviour
    {
        #region Variables
        //풀에 들어가 오브젝트 프리팹
        [SerializeField] private RevisedProjectile projectilePrefab;

        //Fire
        [SerializeField] private float muzzleVelocity = 700f;       //발사 속도
        [SerializeField] private Transform muzzle;                  //발사 총구
        [SerializeField] private float cooldownTime = 0.1f;
        private float nextTimeToShoot;

        [SerializeField] private UnityEvent m_GunFire;              //발사시 등록된 함수 호출

        //풀
        private IObjectPool<RevisedProjectile> objectPool;
        [SerializeField] private bool collectionCheck = true;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxSize = 100;
        #endregion

        private void Awake()
        {
            //풀 셋팅
            objectPool = new ObjectPool<RevisedProjectile>(CreateProjectile,
               OnGetFromPool, OnReleaseToPool, OnDestoryPooledObject,
               collectionCheck, defaultCapacity, maxSize);
        }

        private RevisedProjectile CreateProjectile()
        {
            RevisedProjectile projectileInstance = Instantiate(projectilePrefab);
            projectileInstance.ObjectPool = objectPool;

            return projectileInstance;
        }

        private void OnGetFromPool(RevisedProjectile pooledObejct)
        {
            pooledObejct.gameObject.SetActive(true);
        }

        private void OnReleaseToPool(RevisedProjectile pooledObejct)
        {
            pooledObejct.gameObject.SetActive(false);
        }

        private void OnDestoryPooledObject(RevisedProjectile pooledObejct)
        {
            Destroy(pooledObejct.gameObject);
        }

        private void FixedUpdate()
        {
            //마우스 좌클시 발사
            if (Input.GetMouseButton(0) && Time.time > nextTimeToShoot)
            {
                Shoot();
            }
        }

        void Shoot()
        {
            //풀에서 오브젝트 꺼내기
            RevisedProjectile bulleObject = objectPool.Get();

            if (bulleObject == null)
            {
                return;
            }

            bulleObject.transform.SetPositionAndRotation(muzzle.position, muzzle.rotation);

            Rigidbody rb = bulleObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(bulleObject.transform.forward * muzzleVelocity, ForceMode.Acceleration);
            }

            bulleObject.Deactivate();
            nextTimeToShoot = Time.time + cooldownTime;

            m_GunFire?.Invoke();
        }

    }

}