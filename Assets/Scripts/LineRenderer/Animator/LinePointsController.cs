using System;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class LinePointsController : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private UILineRenderer[] _lineRenderers;
    [SerializeField] private LineAnimator[] _lineAnimators;
    [SerializeField] private Button[] _targetButtons;

    private RectTransform _target;
    private Vector3[] _linePoints;

    public event Action OnLineAnimationCompleted;

    private void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        Subscribe(true);
    }

    private void OnDisable()
    {
        Subscribe(false);
    }

    private void Init()
    {
        var lineRenderer = _lineRenderers[0];
        var pointsCount = lineRenderer.Points.Length;
        _linePoints = new Vector3[pointsCount];
        for (int i = 0; i < pointsCount; i++)
        {
            _linePoints[i] = lineRenderer.Points[i];
        }
    }

    private void Subscribe(bool isSubscribed)
    {
        if (isSubscribed)
        {
            for (int i = 0; i < _targetButtons.Length; i++)
            {
                var button = _targetButtons[i];
                button.onClick.AddListener(() =>
                SetTargetPosition((RectTransform)button.transform));
            }
            _lineAnimators[0].OnAnimationCompleted += OnAnimationCompleted;
        }
        else
        {
            for (int i = 0; i < _targetButtons.Length; i++)
            {
                var button = _targetButtons[i];
                button.onClick.RemoveListener(() =>
                SetTargetPosition((RectTransform)button.transform));
            }
            _lineAnimators[0].OnAnimationCompleted -= OnAnimationCompleted;
        }
    }

    private void OnAnimationCompleted()
    {
        OnLineAnimationCompleted?.Invoke();
    }

    public void SetTargetPosition(RectTransform target)
    {
        _target = target;
        Vector3 targetPosition = GetTargetPositionRelativeToCanvas();
        int directionX = targetPosition.x > 0 ? 1 : targetPosition.x < 0 ? -1 : 0;

        for (int i = 0; i < _lineRenderers.Length; i++)
        {
            var lineRenderer = _lineRenderers[i];
            List<Vector2> tempList = new List<Vector2>();
            for (int j = 0; j < 16; j++)
            {
                var pointPosition = _linePoints[j];
                Vector3 endPosition;

                if (directionX == 1)
                {
                    endPosition = j < 7
                        ? new Vector3(-pointPosition.x, pointPosition.y, pointPosition.z)
                        : new Vector3(-pointPosition.x + targetPosition.x + _linePoints[15].x, pointPosition.y, pointPosition.z);
                }
                else if (directionX == -1)
                {
                    endPosition = j < 7
                        ? pointPosition
                        : new Vector3(pointPosition.x + targetPosition.x - _linePoints[15].x, pointPosition.y, pointPosition.z);
                }
                else
                {
                    endPosition = j < 7
                        ? new Vector3(0, pointPosition.y, pointPosition.z)
                        : new Vector3(targetPosition.x, pointPosition.y, pointPosition.z);
                }

                tempList.Add(endPosition);
            }
            lineRenderer.Points = tempList.ToArray();

            _lineAnimators[i].Init(lineRenderer);
        }
    }

    private Vector2 GetTargetPositionRelativeToCanvas()
    {
        if (_canvas != null)
        {
            Vector3 targetWorldPosition = _target.position;
            Vector3 targetLocalPosition = _canvas.transform.InverseTransformPoint(targetWorldPosition);
            return new Vector2(targetLocalPosition.x, targetLocalPosition.y);
        }
        else
        {
            Debug.LogError("Canvas not found");
            return Vector2.zero;
        }
    }

    public void SetLinesActive(bool isActive)
    {
        foreach (var line in _lineRenderers)
        {
            line.enabled = isActive;
        }
    }
}