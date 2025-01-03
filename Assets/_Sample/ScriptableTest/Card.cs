using UnityEngine;

namespace MySampleEx
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Scriptable Objects/Card")]
    public class Card : ScriptableObject
    {
        new public string name;
        public string description;

        public int mana;
        public int health;
        public int attack;

        public Sprite artImage;
    }
}