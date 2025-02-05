using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 등록된 오디오 클립중 랜덤하게 하나 플레이 한다
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class RandomAudioPlayer : MonoBehaviour
    {
        [SerializeField]
        public class SoundBank
        {
            public string name;
            public AudioClip[] clips;
        }

        #region Variables
        //재생속도 랜덤
        public bool randomizePitch = true;
        public float pitchRandomRange = 0.2f;

        public float playeDelay = 0;            //지연시간
        public SoundBank defaultBank = new SoundBank();

        protected AudioSource m_AudioSource;
        public AudioSource audioSource { get { return m_AudioSource; } }
        public AudioClip clip { get; private set; }
        #endregion

        private void Awake()
        {
            //참조
            m_AudioSource = GetComponent<AudioSource>();
        }

        public AudioClip PlayRandomClip(int bankId = 0)
        {
            return InternalPlayRandomClip(bankId);
        }

        AudioClip InternalPlayRandomClip(int bankId)
        {
            var bank = defaultBank;
            if(bank.clips == null || bank.clips.Length == 0)
                return null;

            //클립중 하나를 랜덤하게 선택
            var clip = bank.clips[Random.Range(0, bank.clips.Length)];
            if(clip == null)
                return null;

            m_AudioSource.pitch = randomizePitch ? 
                Random.Range(1.0f-pitchRandomRange, 1.0f+pitchRandomRange) : 1.0f;
            m_AudioSource.clip = clip;
            m_AudioSource.PlayDelayed(playeDelay);

            return clip;
        }
    }
}

