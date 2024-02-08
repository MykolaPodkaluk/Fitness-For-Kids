using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI.Extensions;
using System.Collections.Generic;

public class LineAnimator : MonoBehaviour
{
    [SerializeField] private float _animationDuration = 1f;

    private UILineRenderer _lineRenderer;
    private Vector2[] _linePoints;
    private int _pointsCount;
    public event Action OnAnimationCompleted;

    public void Init(UILineRenderer lineRenderer)
    {
        _lineRenderer = lineRenderer;

        // Store a copy of lineRenderer's points in linePoints array
        _pointsCount = _lineRenderer.Points.Length;
        _linePoints = new Vector2[_pointsCount];
        for (int i = 0; i < _pointsCount; i++)
        {
            _linePoints[i] = _lineRenderer.Points[i];
        }

        //StartCoroutine(AnimateLine());
    }

    private IEnumerator AnimateLine()
    {
        float[] segmentDurations = new float[_pointsCount - 1];

        // Calculate the length of each segment and sum them up
        float totalLength = 0f;
        for (int i = 0; i < _pointsCount - 1; i++)
        {
            float segmentLength = Vector2.Distance(_linePoints[i], _linePoints[i + 1]);
            totalLength += segmentLength;
            segmentDurations[i] = segmentLength;
        }

        // Calculate the time ratio for each segment
        for (int i = 0; i < _pointsCount - 1; i++)
        {
            segmentDurations[i] = (segmentDurations[i] / totalLength) * _animationDuration;
        }

        for (int i = 0; i < _pointsCount - 1; i++)
        {
            float startTime = Time.time;

            Vector2 startPosition = _linePoints[i];
            Vector2 endPosition = _linePoints[i + 1];

            float segmentDuration = segmentDurations[i];

            Vector2 pos = startPosition;
            List<Vector2> tempList = new List<Vector2>();
            while (pos != endPosition)
            {
                float t = (Time.time - startTime) / segmentDuration;
                pos = Vector2.Lerp(startPosition, endPosition, t);

                // animate all other points except point at index i
                for (int j = i + 1; j < _pointsCount; j++)
                    tempList.Add(pos);
                _lineRenderer.Points = tempList.ToArray();
                yield return null;
            }
        }
        OnAnimationCompleted?.Invoke();
    }
}