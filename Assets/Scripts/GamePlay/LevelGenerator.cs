using System;
using System.Collections.Generic;
using Data;
using ServiceLocator;
using Services;
using UnityEngine;

namespace GamePlay
{
    public class LevelGenerator : MonoBehaviour
    {
        public GameObject obstaclePrefab;
        public GameObject carPrefab;

        private GameObject _obstacleParent;
        private GameObject _carParent;

        private Stack<GameObject> _obstacles;
        private Queue<CarController> _cars;
        private PoolService _poolService;
        private GamePlayService _gamePlayService;
        private void Awake()
        {
            _poolService = Locator.instance.Get<PoolService>();
            _gamePlayService = Locator.instance.Get<GamePlayService>();
            _gamePlayService.OnLevelComplete += ReloadLevel;
            
            _obstacles = new Stack<GameObject>();
            _cars = new Queue<CarController>();

            _obstacleParent = new GameObject("Obstacles");
            _carParent = new GameObject("Cars");
            LoadLevel();
        }
        
        private void LoadLevel()
        {
            var gridData = _gamePlayService.GetLevelData();
            for (int col = 0; col < gridData.GridElements.Count; col++)
            {
                for (int row = 0; row < gridData.GridElements[col].Count; row++)
                {
                    if (gridData.GridElements[col][row].Type == GridEnum.Obstacle)
                    {
                        var obstacle = _poolService.TryPop("Obstacle");
                        if (obstacle == null)
                        {
                            obstacle = Instantiate(obstaclePrefab, _obstacleParent.transform, true);
                            obstacle.transform.position = new Vector3(row + 0.5f, 0f, col + 0.5f);
                        }
                        else
                        {
                            obstacle.transform.position = new Vector3(row + 0.5f, 0f, col + 0.5f);
                            obstacle.SetActive(true);
                        }
                        _obstacles.Push(obstacle);
                    }
                     
                }
            }

            for (int i = 0; i < gridData.Cars.Count; i++)
            {
                var car = _poolService.TryPop("Car");
                if (car == null)
                {
                    car = Instantiate(carPrefab, _carParent.transform, true);
                    car.transform.position = new Vector3(gridData.Cars[i].Enter.Item2 + 0.5f, 0f, gridData.Cars[i].Enter.Item1 + 0.5f);
                    car.transform.rotation = Quaternion.Euler(0f, 90f * (int)gridData.Cars[i].Direction, 0f);
                }
                else
                {
                    car.transform.position = new Vector3(gridData.Cars[i].Enter.Item2 + 0.5f, 0f, gridData.Cars[i].Enter.Item1 + 0.5f);
                    car.transform.rotation = Quaternion.Euler(0f, 90f * (int)gridData.Cars[i].Direction, 0f);
                }
                var carController = car.GetComponent<CarController>();
                carController.InitExitPosition((gridData.Cars[i].Exit.Item2, gridData.Cars[i].Exit.Item1));
                _cars.Enqueue(carController);
            }
            _gamePlayService.OnLevelLoaded?.Invoke(this, _cars);
        }

        private void ReloadLevel(object sender, EventArgs e)
        {
            while (_obstacles.Count > 0)
            {
                if (_obstacles.TryPop(out var obstacle))
                {
                    _poolService.Push("Obstacle", obstacle);
                }
            }

            while (_cars.Count > 0)
            {
                if (_cars.TryDequeue(out var car))
                {
                    _poolService.Push("Car", car.gameObject);
                }
            }
            LoadLevel();
        }
    }
}