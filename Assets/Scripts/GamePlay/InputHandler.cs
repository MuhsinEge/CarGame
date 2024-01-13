using System;
using UnityEngine;

namespace GamePlay
{
    public class InputHandler : MonoBehaviour
    {
        protected float HorizontalInput;
        protected bool IsInitialTouchExist;
        private float _jumpStartDelay = 0f;
        private readonly float _midScreenPoint = Screen.width / 2f;

        protected virtual void Update()
        {
            if(_jumpStartDelay > 0)
            {
                _jumpStartDelay -= Time.deltaTime;
                return;
            }
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (!IsInitialTouchExist)
                {
                    _jumpStartDelay = 0.1f;
                    IsInitialTouchExist = true;
                    return;
                }

                if (touch.position.x < _midScreenPoint)
                {
                    HorizontalInput = Mathf.Lerp(HorizontalInput, -1, Time.deltaTime * 3f);
                }
                else
                {
                    HorizontalInput = Mathf.Lerp(HorizontalInput, 1, Time.deltaTime * 3f);
                }
            }
            else
            {
                HorizontalInput = 0f;
            }
        }
    }
}