using UnityEngine;
using UnityEngine.UI;

namespace Core.Service
{
    public interface IInputService
    {
        void Init();
        void OnClick();
        void ChangeTimeBetweenClicks(float time);
    }

    internal interface ITimerObserver
    {
        void OnUpdate();
    }

    public class InputService : IInputService, ITimerObserver
    {
        private const string kInputProxy = "InputProxy";
        private const string kSortingLayer = "InputDetector";
        private const string kBlocker = "Blocker";

        private GameObject _blocker;
        private Timer _timer;
        private float _timeBetweenClicks = 0.5f;
        private ReactiveTime _time;
        private bool _isInited;
        private bool _canClick;

        public void Init()
        {
            GameObject holder = new GameObject(kInputProxy);
            var rectTransform = holder.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            var canvas = holder.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.sortingLayerName = kSortingLayer; 
            var scaler = holder.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            holder.AddComponent<GraphicRaycaster>();

            GameObject blockerGO = new GameObject(kBlocker);
            blockerGO.transform.SetParent(holder.transform);
            var image = blockerGO.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0);
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.anchoredPosition = Vector2.zero;
            blockerGO.SetActive(false);

            _time = new ReactiveTime();
            _time.Subscribe(OnTimeChanged);
            _timer = holder.AddComponent<Timer>();
            _timer.Init(this);
            _blocker = blockerGO;

            Input.multiTouchEnabled = false;
            GameObject.DontDestroyOnLoad(holder);

            _canClick = true;
            _isInited = true;
        }

        public void ChangeTimeBetweenClicks(float time)
        {
            time = Mathf.Clamp(time, 0, Mathf.Infinity);
            _timeBetweenClicks = time;
        }


        public void OnClick()
        {
            if (!_isInited)
            {
                return;
            }

            if (!_canClick)
            {
                return;
            }

            _time.Value = _timeBetweenClicks;
        }

        public void OnUpdate()
        {
            CheckIsTimeUp();
        }


        private bool CheckIsTimeUp()
        {
            float currentValue = _time.Value;
            if (currentValue == 0)
            {
                return true;
            }

            currentValue -= Time.deltaTime;
            currentValue = currentValue > 0
                ? currentValue
                : 0;
            _time.Value = currentValue;
            return false;
        }

        private void OnTimeChanged(float time)
        {
            if (time == 0)
            {
                _blocker.SetActive(false);
                _canClick = true;
            }
            else if (time == _timeBetweenClicks)
            {
                _canClick = false;
                _blocker.SetActive(true);
            }
        }
    }
}

