using Firebase.Auth;
using Firebase.Database;
using System;
using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 게임 데이터를 Firebase에 관리(저장하기, 불러오기)하는 클래스
    /// </summary>
    public class FirebaseDatabaseManager : PersistentSingleton<FirebaseDatabaseManager>
    {
        #region Variables
        private DatabaseReference databaseRef;

        //게임 데이터
        public StatsObject playerStats;
        public InventoryObject playerInventory;
        public InventoryObject playerEquipment;

        //저장,로드
        private string UserDataPath => "users";
        private string StatsDataPath => "stats";
        private string InventoryDataPath => "inventory";
        private string EquipmentDataPath => "equipment";

        public Action<int> OnChangeData;
        #endregion

        private void OnEnable()
        {
            playerStats.OnChagnedStats += OnChangedStats;
        }

        private void OnDisable()
        {
            playerStats.OnChagnedStats -= OnChangedStats;
        }

        private void Start()
        {
            //참조
            databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        }

        //스탯정보 변경시 호출되어 저장하기
        public void OnChangedStats(StatsObject statsObject = null)
        {
            OnSavePayerStats();
        }

        //저장하기
        public async void OnSavePayerStats()
        {
            int result = 0;

            var userId = FirebaseAuthManager.Instance.UserId;
            if (userId == string.Empty)
            {
                return;
            }

            //저장할 데이터
            string statsJson = playerStats.ToJson();
            await databaseRef.Child(UserDataPath).Child(userId).Child(StatsDataPath).SetRawJsonValueAsync(statsJson).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("SetRawJsonValueAsync was canceled");
                    result = 1;
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.Log("SetRawJsonValueAsync was failed");
                    result = 1;
                    return;
                }
                Debug.Log($"userstats data save success: {userId}, {statsJson}");
            });

            OnChangeData?.Invoke(result);
        }


        //불러오기
        public async void OnLoad()
        {
            int result = 0;

            var userId = FirebaseAuthManager.Instance.UserId;
            if (userId == string.Empty)
            {
                return;
            }

            //스탯 가져오기
            await databaseRef.Child(UserDataPath).Child(userId).Child(StatsDataPath).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("GetValueAsync was canceled");
                    result = 1;
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.Log("GetValueAsync was failed");
                    result = 1;
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if(snapshot.Value == null)
                {
                    Debug.Log("snapshot.Value is null");
                    result = 1;
                    return;
                }

                playerStats.FromJson(snapshot.GetRawJsonValue());
                Debug.Log($"userstats data load success: {userId}, {snapshot.GetRawJsonValue()}");

            });

            //인벤토리 가져오기

            //장착아이템 가져오기

            OnChangeData?.Invoke(result);
        }

    }
}
