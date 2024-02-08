using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScrollItemScaler : MonoBehaviour
{
    #region FIELDS

    [Header("COMPONENTS:")]
    [SerializeField] private RectTransform snapTarget;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private List<RectTransform> elements;
    [SerializeField] private List<Image> navigation;
    [SerializeField] private Sprite activeNav;
    [SerializeField] private Sprite inactiveNav;

    [Header("CONFIG:")]
    [SerializeField] private float snapSpeed = 10f;

    private float[] distances;
    [SerializeField] private int closestElementIndex;
    public int ClosestElementIndex
    {
        get => closestElementIndex;
        private set
        {
            if (closestElementIndex != value)
            {
                closestElementIndex = value;
                OnElementSelected?.Invoke();
            }
        }
    }
    private int activeElementsAmount;
    public int ActiveElementsAmount
    {
        get => activeElementsAmount;
        set
        {
            activeElementsAmount = value;
            UpdateActiveElements();
        }
    }

    #endregion

    private int elementDistance;
    private bool isDragging = false;

    public UnityEvent OnElementSelected;

    #region MONO AND INITIALIZATION

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        int elementsCount = elements.Count;
        distances = new float[elementsCount];

        //Waiting for layout group of the elements parent to updated
        yield return new WaitForEndOfFrame();

        elementDistance = (int)Mathf.Abs(elements[1].anchoredPosition.x -
            elements[0].anchoredPosition.x);
    }

    private void LateUpdate()
    {
        // Find the closest element to the center of the snap target
        for (int i = 0; i < elements.Count; i++)
        {
            distances[i] = Mathf.Abs(snapTarget.position.x - elements[i].transform.position.x);
        }

        float minDistance = Mathf.Min(distances);
        for (int a = 0; a < elements.Count; a++)
        {
            if (minDistance == distances[a])
            {
                ClosestElementIndex = a;
            }
        }

        // Interpolate the position of the content to the target position
        if (!isDragging)
        {
            LerpToElement(ClosestElementIndex * elementDistance);
        }
        UpdateElementsScale();
    }

    private void UpdateElementsScale()
    {
        for (int i = 0; i < elements.Count; i++)
        {
            Vector3 scale = Vector3.Lerp(elements[i].localScale, 
                i == ClosestElementIndex ? Vector3.one : Vector3.one * 0.8f, snapSpeed * Time.deltaTime);
            elements[i].localScale = scale;
            navigation[i].sprite = i == ClosestElementIndex ? activeNav : inactiveNav;
        }
    }

    #endregion

    private void LerpToElement(int position)
    {
        float snapX = Mathf.Lerp(contentRect.anchoredPosition.x, -position,
            snapSpeed * Time.deltaTime);
        Vector2 snapPosition = new Vector2(snapX, contentRect.anchoredPosition.y);
        contentRect.anchoredPosition = snapPosition;
    }

    private void UpdateActiveElements()
    {
        for (int i = 0; i < activeElementsAmount; i++)
        {
            elements[i].gameObject.SetActive(true);
        }

        for (int i = activeElementsAmount; i < elements.Count; i++)
        {
            elements[i].gameObject.SetActive(false);
        }
    }

    public void StartDragging()
    {
        isDragging = true;
    }
    public void StopDragging()
    {
        isDragging = false;
    }
}
