using System;
using System.Collections.Generic;
using Data;
using UnityEditor;
using UnityEngine;
using Utils;

namespace LevelEditor
{
    public class GridContainer : MonoBehaviour
    {
        public Color[] carColors = new Color[8];
        public List<GridData> levels;
        public void SaveLevel(int levelIndex, GridData gridData)
        {
            if (levels == null)
            {
                levels = new List<GridData>();
            }
            if (levelIndex >= 0)
            {
                levels[levelIndex] = gridData;
            }
            else
            {
                levels.Add(gridData);
            }
            
            LevelSaveSystem.SaveLevels(levels);
        }
        
        public void DeleteLevel(int levelIndex)
        {
            if (levels == null)
            {
                return;
            }
            if (levelIndex < 0 || levelIndex > levels.Count - 1) return;

            levels.RemoveAt(levelIndex);
            LevelSaveSystem.SaveLevels(levels);
        }
        
        public int GetCurrentLevelCount()
        {
            return levels != null ? levels.Count -1 : 0;
        }
        
        public GridData GetLevelIndexData(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex > levels.Count - 1) return null;
            return levels[levelIndex];
        }

        public void OnLoad()
        {
            levels = LevelSaveSystem.LoadLevels();
        }
    }
}
