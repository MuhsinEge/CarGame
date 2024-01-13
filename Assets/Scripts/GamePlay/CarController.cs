using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

namespace GamePlay
{
    public class CarController : InputHandler
    {
        public float speed = 5.0f;
        public float turnSpeed = 50.0f;
        public (int, int) ExitGridPosition;
        
        private readonly List<Vector3> _positions = new();
        private readonly List<Quaternion> _rotations = new();
        private bool _isFrozen = true;
        private bool _isPrerecorded;
        private int _playbackState;
        private Action _onCrashedAction;
        private Action _onReachedFinishLineAction;
        private Collider _collider;
        private MeshRenderer _meshRenderer;
        private Coroutine _playBackCoroutine;
        
        public void InitExitPosition((int, int) exitGridPosition)
        {
            ExitGridPosition = exitGridPosition;
        }
        
        public void InitActions(Action onCrashed, Action onReachedFinishLine)
        {
            _onCrashedAction = onCrashed;
            _onReachedFinishLineAction = onReachedFinishLine;
            _collider = GetComponent<Collider>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_isPrerecorded) return;
            if (other.CompareTag("Obstacle") || other.CompareTag("Car"))
            {
              _onCrashedAction?.Invoke();   
            }
            if (other.CompareTag("ExitPoint"))
            {
                _onReachedFinishLineAction?.Invoke();
            }
        }
        
        public void StartMovement(bool isPrerecorded)
        {
            _isFrozen = false;
            IsInitialTouchExist = false;
            _playbackState = 0;
            _isPrerecorded = isPrerecorded;
            _collider.enabled = true;
            if (_isPrerecorded)
            {
                _collider.isTrigger = true;
                _meshRenderer.material.color = Color.gray;
                PlayRecordedMovement();
            }
            else
            {
                _collider.isTrigger = false;
                _meshRenderer.material.color = Color.green;
                ClearRecordedMovement();
            }
            
        }

        public void RollBack(Action restart)
        {
            _isFrozen = true;
            _collider.enabled = false;
            if(_playBackCoroutine != null)
                StopCoroutine(_playBackCoroutine);
            StartCoroutine(PlayRollback(restart));
        }
        
        private void ClearRecordedMovement()
        {
            _positions.Clear();
            _rotations.Clear();
        }
        
        void FixedUpdate()
        {
            if (_isFrozen || _isPrerecorded)
            {
                return;
            }
            
            if (!IsInitialTouchExist)
            {
                return;
            }
            
            RecordMovement();
            
            float rotationAngle = HorizontalInput * turnSpeed * Time.deltaTime;
            
            transform.Rotate(0, rotationAngle, 0);
            
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        void RecordMovement()
        {
            _positions.Add(transform.position);
            _rotations.Add(transform.rotation);
            _playbackState++;
        }
        
        void PlayRecordedMovement()
        {
            if(_playBackCoroutine != null)
                StopCoroutine(_playBackCoroutine);
            _playBackCoroutine = StartCoroutine(Playback());
        }

        IEnumerator PlayRollback(Action restart)
        {
            yield return new WaitUntil(() => IsInitialTouchExist);
            for (int i = _playbackState -1; i >= 0; i--)
            {
                if (i - 3 >= 0)
                    i -= 3;
                _playbackState = i;
                transform.position = _positions[i];
                transform.rotation = _rotations[i];
                yield return null;
            }
            restart?.Invoke();
        }

        IEnumerator Playback()
        {
            transform.position = _positions[0];
            transform.rotation = _rotations[0];
            
            yield return new WaitUntil(() => IsInitialTouchExist);
            
            for (int i = 1; i < _positions.Count; i++)
            {
                _playbackState = i;
                transform.position = _positions[i];
                transform.rotation = _rotations[i];
                yield return null;
            }
        }
    }
}
