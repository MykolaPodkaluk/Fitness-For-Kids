using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FitnessForKids.UI.Subscription
{
    public interface IClickableTextListener
    {
        void OnTextClick(string linkID);
    }

    public class ClickableText : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _text;
        private Camera _camera;
        private IClickableTextListener _listener;

        public void Init(IClickableTextListener listener, Camera camera)
        {
            _listener = listener;
            _camera = camera;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, eventData.pressPosition, _camera);
            if (linkIndex != -1)
            {
                var linkInfo = _text.textInfo.linkInfo[linkIndex];
                var linkID = linkInfo.GetLinkID();
                _listener.OnTextClick(linkID);
            }
        }
    }
}