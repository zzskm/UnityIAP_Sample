using UnityEngine;
using UnityEngine.Purchasing;

public class BillingManager : MonoBehaviour // MonoSingleton<BillingManager>
{
    private bool _isInitialized = false;
    private UnityPurchaser _purchaser = null;

    public void Initialize()
    {
        if (_purchaser == null)
        {
            _purchaser = new UnityPurchaser
            {
                handleInitialize = OnPurchaseInitialize,

                handleVerify = OnRequestVerify,

                handleSucceeded = OnPurchaseSucceeded,
                handleFailed = OnPurchaseFailed
            };

        }

        if (!_purchaser.IsInitialized())
        {
            RequestProducts(); // send request online
        }
    }

    private void RequestProducts()
    {
        ReponseProducts();
    }

    private void ReponseProducts()
    {
        _purchaser.Initialize(new string[0]); // forced initialized
    }

    private void OnPurchaseInitialize(bool isInitialized, string reason)
    {
        _isInitialized = isInitialized;
    }

    private void OnRequestVerify(Product product, PurchaseEventArgs e)
    {
        // receipt verifying
    }

    private void OnPurchaseSucceeded(string productId)
    {

    }

    private void OnPurchaseFailed(string productId, string reason)
    {

    }

    public void PurchaseProduct(string productId, System.Action<bool> callback = null)
    {
        
    }
}
