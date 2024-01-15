using System.Collections.Generic;
using Data;
using Newtonsoft.Json;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace ServiceLocator
{
    public class Launcher : MonoBehaviour
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            Locator.Initialize();
            Application.targetFrameRate = 60;
            Locator.instance.Register(new PoolService());
            Locator.instance.Register(new GamePlayService(LevelSaveSystem.LoadLevels()));
            SceneManager.LoadSceneAsync(1);
        }
    }
}