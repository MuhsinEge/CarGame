using System.Collections.Generic;
using Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Utils
{
    public class LevelSaveSystem : MonoBehaviour
    {
        public static List<GridData> LoadLevels()
        {
#if UNITY_EDITOR
            var json = System.IO.File.ReadAllText(Application.dataPath + $"/Resources/LevelData.json");
#else
            var json = Resources.Load<TextAsset>("LevelData").text;
#endif

            return JsonConvert.DeserializeObject<List<GridData>>(json);
        }

        public static void SaveLevels(List<GridData> levelGrid)
        {
            string json = JsonConvert.SerializeObject(levelGrid);
            System.IO.File.WriteAllText(Application.dataPath + $"/Resources/LevelData.json", json);
        }
    }
}