using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace MySampleEx
{
    public class MeeleWeapon : MonoBehaviour
    {
        //무기 공격시 상대에게 데미지 입히는 포인트
        [System.Serializable]
        public class AttackPoint
        {
            public float radius;
            public Vector3 offset;
            public Transform attackRoot;

#if UNITY_EDITOR
            public List<Vector3> previousPositions = new List<Vector3>();
#endif
        }

        #region Variables
        public int damage = 1;          //hit시 데미지 포인트

        public AttackPoint[] attackPoints = new AttackPoint[0];     //공격 포인트
        public TimeEffect[] effects;                                //공격 스윙 이펙트

        public ParticleSystem hitParticlePrefab;                    //공격시 타격 이펙트
        public LayerMask targetLayers;

        protected GameObject m_Owner;

        protected Vector3[] m_PreviousPos = null;
        protected Vector3 m_Direction;

        protected bool m_IsThrowingHit = false;
        protected bool m_InAttack = false;

        public bool ThrowingHit
        {
            get { return m_IsThrowingHit; }
            set { m_IsThrowingHit = value; }
        }

        const int PARTICLE_COUNT = 10;
        protected ParticleSystem[] m_ParticlesPool = new ParticleSystem[PARTICLE_COUNT];
        protected int m_CurrentParticle = 0;

        protected static RaycastHit[] s_RaycastHitCache = new RaycastHit[32];
        protected static Collider[] s_ColliderCache = new Collider[32];
        #endregion

        private void Awake()
        {
            //타격 이펙트 풀 생성
            if (hitParticlePrefab != null)
            {
                for (int i = 0; i < PARTICLE_COUNT; i++)
                {
                    m_ParticlesPool[i] = Instantiate(hitParticlePrefab);
                    m_ParticlesPool[i].Stop();
                }
            }
        }

        public void SetOwner(GameObject owner)
        {
            this.m_Owner = owner;
        }

        public void BeginAttack(bool throwingAttack)
        {
            ThrowingHit = throwingAttack;

            m_PreviousPos = new Vector3[attackPoints.Length];

            for (int i = 0; i < attackPoints.Length; i++)
            {
                Vector3 worldPos = attackPoints[i].attackRoot.position +
                    attackPoints[i].attackRoot.TransformVector(attackPoints[i].offset);
                m_PreviousPos[i] = worldPos;

#if UNITY_EDITOR
                attackPoints[i].previousPositions.Clear();
                attackPoints[i].previousPositions.Add(m_PreviousPos[i]);
#endif
            }

            m_InAttack = true;
        }

        public void EndAttack()
        {
            m_InAttack = false;

#if UNITY_EDITOR
            for (int i = 0; i < attackPoints.Length; i++)
            {
                attackPoints[i].previousPositions.Clear();
            }
#endif
        }

        private void FixedUpdate()
        {
            if(m_InAttack)
            {
                //어택 포인트별 히트 판정
                for (int i = 0; i < attackPoints.Length; i++)
                {
                    AttackPoint apt = attackPoints[i];
                    Vector3 worldPos = attackPoints[i].attackRoot.position +
                        attackPoints[i].attackRoot.TransformVector(attackPoints[i].offset);
                    Vector3 attackVector = worldPos - m_PreviousPos[i];
                    if(attackVector.magnitude < 0.001f)
                    {
                        attackVector = Vector3.forward * 0.0001f;
                    }

                    Ray r = new Ray(worldPos, attackVector.normalized);
                    int contacts = Physics.SphereCastNonAlloc(r, apt.radius, s_RaycastHitCache, attackVector.magnitude,
                            ~0, QueryTriggerInteraction.Ignore);
                    for (int j = 0; j < contacts; j++)
                    {
                        Collider col = s_RaycastHitCache[i].collider;
                        if (col != null)
                        {
                            CheckDamage(col, apt);
                        }
                    }

                    m_PreviousPos[i] = worldPos;
#if UNITY_EDITOR
                    attackPoints[i].previousPositions.Add(m_PreviousPos[i]);
#endif
                }
            }
        }

        //콜라이더 확인후 데미지 주기
        private void CheckDamage(Collider other, AttackPoint apt)
        {
            //Damageable 콜라이더 확인
            Damageable d = other.GetComponent<Damageable>();
            if (d == null)
            {
                return;
            }

            //셀프 데미지 체크
            if(d.gameObject == m_Owner)
            {
                return;
            }

            //타겟 레이어 체그
            if((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            {
                return;
            }

            //데미지 데이터 가공후 데미지 주기
            Damageable.DamageMessage data;
            data.amount = damage;
            data.damager = this;
            data.direction = m_Direction.normalized;
            data.damgeSource = m_Owner.transform.position;
            data.throwing = ThrowingHit;
            data.stopCamera = false;

            d.TakeDamage(data);

            //타격 이펙트
            if (hitParticlePrefab != null)
            {
                m_ParticlesPool[m_CurrentParticle].transform.position = apt.attackRoot.transform.position;
                m_ParticlesPool[m_CurrentParticle].time = 0;
                m_ParticlesPool[m_CurrentParticle].Play();
                m_CurrentParticle = (m_CurrentParticle + 1) % PARTICLE_COUNT;
            }   
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < attackPoints.Length; i++)
            {
                AttackPoint attackPoint = attackPoints[i];

                if (attackPoint.attackRoot != null)
                {
                    Vector3 worldPos = attackPoint.attackRoot.TransformVector(attackPoint.offset);
                    Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
                    Gizmos.DrawSphere(attackPoint.attackRoot.position + worldPos, attackPoint.radius);
                }

                if(attackPoint.previousPositions.Count > 0)
                {
                    UnityEditor.Handles.DrawAAPolyLine(10, attackPoint.previousPositions.ToArray());
                }
            }        
        }
#endif
    }
}
