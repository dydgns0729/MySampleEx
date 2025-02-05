using UnityEngine;

namespace MySampleEx
{
    public class PickupItem : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float verticalBobFrequency = 1f;   //이동 속도
        [SerializeField] private float bobingAmount = 1f;           //이동 거리
        [SerializeField] private float rotateSpeed = 360f;          //회전 속도

        private Vector3 startPosition;                              //시작 위치

        public ItemObject itemObject;
        #endregion

        private void Start()
        {
            //처음 위치 저장
            startPosition = transform.position;
        }

        private void Update()
        {
            //위 아래 흔들림
            float bobingAnimationPhase = Mathf.Sin(Time.time * verticalBobFrequency) * bobingAmount;
            transform.position = startPosition + Vector3.up * bobingAnimationPhase;

            //회전
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        }

        //충돌 체크
        private void OnTriggerEnter(Collider other)
        {
            //플레이어 체크
            if(other.tag == "Player")
            {
                PlayerController playerController = other.GetComponent<PlayerController>();

                if(playerController != null)
                {
                    //아이템 획득
                    if (playerController.PickupItem(itemObject) == true)
                    {
                        //획득성공효과 사운드/이펙트

                        //킬
                        Destroy(gameObject);
                    }
                }
                
            }
        }

        //아이템 획득 성공,실패 반환
        protected virtual bool OnPickup()
        {
            //탄환 7개 지급
            //Hp, Mp 아이템
            //...
            return true;
        }
    }
}