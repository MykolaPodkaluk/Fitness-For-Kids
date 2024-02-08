using Zenject;
using FitnessForKids.Services;
using FitnessForKids.UI.Subscription;
using FitnessForKids.Services.Subscription;

public class AppPurchasesContextInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        BindIApPurchaseService();
        BindSubscriptionService();
        BindSubscriptionDataAdapter();
        BindSubscriptionFreeTrialTracker();
    }

    private void BindIApPurchaseService()
    {
        Container.Bind<IIAPService>().To<IAPService>().AsSingle();
    }

    private void BindSubscriptionService()
    {
        Container.Bind<ISubscriptionService>().To<SubscriptionService>().AsSingle();
    }

    private void BindSubscriptionDataAdapter()
    {
        Container.Bind<ISubscriptionDataAdapter>().To<SubscriptionDataAdapter>().AsSingle();
    }

    private void BindSubscriptionFreeTrialTracker()
    {
    #if UNITY_ANDROID //|| UNITY_EDITOR_WIN
        Container.Bind<IFreeTrialTracker>().To<AndroidSubscriptionFreeTrial>().AsSingle();
    #else
        Container.Bind<IFreeTrialTracker>().To<SubscriptionFreeTrialStub>().AsSingle();
    #endif
    }
}