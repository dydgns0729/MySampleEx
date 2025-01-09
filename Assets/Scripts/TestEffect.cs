using UnityEngine;

namespace MySampleEx
{
    public class TestEffect : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            EffectManager.Instance.EffectOneShot((int)EffectList.NewEffect0, Vector3.zero);
        }
    }
}