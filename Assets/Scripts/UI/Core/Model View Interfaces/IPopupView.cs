using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IPopupView : IView
{
    UniTask InitPopup(Camera camera, Transform parent, int orderLayer = 0);
}