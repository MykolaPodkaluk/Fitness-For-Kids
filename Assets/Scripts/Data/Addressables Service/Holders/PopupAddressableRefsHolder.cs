using UnityEngine;

namespace FitnessForKids.Data.Addressables
{
    public interface IPopupAddressableRefsHolder
    {
        IPopupsAddressableRefProvider Main { get; }
    }

    [CreateAssetMenu(fileName = "PopupRefsHolder", menuName = "ScriptableObjects/AddressableRefsHolders/Popups")]
    public class PopupAddressableRefsHolder : ScriptableObject, IPopupAddressableRefsHolder
    {
        [SerializeField] private PopupsAddressableRef _popupsProvider;
        public IPopupsAddressableRefProvider Main => _popupsProvider;
    }
}