using Firebase.Database;
using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 게임 데이터를 Firebase에 관리(저장, 불러오기)하는 클래스
    /// </summary>
    public class FirebaseDatabaseManager : PersistentSingleton<FirebaseDatabaseManager>
    {
        #region Variables
        private DatabaseReference databaseRef;

        //게임데이터
        public StatsObject playerStats;
        public InventoryObject playerInventory;
        public InventoryObject playerEquipment;
        #endregion

        private void Start()
        {
            //참조
            databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        }
    }
}
