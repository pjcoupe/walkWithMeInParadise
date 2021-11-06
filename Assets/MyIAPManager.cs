using UnityEngine;
using UnityEngine.Purchasing;
using PaperPlaneTools;


public class MyIAPManager : MonoBehaviour, IStoreListener
{
    private static string unlockBookID = "unlock_full_book";

    private IStoreController controller;
    private IExtensionProvider extensions;
    private BookManager bookManager;

    void Start()
    {
        bookManager = gameObject.GetComponentInChildren<BookManager>();
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(unlockBookID, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void PurchaseBook()
    {
        //Debug.LogWarning("Purchasing book...");
        // If Purchasing has been initialized ...
        if (controller != null && extensions != null)
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = controller.products.WithID(unlockBookID);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                //Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                controller.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                Alert alert = new Alert("Error", "Product Not Available");
                alert.Show();
            }
        }
        // Otherwise ...
        else
        {
            Alert alert = new Alert("Error", "Failed to initialise In App Purchases. Is your internet on?");
            alert.Show();
        }
    }

    /// <summary>
    /// Called when Unity IAP is ready to make purchases.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        //Debug.LogWarning("IAP Initialised");
        this.controller = controller;
        this.extensions = extensions;
    }

    /// <summary>
    /// Called when Unity IAP encounters an unrecoverable initialization error.
    ///
    /// Note that this will not be called if Internet is unavailable; Unity IAP
    /// will attempt initialization until it becomes available.
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogWarning("IAP Initialise FAILURE "+error);
    }

    /// <summary>
    /// Called when a purchase completes.
    ///
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        //Debug.LogWarning("Purchase Processed Book Unlocked");
        PlayerPrefs.SetString("Unlocked", SystemInfo.deviceUniqueIdentifier);
        bookManager.CheckLockUnlockBook();
        
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        
        string reason = "Unknown Error";
        string title = "Unlock Purchase Failure";
        switch (p)
        {
            case PurchaseFailureReason.PurchasingUnavailable:
                reason = "Purchasing Unavailable try again later";
                break;
            case PurchaseFailureReason.ExistingPurchasePending:
                reason = "Existing purchase still pending - wait for a minute and if that doesn't work maybe try to close and reopen the app.";
                break;
            case PurchaseFailureReason.PaymentDeclined:
                reason = "Payment declined sorry - check your balance/funds/credit card details";
                break;
            case PurchaseFailureReason.ProductUnavailable:
                reason = "Product is not available";
                break;
            case PurchaseFailureReason.DuplicateTransaction:
                title = "Already previously purchased this book!";
                reason = "Duplicate Transaction - so now unlocking. Next time use restore purchases from the main menu!";
                ProcessPurchase(null);
                break;
            case PurchaseFailureReason.SignatureInvalid:
                reason = "Signature invalid";
                break;
            case PurchaseFailureReason.UserCancelled:
                reason = "User Cancelled - don't worry you weren't billed";
                break;
            case PurchaseFailureReason.Unknown:
                reason = "Unknown reason reported";
                break;
        }
        Alert a = new PaperPlaneTools.Alert(title, reason);
        Debug.LogError("Failed to purchase " + title + " " + reason);
        a.Show();
    }
}