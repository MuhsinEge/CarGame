using System.Collections.Generic;
using ServiceLocator;
using UnityEngine;

namespace Services
{
    public class PoolService : IService
    {
        private readonly Dictionary<string, Stack<GameObject>> _poolStacks;

        public PoolService()
        {
            _poolStacks = new Dictionary<string, Stack<GameObject>>();
        }

        public GameObject TryPop(string key)
        {
            if (!_poolStacks.ContainsKey(key))
            {
                _poolStacks.Add(key, new Stack<GameObject>());
                return null;
            }

            if (_poolStacks[key].Count > 0)
            {
                return _poolStacks[key].Pop();
            }

            return null;
        }

        public void Push(string key, GameObject go)
        {
            go.SetActive(false);
            if (!_poolStacks.ContainsKey(key))
            {
                _poolStacks.Add(key, new Stack<GameObject>());
            }

            _poolStacks[key].Push(go);
        }
    }
}