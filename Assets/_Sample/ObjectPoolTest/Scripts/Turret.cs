using UnityEngine;

namespace MySampleEx
{
    public class Turret : MonoBehaviour
    {
        #region Variables
        //[SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float muzzleVelocity = 700f;       //발사 속도
        [SerializeField] private Transform muzzle;                  //발사 총구
        [SerializeField] private float cooldownTime = 0.1f;

        [SerializeField] private ObjectPool objectPool;

        private float nextTimeToShoot;
        #endregion

        private void Update()
        {
            //마우스 좌클시 발사
            if (Input.GetMouseButton(0) && Time.time > nextTimeToShoot)
            {
                //발사체 새로 생성
                //GameObject bulletGo = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
                GameObject bulletGo = objectPool.GetPooledObejct().gameObject;

                if (bulletGo != null)
                {
                    bulletGo.SetActive(true);
                    bulletGo.transform.SetPositionAndRotation(muzzle.position, muzzle.rotation);

                    Rigidbody rb = bulletGo.GetComponent<Rigidbody>();
                    if(rb != null)
                    {
                        rb.AddForce(bulletGo.transform.forward * muzzleVelocity, ForceMode.Acceleration);
                    }
                }

                //Destroy(bulletGo, 1f);
                Projectile projectile = bulletGo.GetComponent<Projectile>();
                projectile?.Deactivate();

                nextTimeToShoot = Time.time + cooldownTime;
            }
        }
    }
}
