using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// EnemyController
    /// </summary>
    public class EnemyController : MonoBehaviour, IMessageReceiver
    {
        #region Variables
        //데미지
        protected Damageable m_Damageable;          //Damageable 객체
        #endregion

        private void OnEnable()
        {
            m_Damageable = GetComponent<Damageable>();
            m_Damageable.onDamageMessageReceviers.Add(this);
            m_Damageable.IsInvulnerable = true;
        }

        private void OnDisable()
        {
            m_Damageable.onDamageMessageReceviers.Remove(this);
        }

        //메세지 인터페이스 기능 구현
        public void OnReceiveMessage(MessageType type, object sender, object msg)
        {
            switch (type)
            {
                case MessageType.Damaged:
                    {
                        Damageable.DamageMessage damageData = (Damageable.DamageMessage)msg;
                        Damaged(damageData);
                    }
                    break;
                case MessageType.Death:
                    {
                        Damageable.DamageMessage damageData = (Damageable.DamageMessage)msg;
                        Die(damageData);
                    }
                    break;
            }
        }

        //데미지 처리, 애니메이션, 연출(vfx, sfx), ...
        void Damaged(Damageable.DamageMessage damageMessage)
        {
            //TODO
        }

        //죽음 처리, 애니메이션, 연출(vfx, sfx), ...
        void Die(Damageable.DamageMessage damageMessage)
        {
            //TODO
            //퀘스트 처리
            QuestManager.Instance.UpdateQuest(QuestType.Kill, 0);

            Destroy(gameObject);
        }
    }
}