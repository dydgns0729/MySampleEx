using Firebase.Auth;
using Firebase.Database;
using System;
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

        //저장, 불러오기
        private string UserDataPath => "users";
        private string StatsDataPath => "stats";
        private string InventoryDataPath => "inventory";
        private string EquipmentDataPath => "equipment";

        public Action<int> OnChangedData;
        #endregion

        private void OnEnable()
        {
            playerStats.OnChagnedStats += OnChangedStatsInfo;

        }

        private void OnDisable()
        {
            playerStats.OnChagnedStats -= OnChangedStatsInfo;
        }

        private void Start()
        {
            //참조
            databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        }


        //스텟정보 변경시 호출되는 함수
        public void OnChangedStatsInfo(StatsObject statsObject = null)
        {
            OnSavePlayerStats();
        }

        //저장하기
        public async void OnSavePlayerStats()
        {
            int result = 0;

            var userId = FirebaseAuthManager.Instance.UserId;
            if (userId == string.Empty)
            {
                Debug.LogError("userId 값이 들어오지 않음");
                return;
            }

            //저장할 데이터
            string statsJson = playerStats.ToJson();
            await databaseRef.Child(UserDataPath).Child(userId).Child(StatsDataPath).SetRawJsonValueAsync(statsJson).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SetRawJsonValueAsync was canceled");
                    result = 1;
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SetRawJsonValueAsync was failed");
                    result = 1;
                    return;
                }
                Debug.Log($"UserStats data save success : {userId}, {statsJson}");
            });
            OnChangedData?.Invoke(result);
        }

        //불러오기
        public async void OnLoad()
        {
            int result = 0;

            var userId = FirebaseAuthManager.Instance.UserId;
            if (userId == string.Empty)
            {
                Debug.LogError("userId 값이 들어오지 않음");
                return;
            }

            //스텟 가져오기
            await databaseRef.Child(UserDataPath).Child(userId).Child(StatsDataPath).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("GetValueAsync was canceled");
                    result = 1;
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("GetValueAsync was failed");
                    result = 1;
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if (snapshot.Value == null)
                {
                    Debug.LogError("snapshot.Value is null");
                    result = 1;
                    return;
                }

                playerStats.FromJson(snapshot.GetRawJsonValue());
                Debug.Log($"UserStats data load success : {userId}, {snapshot.GetRawJsonValue()}");
            });

            //인벤토리 가져오기

            //장착아이템 가져오기

            OnChangedData?.Invoke(result);
        }
    }
}
