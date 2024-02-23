using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Core.Service
{
    internal class Timer : MonoBehaviour
    {
        private ITimerObserver _observer;
        private bool _isInited;

        public void Init(ITimerObserver observer)
        {
            _observer = observer;
            _isInited = true;
        }


        private void Update()
        {
            if (_isInited)
            {
                _observer.OnUpdate();
            }
        }
    }
}

