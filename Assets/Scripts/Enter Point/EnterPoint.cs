using UnityEngine.SceneManagement;
using FitnessForKids.Services;
using Cysharp.Threading.Tasks;
using FitnessForKids.Data;
using FitnessForKids.UI;
using UnityEngine;
using Zenject;
using Core.Service;

namespace FitnessForKids.SceneManagement
{
    public class EnterPoint : MonoBehaviour
    {
        [Inject] private DiContainer _container;
        private IAccountService _accountService;
        private IIAPService _ipService;
        private IAdsService _adsService;
        private ISubscriptionService _subscriptionService;
        private IDataService _dataService;
        private IInputService _inputService;

        [SerializeField] private LoadingScreenView _loadingView;
        [SerializeField] private int tweenCyclesToUnload = 2;
        [SerializeField] private GameObject _debugConsole;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _dataService = _container.Resolve<IDataService>();
            _dataService.UserProfileController.OnProfilesChanged += Subscribe;
            _ = LoadMainMenuScene();
        }

        private void Subscribe(UserProfileData data)
        {
            if (data != null)
            {
                Init();
            }
        }

        private async void Init()
        {
            _ipService = _container.Resolve<IIAPService>();
            await _ipService.InitializePurchasing();
            _adsService = _container.Resolve<IAdsService>();
            _adsService.Init();
            _accountService = _container.Resolve<IAccountService>();

            _inputService = _container.Resolve<IInputService>();
            _inputService.Init();

            _subscriptionService = _container.Resolve<ISubscriptionService>();
            _subscriptionService.Init();

            await _accountService.CheckAllAsync();
        }

        private async UniTask LoadMainMenuScene()
        {
            SceneLoader.Instance.LoadScene("Home Screen GUI", SceneTransitionMode.None, LoadSceneMode.Additive);
            await UniTask.WaitUntil(() => _loadingView.LoadingTweenCounter == tweenCyclesToUnload);
            _loadingView.Hide(() => _ = UnloadEnterPointScene());
        }

        private async UniTask UnloadEnterPointScene()
        {
            await SceneManager.UnloadSceneAsync("EnterPoint");
        }
    }
}