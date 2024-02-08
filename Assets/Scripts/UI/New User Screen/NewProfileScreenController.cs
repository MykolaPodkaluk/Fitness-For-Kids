public interface INewProfileScreenController
{
    INewProfileScreenView View { get; }
    INewProfileScreenModel Model { get; }
    void Init(INewProfileScreenView view, bool isFirstRegistration);
}

public class NewProfileScreenController : INewProfileScreenController
{
    private INewProfileScreenView _view;
    private INewProfileScreenModel _model;
    public INewProfileScreenView View => _view;
    public INewProfileScreenModel Model => _model;

    public void Init(INewProfileScreenView view, bool isFirstRegistration)
    {
        _model = new NewProfileScreenViewModel();
        _view = view;        
        _view.Init((NewProfileScreenViewModel)_model, isFirstRegistration);
    }
}
