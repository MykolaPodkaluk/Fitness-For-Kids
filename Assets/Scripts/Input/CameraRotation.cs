using System.Collections; 
using UnityEngine; 

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;
    [SerializeField] private float distanceToTarget = 10;
    [SerializeField] private float currentMinY = 1f;
    [SerializeField] private float minRotationX = 5f;
    [SerializeField] private float cameraStartYPosition = 0.8f;
    [SerializeField] private float minZoomDistance = 2f;
    [SerializeField] private float maxZoomDistance = 4f;
    [SerializeField] private float minYDistance = 0.5f;
    [SerializeField] private float maxYDistance = 1.5f;

    [SerializeField] private float _cameraSpeed = 0.1f;
    [SerializeField] private float _ySpeed = 0.1f;

    [SerializeField] private float _changeRotation;

    private int _zoomCounter;
    private int _YCounter;
     
    private Coroutine _zoomCoroutine; 
    private Vector3 previousPosition; 
    private Vector3 previousPositionTwoTouches;
    private float tempYDistance;
     
    public void ChangeRotation(float x, float y)
    { 
        float rotationAroundYAxis = -x * 180 * _changeRotation; // camera moves horizontally
        float rotationAroundXAxis = y * 180 * _changeRotation; // camera moves vertically

        cam.transform.position = new Vector3(target.position.x, cameraStartYPosition, target.position.z);

        // Застосовуємо обмеження для повороту по осі X
        float newRotationX = Mathf.Clamp(cam.transform.rotation.eulerAngles.x + rotationAroundXAxis, minRotationX, 90);

        cam.transform.rotation = Quaternion.Euler(newRotationX, cam.transform.rotation.eulerAngles.y + rotationAroundYAxis, 0);

        cam.transform.Translate(new Vector3(0, 0, -distanceToTarget)); 
    }

    public void ChangeZoom(float x)
    {
        if (maxZoomDistance * 10 > _zoomCounter & x == 1 || _zoomCounter > minZoomDistance * 10 & x == -1)
        {
            cam.transform.Translate(new Vector3(0, 0, -_cameraSpeed * x));
            distanceToTarget += _cameraSpeed * x;
            _zoomCounter += (int)x;
        }
    }

    public void ChangeDoubleRotation(float x)
    {
        if (_YCounter > minYDistance * 100 & x == -1 || _YCounter < maxYDistance * 100 & x == 1)
        {
            tempYDistance = _ySpeed * x;
            cameraStartYPosition += _ySpeed * x;
            _YCounter += (int)x;
        }
    }

    private void Start()
    {
        _YCounter = (int)(minYDistance * 100) + 1;
        /*
        _touchControls.Touch.SecondaryTouchContact.started += _ => ZoomStart();
        _touchControls.Touch.SecondaryTouchContact.canceled += _ => ZoomEnd();
        */
        _zoomCounter = (int)(distanceToTarget * 10f);
        ChangeRotation(1, 1);
    } 
/*
    private void Awake()
    {
        _touchControls = new TouchControls();
    }

    private void OnEnable()
    {
        _touchControls.Enable();
    }

    private void OnDisable()
    {
        _touchControls.Disable();
    }

    IEnumerator ZoomDetection()
    {
        float previousDistance = 0f;
        float distance = 0f;

        while (true)
        {
            distance = Vector2.Distance(_touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>(),
                _touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>());
            if (distance < previousDistance && _zoomCounter < maxZoomDistance * 10)
            {
                ChangeZoom(1);
            }
            //zoom in
            if (distance > previousDistance && _zoomCounter > minZoomDistance * 10)
            {
                ChangeZoom(-1);
            }

            previousDistance = distance;
            yield return null;
        }
    }
*/
    private void Update()
    {
/*
        if (Input.GetMouseButtonDown(1))
        {
            var firstTouch = cam.ScreenToViewportPoint(Input.touches[0].position);
            var secondTouch = cam.ScreenToViewportPoint(Input.touches[1].position);
            previousPositionTwoTouches = GetMedianeVector(firstTouch, secondTouch);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            previousPosition = cam.ScreenToViewportPoint(Input.touches[0].position);
        }
        else if (Input.GetMouseButton(0) && Input.touchCount == 1)
        {
            Vector3 newPosition = cam.ScreenToViewportPoint(Input.touches[0].position);
            Vector3 direction = previousPosition - newPosition;

            var directionDistance = Vector3.Distance(newPosition, previousPosition); 
            if (directionDistance < 0.2f)
            { 
                float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
                float rotationAroundXAxis = direction.y * 180; // camera moves vertically

                cam.transform.position = new Vector3(target.position.x, cameraStartYPosition, target.position.z);

                // Застосовуємо обмеження для повороту по осі X
                float newRotationX = Mathf.Clamp(cam.transform.rotation.eulerAngles.x + rotationAroundXAxis, minRotationX, 90);

                cam.transform.rotation = Quaternion.Euler(newRotationX, cam.transform.rotation.eulerAngles.y + rotationAroundYAxis, 0);

                cam.transform.Translate(new Vector3(0, 0, -distanceToTarget)); 

                previousPosition = newPosition;
            }
        }
        else if (Input.touchCount == 2)
        {
            var firstTouch = cam.ScreenToViewportPoint(Input.touches[0].position);
            var secondTouch = cam.ScreenToViewportPoint(Input.touches[1].position);
            var positionTwoTouches = GetMedianeVector(firstTouch, secondTouch);

            var temp = positionTwoTouches.y - previousPositionTwoTouches.y;
            bool isPositive = temp > 0;

            if (isPositive & _YCounter < maxYDistance * 100 & temp != 0)
            {
                tempYDistance = _ySpeed;
                cameraStartYPosition += _ySpeed;
                _YCounter++;
            }
            else if (!isPositive & _YCounter > minYDistance * 100 & temp != 0)
            {
                tempYDistance = -_ySpeed;
                cameraStartYPosition -= _ySpeed;
                _YCounter--;
            }
            previousPositionTwoTouches = positionTwoTouches;
        }
*/
        // Обмеження для положення по осі Y
        float newY = Mathf.Max(cam.transform.position.y, currentMinY) + tempYDistance;
        cam.transform.position = new Vector3(cam.transform.position.x, newY, cam.transform.position.z);
        tempYDistance = 0;
    }/*

    private Vector3 GetMedianeVector(Vector3 firstVector, Vector3 secondVector)
    {
        return new Vector3((firstVector.x + secondVector.x) / 2, (firstVector.y + secondVector.y) / 2, (firstVector.z + secondVector.z) / 2);
    }

    private void ZoomStart()
    {
        _zoomCoroutine = StartCoroutine(ZoomDetection());
    }

    private void ZoomEnd()
    {
        StopCoroutine(_zoomCoroutine);
    }*/
}