using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 아이템 정보를 가지고 있는 오브젝트들을 모아놓은 스크립터블 오브젝트
    /// </summary>
    [CreateAssetMenu(fileName = "New ItemDataBase", menuName = "Inventory System/Item/New ItemDataBase")]
    public class ItemDataBase : ScriptableObject
    {
        #region Variables
        public ItemObject[] itemObjects;
        #endregion

        //인스펙터창에서 값을 조정할때마다 호출되는 함수
        //itemObjects에 있는 item의 id값을 설정
        private void OnValidate()
        {
            for (int i = 0; i < itemObjects.Length; i++)
            {
                if(itemObjects[i] == null)
                    continue;

                itemObjects[i].data.id = i;
            }
        }
    }
}
