using System.Collections.Generic;
using Data;
using Newtonsoft.Json;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ServiceLocator
{
    public class Launcher : MonoBehaviour
    {
        public TextAsset LevelData;

        private void Awake()
        {
            Locator.Initialize();
            Application.targetFrameRate = 60;
            Locator.instance.Register(new PoolService());
            Locator.instance.Register(new GamePlayService(JsonConvert.DeserializeObject<List<GridData>>(LevelData.text)));
            SceneManager.LoadSceneAsync(1);
        }
    }
}