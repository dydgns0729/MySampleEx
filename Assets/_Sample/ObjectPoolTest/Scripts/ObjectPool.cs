using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MySampleEx
{
    /// <summary>
    /// 오브젝트 풀: 풀 셋팅, 오브젝트 꺼내오기, 오브젝트 다시 넣기
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        #region Variables
        [SerializeField] private PooledObejct objectToPool;
        [SerializeField] private Stack<PooledObejct> stack = new Stack<PooledObejct>();
        [SerializeField] private int initPoolSize;
        #endregion

        private void Start()
        {
            //풀 셋팅
            SetupPool();
        }

        //풀 셋팅
        private void SetupPool()
        {
            if (objectToPool == null)
            {
                return;
            }

            stack = new Stack<PooledObejct>();

            //stack 넣을 오브젝트 객체 변수
            PooledObejct instance = null;

            for (int i = 0; i < initPoolSize; i++)
            {
                instance = Instantiate(objectToPool);   //생성한 풀 오브젝트
                instance.Pool = this;                   //생성한 풀 오브젝트에 풀 등록
                instance.gameObject.SetActive(false);   //생성한 풀 오브젝트 비활성화
                stack.Push(instance);                   //풀 컬렉션에 넣기
            }
        }

        //오브젝트 꺼내오기
        public PooledObejct GetPooledObejct()
        {
            if (objectToPool == null)
            {
                return null;
            }

            //풀(스택)에 더이상 오브젝트가 없으면 새로 생성한다
            if (stack.Count == 0)
            {
                PooledObejct newInstance = Instantiate(objectToPool);
                newInstance.Pool = this;
                newInstance.gameObject.SetActive(true);
                return newInstance;
            }

            PooledObejct nextInstance = stack.Pop();
            nextInstance.gameObject.SetActive(true);
            return nextInstance;
        }

        //오브젝트 다시 넣기
        public void ReturnToPool(PooledObejct pooledObejct)
        {
            stack.Push(pooledObejct);
            pooledObejct.gameObject.SetActive(false);
        }
    }
}
