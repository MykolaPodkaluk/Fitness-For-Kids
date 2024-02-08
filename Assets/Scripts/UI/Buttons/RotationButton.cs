using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace FitnessForKids.UI
{
    public class RotationButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private CameraRotation rotationCamera;
        [SerializeField] private RotationType rotationType;

        private bool mouseDown = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            mouseDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            mouseDown = false;
        }

        private void Update()
        {
            if (mouseDown)
            {
                switch (rotationType)
                {
                    case RotationType.None:
                        Debug.Log("serialize field dont take any arguments");
                        break;
                    case RotationType.Left:
                        rotationCamera.ChangeRotation(1, 0);
                        break;
                    case RotationType.Right:
                        rotationCamera.ChangeRotation(-1, 0);
                        break;
                    case RotationType.Up:
                        rotationCamera.ChangeRotation(0, 1);
                        break;
                    case RotationType.Down:
                        rotationCamera.ChangeRotation(0, -1);
                        break;
                    case RotationType.DoubleUp:
                        rotationCamera.ChangeDoubleRotation(1);
                        break;
                    case RotationType.DoubleDown:
                        rotationCamera.ChangeDoubleRotation(-1);
                        break;
                    case RotationType.ZoomIn:
                        rotationCamera.ChangeZoom(-1);
                        break;
                    case RotationType.ZoomOut:
                        rotationCamera.ChangeZoom(1);
                        break;
                }
            }
        }
    }

    public enum RotationType
    {
        None = 0,
        Right = 1,
        Left = 2,
        Up = 3,
        Down = 4,
        DoubleUp = 5,
        DoubleDown = 6,
        ZoomIn = 7,
        ZoomOut = 8,
    }
} 