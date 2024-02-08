using UnityEngine.Purchasing.Extension;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Purchasing;
using UnityEngine;
using System;

#if RECEIPT_VALIDATION
using UnityEngine.Purchasing.Security;
#endif

namespace FitnessForKids.Services
{
    /// <summary>
    /// Interface for the in-app purchase service.
    /// </summary>
    public interface IIAPService
    {
        event PurchaseCallback ON_PURCHASE_CALLBACK;

        bool IsInited { get; }

        /// <summary>
        /// Initializes the in-app purchase service.
        /// </summary>
        UniTask InitializePurchasing();

        /// <summary>
        /// Restores the previous purchases, necessary for AppStore only
        /// </summary>
        /// <param name="callback">The callback to invoke when the restore operation completes.</param>
        void RestorePurchases(Action<bool> callback);

        /// <summary>
        /// Retrieves the product information for a given purchase product type.
        /// </summary>
        Product GetProduct(PurchaseProductType product);

        /// <summary>
        /// Initiates the purchase of a product.
        /// </summary>
        /// <param name="productType">The purchase product type to buy.</param>
        void BuyProduct(PurchaseProductType productType);

        /// <summary>
        /// Gets the introductory price information for Apple subscriptions.
        /// </summary>
        /// <returns>A dictionary containing the introductory price information.</returns>
        Dictionary<string, string> GetAppleIntroductoryPriceDictionary();
    }


    /// <summary>
    /// Implementation of the in-app purchase service.
    /// </summary>
    public class IAPService : IDetailedStoreListener, IIAPService
    {
        public event PurchaseCallback ON_PURCHASE_CALLBACK;

        // Constants for product IDs
        private const string kGoogleNoAdsProductId = "fivesysdev_sport_noads";
        private const string kGoogleSubscriptionYearProductId = "fivesysdev_kidsfitness_subscription_year";
        private const string kGoogleSubscriptionSeasonProductId = "fivesysdev_sport_subscription_season";
        private const string kAppleSubscriptionYearProductId = "five.systems.development.sport.year.sub";
        private const string kAppleSubscriptionSeasonProductId = "five.systems.development.sport.season.sub";

        private IStoreController _storeController;
        private IExtensionProvider _storeExtensionProvider;
        private IAppleExtensions _appleExtensions;
        private Dictionary<PurchaseProductType, Product> _products = new();
        private UniTaskCompletionSource _tcs;

        private bool IsPurchasingInited()
        {
#if UNITY_EDITOR_WIN
            if (InternetConnectionChecker.IsConnected())
            {
                return _storeController != null && _storeExtensionProvider != null;
            }
            else
            {
                return false;
            }
#else
            return _storeController != null && _storeExtensionProvider != null;
#endif

        }

        public bool IsInited => IsPurchasingInited();


        /// <summary>
        /// Initializes the in-app purchase service.
        /// </summary>
        /// <returns>A UniTask representing the completion of the initialization.</returns>
        public async UniTask InitializePurchasing()
        {
            if (IsPurchasingInited())
            {
                return;
            }

            _tcs = new UniTaskCompletionSource();

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
#if UNITY_IOS
            builder.AddProduct(kAppleSubscriptionYearProductId, ProductType.Subscription);
            builder.AddProduct(kAppleSubscriptionSeasonProductId, ProductType.Subscription);
#else
            builder.AddProduct(kGoogleNoAdsProductId, ProductType.NonConsumable);
            builder.AddProduct(kGoogleSubscriptionYearProductId, ProductType.Subscription);
            builder.AddProduct(kGoogleSubscriptionSeasonProductId, ProductType.Subscription);
#endif

            UnityPurchasing.Initialize(this, builder);

            await _tcs.Task;
        }

        /// <summary>
        /// Restores the previous purchases, necessary for AppStore only.
        /// </summary>
        /// <param name="callback">The callback to invoke when the restore operation completes.</param>
        public void RestorePurchases(Action<bool> callback)
        {
            _appleExtensions.RestoreTransactions((success, message) => {
                if (success)
                {
                    Debug.Log("Restore purchases succeeded!" + message);
                    callback?.Invoke(true);
                }
                else
                {
                    Debug.Log("Restore purchases failed!" + message);
                    callback?.Invoke(false);
                }
            });
        }

        /// <summary>
        /// Retrieves the product information for a given purchase product type.
        /// </summary>
        /// <param name="product">The purchase product type.</param>
        public Product GetProduct(PurchaseProductType product)
        {
            if (!IsPurchasingInited() || !_products.ContainsKey(product))
            {
                return default;
            }
            return _products[product];
        }

        /// <summary>
        /// Initiates the purchase of a product.
        /// </summary>
        /// <param name="productType">The purchase product type to buy.</param>
        public void BuyProduct(PurchaseProductType productType)
        {
            if (IsPurchasingInited())
            {
                var product = _products[productType];
                Debug.LogFormat("Product id called: {0}", product.definition.id);

                if (product != null && product.availableToPurchase)
                {
                    _storeController.InitiatePurchase(product);
                }
                else
                {
                    Debug.LogErrorFormat("Purchase of {0} failed", product.definition.id);
                }
            }
            else
            {
                Debug.LogErrorFormat("Purchase failed, service not initialized!");
            }
        }

        /// <summary>
        /// Gets the introductory price information for Apple subscriptions.
        /// </summary>
        /// <returns>A dictionary containing the introductory price information.</returns>
        public Dictionary<string, string> GetAppleIntroductoryPriceDictionary()
        {
            if (!IsPurchasingInited())
            {
                return default;
            }
            return _appleExtensions.GetIntroductoryPriceDictionary();
        }

        // Implementation of IDetailedStoreListener
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _storeExtensionProvider = extensions;
            _appleExtensions = extensions.GetExtension<IAppleExtensions>();

            foreach (var product in controller.products.all)
            {
                if (!product.availableToPurchase)
                {
                    Debug.LogWarning($"Product not available for purchase {product.definition.id}");
                }
            }

            OnCompleteInitialization(controller.products.all);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            _tcs.TrySetResult();
            Debug.LogError("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            _tcs.TrySetResult();
            Debug.LogError("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            ON_PURCHASE_CALLBACK?.Invoke(false, product);
            Debug.LogErrorFormat("Purchase Failed for {0} because of {1}", product, failureReason);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            ON_PURCHASE_CALLBACK?.Invoke(false, product);
            Debug.LogErrorFormat("Purchase Failed for {0} because of {1}", product, failureDescription.message);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            var validPurchase = true;
#if !UNITY_EDITOR && RECEIPT_VALIDATION
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            try
            {
                var result = validator.Validate(args.purchasedProduct.receipt);
                Debug.Log("Receipt valid");
            }
            catch (IAPSecurityException)
            {
                Debug.LogError("Validation failed");
                Debug.Log("Invalid receipt, not unlocking content");

                validPurchase = false;
            }
#endif
            if (validPurchase)
            {
                ON_PURCHASE_CALLBACK?.Invoke(true, args.purchasedProduct);
                Debug.LogWarningFormat("Purchase process for {0} is valid", args.purchasedProduct);
            }
            else
            {
                ON_PURCHASE_CALLBACK?.Invoke(false, args.purchasedProduct);
                Debug.LogWarningFormat("Purchase process for {0} is not valid", args.purchasedProduct);
            }

            return PurchaseProcessingResult.Complete;
        }

        private void OnCompleteInitialization(Product[] allProducts)
        {
            for (int i = 0, j = allProducts.Length; i < j; i++)
            {
                if (!allProducts[i].availableToPurchase)
                {
                    Debug.LogWarning($"Product not available for purchase {allProducts[i].definition.id}");
                }

                var product = allProducts[i];
                if (!_products.ContainsValue(product))
                {
                    MapProducts(product.definition.id, product);
                }
            }
            //Debug.Log("Purchases Service initialized");
            _tcs.TrySetResult();
        }

        private void MapProducts(string id, Product product)
        {
            switch (id)
            {
                case kAppleSubscriptionYearProductId:
                    _products.Add(PurchaseProductType.SubscriptionYear, product);
                    break;
                case kAppleSubscriptionSeasonProductId:
                    _products.Add(PurchaseProductType.SubscriptionSeason, product);
                    break;

                case kGoogleSubscriptionYearProductId:
                    _products.Add(PurchaseProductType.SubscriptionYear, product);
                    break;
                case kGoogleSubscriptionSeasonProductId:
                    _products.Add(PurchaseProductType.SubscriptionSeason, product);
                    break;

                case kGoogleNoAdsProductId:
                    _products.Add(PurchaseProductType.NoAds, product);
                    break;

                default:
                    Debug.LogWarningFormat("Product {0} with id: {1} - was not added to products, because of target platform", product.definition.id, id);
                    break;
            }
        }
    }

    /// <summary>
    /// Delegate for purchase callbacks.
    /// </summary>
    /// <param name="success">True if the purchase was successful; otherwise, false.</param>
    /// <param name="product">The purchased product.</param>
    public delegate void PurchaseCallback(bool success, Product product);
}