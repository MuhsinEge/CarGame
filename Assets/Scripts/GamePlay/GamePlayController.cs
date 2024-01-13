using System.Collections.Generic;
using System.Linq;
using ServiceLocator;
using Services;
using UnityEngine;

namespace GamePlay
{
    public class GamePlayController : MonoBehaviour
    {
        [SerializeField] private GameObject exitPoint;
        private GameObject _exitPoint = null;
        private GamePlayService _gamePlayService;
        private Queue<CarController> _inactiveCars;
        private Stack<CarController> _activeCars;
        private int _carsOnFinishLine;
        private int _resetCounter;
        private void Awake()
        {
            _gamePlayService = Locator.instance.Get<GamePlayService>();
            _gamePlayService.OnLevelLoaded += OnLevelLoaded;
            if (_exitPoint == null)
            {
                _exitPoint = Instantiate(exitPoint);
            }
        }

        private void OnLevelLoaded(object sender, Queue<CarController> cars)
        {
            _activeCars = new Stack<CarController>();
            _inactiveCars = new Queue<CarController>(cars);
            _carsOnFinishLine = 0;

            foreach (var car in _inactiveCars)
            {
                car.InitActions(ResetCars, OnCarReachedFinishLine);
            }
            StartGame();
        }

        private void StartGame()
        {
            while (_activeCars.Count < _carsOnFinishLine + 1)
            {
                var car = _inactiveCars.Dequeue();
                car.gameObject.SetActive(true);
                _activeCars.Push(car);
            }

            int counter = 0;
            foreach (var car in _activeCars)
            {
                car.StartMovement(counter != 0);
                counter++;
            }
            _exitPoint.transform.position = new Vector3(_activeCars.ElementAt(0).ExitGridPosition.Item1 + 0.5f, 0f, _activeCars.ElementAt(0).ExitGridPosition.Item2 + 0.5f);
        }

        private void ResetCars()
        {
            _resetCounter = 0;
            foreach (var car in _activeCars)
            {
                car.RollBack(ResetGameState);
            }
        }

        private void ResetGameState()
        {
            _resetCounter++;
            if (_resetCounter == _activeCars.Count)
            {
                StartGame();
            }
        }
        
        private void OnCarReachedFinishLine()
        {
            _carsOnFinishLine++;
            if (_carsOnFinishLine == 8)
            {
                _gamePlayService.LevelComplete();
                return;
            }
            ResetCars();
        }
    }
}