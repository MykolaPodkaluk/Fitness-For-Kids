using FitnessForKids.Data.Addressables;
using FitnessForKids.UI.Subscription;
using FitnessForKids.Services.UI;
using FitnessForKids.Services;
using FitnessForKids.UI;
using UnityEngine;
using Zenject;

/// <summary>
/// Installs all dependency bindings using Zenject framework
/// </summary>
public class ProjectContextInstaller : MonoInstaller
{
    #region FIELDS

    [Header("ADDRESSABLES:")]
    [SerializeField] private AddressableRefsHolder refsHolder;

    #endregion

    public override void InstallBindings()
    {
        BindBasicServices();
        BindAdsProvider();
        BindSubscriptionScreen();
        BindNewProfileScreen();
    }

    private void BindBasicServices()
    {
        Container.Bind<IAddressableRefsHolder>().FromInstance(refsHolder).AsSingle();
        Container.Bind<IUIManager>().To<UIManager>().AsSingle();
        Container.Bind<IAccountService>().To<AccountService>().AsSingle();
        Container.Bind<IAdsService>().To<AdsService>().AsSingle();
        Container.Bind<IDataService>().To<DataService>().AsSingle().NonLazy();
    }

    private void BindAdsProvider()
    {
#if UNITY_ANDROID || UNITY_EDITOR_WIN
        Container.Bind<IAdsProvider>().To<GoogleAdsProvider>().AsSingle();
#elif UNITY_IOS
        Container.Bind<IAdsProvider>().To<AdsProviderStub>().AsSingle();
#endif
    }

    private void BindSubscriptionScreen()
    {
        Container.Bind<ISubscriptionScreenMediator>().To<SubscriptionScreenMediator>().AsTransient();
        Container.Bind<ISubscriptionScreenController>().To<SubscriptionScreenController>().AsTransient();
    }

    private void BindNewProfileScreen()
    {
        Container.Bind<IProfileRegistrationMediator>().To<ProfileRegistrationMediator>().AsTransient();
        Container.Bind<INewProfileScreenController>().To<NewProfileScreenController>().AsSingle();
    }
}