using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MySampleEx
{
    /// <summary>
    /// 데미지 기능 구현 클래스
    /// </summary>
    public partial class Damageable : MonoBehaviour
    {
        #region Variables
        public int maxHitPoints;             //Max Heath
        public float invulnerablityTime;    //데미지 후 무적 타임

        [Range(0.0f, 360.0f)]
        public float hitAngle = 360.0f;
        [Range(0.0f, 360.0f)]
        public float hitForwardRotation = 360f;

        public bool IsInvulnerable {  get; set; }       //무적 여부
        public int CurrentHitPoints { get; private set; }       //Current Health

        public List<MonoBehaviour> onDamageMessageReceviers;

        protected float m_timeSinceLastHit = 0.0f;              //무적타이머 카운트다운

        public UnityAction OnDeath;
        public UnityAction OnReceiveDamage;
        public UnityAction OnHitWhileVulnerable;
        public UnityAction OnBecomeVulnerable;
        public UnityAction OnResetDamage;

        protected Collider m_Collier;
        private System.Action schedule;                         //등록된 함수를 LateUpdate 호출해서 실행
        #endregion

        private void Start()
        {
            //참조
            m_Collier = GetComponent<Collider>();

            //초기화
            ResetDamage();
        }

        private void Update()
        {
            //무적 타이머
            if(IsInvulnerable)
            {
                m_timeSinceLastHit += Time.deltaTime;
                if(m_timeSinceLastHit > invulnerablityTime)
                {
                    IsInvulnerable = false;
                    OnBecomeVulnerable?.Invoke();

                    m_timeSinceLastHit = 0f;
                }
            }
        }

        private void LateUpdate()
        {
            if (schedule != null)
            {
                schedule();
                schedule = null;
            }
        }

        //충돌체 활성화, 비활성
        public void SetColliderState(bool enabled)
        {
            m_Collier.enabled = enabled;
        }

        //데미지 데이터 초기화
        public void ResetDamage()
        {
            CurrentHitPoints = maxHitPoints;
            IsInvulnerable = false;
            m_timeSinceLastHit = 0.0f;
            OnResetDamage?.Invoke();
        }

        //데미지 입기
        public void TakeDamage(DamageMessage data)
        {
            //죽었으면
            if (CurrentHitPoints <= 0)
                return;

            //무적이면
            if (IsInvulnerable)
            {
                OnHitWhileVulnerable?.Invoke();
                return;
            }

            //Hit 방향 구하기
            Vector3 forward = transform.forward;
            forward = Quaternion.AngleAxis(hitForwardRotation, transform.up) * forward;

            Vector3 positionToDamager = data.damgeSource - transform.position;
            positionToDamager -= transform.up * Vector3.Dot(transform.up, positionToDamager);

            if (Vector3.Angle(forward, positionToDamager) > hitAngle * 0.5f)
                return;

            //데미지 처리
            IsInvulnerable = true;
            CurrentHitPoints -= data.amount;

            if(CurrentHitPoints <= 0)
            {
                if(OnDeath != null)
                {
                    schedule += OnDeath.Invoke;
                }
            }
            else
            {
                OnReceiveDamage?.Invoke();
            }

            //데미지 메세지 보내기
            var messageType = CurrentHitPoints <= 0 ? MessageType.Death : MessageType.Damaged;
            for (int i = 0; i < onDamageMessageReceviers.Count; i++)
            {
                var receiver = onDamageMessageReceviers[i] as IMessageReceiver;
                receiver.OnReceiveMessage(messageType, this, data);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 forward = transform.forward;
            forward = Quaternion.AngleAxis(hitForwardRotation, transform.up) * forward;

            if (Event.current.type == EventType.Repaint)
            {
                UnityEditor.Handles.color = Color.blue;
                UnityEditor.Handles.ArrowHandleCap(0, transform.position,
                    Quaternion.LookRotation(forward), 1.0f, EventType.Repaint);
            }

            UnityEditor.Handles.color = new Color(1.0f, 0.0f, 0.5f);
            forward = Quaternion.AngleAxis(-hitAngle * 0.5f, transform.up) * forward;
            UnityEditor.Handles.DrawSolidArc(transform.position, transform.up,
                forward, hitAngle, 1.0f);
        }
#endif
    }
}
