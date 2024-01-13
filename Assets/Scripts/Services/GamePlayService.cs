using System;
using System.Collections.Generic;
using Data;
using GamePlay;
using ServiceLocator;
using UnityEngine;
using Utils;

namespace Services
{
    public class GamePlayService : IService
    {
        private int _level;
        private readonly List<GridData> _levels;
        public EventHandler OnLevelComplete;
        public EventHandler<Queue<CarController>> OnLevelLoaded;

        public GamePlayService(List<GridData> levels)
        {
            _levels = levels;
        }
        
        public GridData GetLevelData()
        {
            if (_level > _levels.Count - 1)
            {
                _level = 0;
            }
            return _levels[_level];
        }
        
        public void LevelComplete()
        {
            _level++;
            OnLevelComplete?.Invoke(this, null);
        }
    }
}