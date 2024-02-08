using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace FitnessForKids.UI
{
    public interface IIndicatorsController
    {
        void Init(int count, int selectedIndex = 0);
        void ChangeIndicator(int index);
    }

    public class IndicatorsController : MonoBehaviour, IIndicatorsController
    {
        [SerializeField] private Image[] _indicatorImages;
        [SerializeField] private HorizontalLayoutGroup _layoutGroup;
        [SerializeField] private Sprite[] _stateSprites;
        private int _activeIndicatorsCount = 0;

        public async void Init(int count, int selectedIndex = 0)
        {
            _layoutGroup.enabled = true;
            for (int i = 0, j = _indicatorImages.Length; i < j; i++)
            {
                var indicator = _indicatorImages[i];

                var isEnable = i < count;
                indicator.gameObject.SetActive(isEnable);
                if (isEnable)
                {
                    _activeIndicatorsCount++;
                }

                indicator.sprite = selectedIndex == i
                    ? _stateSprites[0]
                    : _stateSprites[1];
            }
            await UniTask.DelayFrame(1);
            _layoutGroup.enabled = false;
        }

        public void SwitchIndicator(int x, int y)
        {
            ChangeIndicator(x);
        }

        public void ChangeIndicator(int index)
        {
            if (index < _activeIndicatorsCount)
            {
                for (int i = 0; i < _activeIndicatorsCount; i++)
                {
                    var indicator = _indicatorImages[i];
                    indicator.sprite = index == i
                        ? _stateSprites[0]
                        : _stateSprites[1];
                }
            }
        }
    }
}