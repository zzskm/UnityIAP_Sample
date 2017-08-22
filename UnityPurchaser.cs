using UnityEngine;
using UnityEngine.Purchasing;

public class UnityPurchaser : IStoreListener
{
    private bool _isInitialized = false;

    private IStoreController _controller = null;
    private IExtensionProvider _provider = null;

    //private IAppleExtensions _appleExtensions;

    private bool _isInProgress = false;

    public System.Action<bool, string> handleInitialize = null;
    public System.Action<Product, PurchaseEventArgs> handleVerify = null;
    public System.Action<string> handleSucceeded = null; // product id
    public System.Action<string, string> handleFailed = null; // product id, reason

    public bool IsInitialized() { return _isInitialized; }
    public bool IsInProgress() { return _isInProgress; }

    public void Initialize(string[] products)
    {
        if (_isInitialized)
        {
            if (handleInitialize != null) handleInitialize(true, "Initialized");

            return;
        }

        StandardPurchasingModule module = StandardPurchasingModule.Instance();
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);

        // add products
        //builder.AddProduct(id, ProductType.Consumable, )

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _controller = controller;
        _provider = extensions;

        //_appleExtensions = extensions.GetExtension<IAppleExtensions>();
        //_appleExtensions.RegisterPurchaseDeferredListener(new Action<Product>(OnDeferred));

        _isInitialized = true;

        if (handleInitialize != null)
        {
            handleInitialize(_isInitialized, "Initialized");
        }
    }

    public void OnInitializeFailed(InitializationFailureReason reason)
    {
        Debug.LogError(GetType().Name + " OnInitializeFailed() : " + reason);

        _isInitialized = false;

        if (handleInitialize != null)
        {
            handleInitialize(_isInitialized, reason.ToString());
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        RequestVerify(e.purchasedProduct, e); // verify receipt
        return PurchaseProcessingResult.Pending;
    }

    public void RequestVerify(Product product, PurchaseEventArgs e)
    {
        if (handleVerify != null)
        {
            handleVerify(product, e);
        }
        else
        {
            // forced succeed
            OnPurchaseSucceeded(product);
        }
    }

    public void OnPurchaseSucceeded(Product product)
    {
        _controller.ConfirmPendingPurchase(product); // consume
        _isInProgress = false;

        if (handleSucceeded != null)
        {
            handleSucceeded(product.definition.id);
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log(GetType().Name + " OnInitializeFailed() : " + product.definition.storeSpecificId + " " + reason);

        _isInProgress = false;

        if (handleFailed != null)
        {
            handleFailed(product.definition.id, reason.ToString());
        }
    }

    public Product[] GetProducts()
    {
        return _isInitialized ? _controller.products.all : null;
    }

    private bool IsValidProduct(Product product)
    {
        if (product == null)
        {
            Debug.Log(GetType().Name + " IsValidProduct() : product id cannot be found " + product.definition.id);
            return false;
        }

        if (!product.availableToPurchase)
        {
            Debug.LogError(GetType().Name + " IsValidProduct() : product is not available " + product.definition.id);
            return false;
        }

        return true;
    }

    public void PurchaseProduct(string productId)
    {
        if (!_isInitialized) return;
        if (_isInProgress) return;

        if (string.IsNullOrEmpty(productId))
        {
            Debug.Log(GetType().Name + " PurchaseProduct() : product id cannot be null");
            return;
        }

        Product product = _controller.products.WithID(productId);
        if (!IsValidProduct(product)) return;

        _isInProgress = true;
        _controller.InitiatePurchase(product, "" /* custom payload data */);
    }
}
