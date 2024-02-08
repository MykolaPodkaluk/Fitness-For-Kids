using System;

public interface IPopupMediator
{
    public event Action ON_CLOSE;
    void CreatePopup(Action onComplete = null);
    void ClosePopup(Action onComplete = null);
}
