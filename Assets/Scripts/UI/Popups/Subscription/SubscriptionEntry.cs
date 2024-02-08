using TMPro;
using UnityEngine;

namespace FitnessForKids.UI.Subscription
{
    public class SubscriptionEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        public bool IsInited { get; private set; } 

        public void Init(string text)
        {
            _text.text = text;
            gameObject.SetActive(true);
            IsInited = true;
        }
    }
}