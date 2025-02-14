using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// PooledObejct를 부착한 오브젝트만 풀에 들어간다
    /// </summary>
    public class PooledObejct : MonoBehaviour
    {
        #region Variables
        private ObjectPool pool;    //지금 오브젝트가 소속 되어 있는 풀
        public ObjectPool Pool { get => pool; set => pool = value; }
        #endregion

        //오브젝트를 풀에 돌려보낸다
        public void Release()
        {
            pool.ReturnToPool(this);
        }
    }
}
