using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SnapScrollPicker : MonoBehaviour
{
    #region FIELDS

    [Header("COMPONENTS:")]
    [SerializeField] private RectTransform snapTarget;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private List<DatePickerElement> elements;
    [SerializeField] private Button scrollPrev;
    [SerializeField] private Button scrollNext;

    [Header("REFERENCES:")]
    [SerializeField] private DatePickerElement elementPrefab;
    [SerializeField] private GameObject placeholderPrefab;

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
    private Vector2 scrollPosition;
    public Vector2 ScrollPosition
    {
        get => scrollPosition;
        set
        {
            scrollPosition = value;
            contentRect.anchoredPosition = Vector2.zero;
            var pos = new Vector2(0f, Mathf.Sin(Time.time * 10f) * 100f);
            contentRect.localPosition = pos;
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
        //scrollPrev.onClick.AddListener(ScrollToPrevElement);
        //scrollNext.onClick.AddListener(ScrollToNextElement);

        int elementsCount = elements.Count;
        distances = new float[elementsCount];

        //Waiting for layout group of the elements parent to updated
        yield return new WaitForEndOfFrame();

        elementDistance = (int)Mathf.Abs(elements[1].AnchoredPosition.y -
            elements[0].AnchoredPosition.y);
    }

    public void InitializeElements(List<int> elementsValues)
    {
        var content = contentRect.transform;
        contentRect.transform.DestroyChildren();
        elements = new List<DatePickerElement>();

        Instantiate(placeholderPrefab, content);
        foreach (int value in elementsValues)
        {
            DatePickerElement element = Instantiate(elementPrefab, content);
            element.Value = value;
            elements.Add(element);
        }
        Instantiate(placeholderPrefab, content);
    }

    private void LateUpdate()
    {
        // Find the closest element to the center of the snap target
        for (int i = 0; i < elements.Count; i++)
        {
            distances[i] = Mathf.Abs(snapTarget.position.y - elements[i].transform.position.y);
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
    }

    #endregion

    private void LerpToElement(int position)
    {
        float snapY = Mathf.Lerp(contentRect.anchoredPosition.y, position,
            snapSpeed * Time.deltaTime);
        Vector2 snapPosition = new Vector2(contentRect.anchoredPosition.x, snapY);
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

    public void ScrollToPrevElement()
    {
        isDragging = true;
        var snapY = ClosestElementIndex - 1 * elementDistance;
        Vector2 snapPosition = new Vector2(contentRect.anchoredPosition.x, snapY);
        contentRect.anchoredPosition = snapPosition;
        isDragging = false;
        Debug.Log("Prev");
    }

    public void ScrollToNextElement()
    {
        isDragging = true;
        var snapY = ClosestElementIndex + 1 * elementDistance;
        Vector2 snapPosition = new Vector2(contentRect.anchoredPosition.x, snapY);
        contentRect.anchoredPosition = snapPosition;
        isDragging = false;
        Debug.Log("Next");
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