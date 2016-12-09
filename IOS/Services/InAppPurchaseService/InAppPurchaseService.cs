using System;
using FlexyPark.Core.Services;
using System.Collections.Generic;
using StoreKit;

namespace FlexyPark.UI.Touch.Services.InAppPurchaseService
{
    public class InAppPurchaseService : InAppPurchaseServiceBase
    {
        private PurchaseManager mPurchaseManager;
        private FPPaymentObserver mPaymentObserver;

        public InAppPurchaseService(PurchaseManager purchaseManager)
        {
            mPurchaseManager = purchaseManager;
            RegisterEventHandler();
        }

        private void RegisterEventHandler()
        {
            mPurchaseManager.OnReceivedProductInformation += (object sender, StoreKit.SKProduct[] e) =>
            {
                var productsInformation = new List<Tuple<string,string,string,string, double>>();
                foreach (var product in e)
                {
                        var information = new Tuple<string,string,string,string,double>(product.ProductIdentifier, product.LocalizedTitle, product.LocalizedDescription, product.LocalizedPrice(), product.Price.DoubleValue);
                        productsInformation.Add(information);
                }

                if (View != null)
                    View.ShowProductsInformation(productsInformation);
            };

            mPurchaseManager.OnCompleteTransaction += (object sender, TransactionInfo e) =>
            {
                if (View != null)
                    View.SendTransactionToServer(e);
            };

            mPurchaseManager.OnRestoreTransaction += (object sender, EventArgs e) =>
            {
                    Console.WriteLine("Restore Completed");      
            };

            mPurchaseManager.OnRejectTransaction += (object sender, EventArgs e) =>
            {
                if (View != null)
                    View.RejectTransaction();
            };
        }

        #region implemented abstract members of InAppPurchaseServiceBase

        protected override void OnInitialize()
        {
            mPaymentObserver = new FPPaymentObserver(mPurchaseManager);

            // Call this once upon startup of in-app-purchase activities
            // This also kicks off the TransactionObserver which handles the various communications
            SKPaymentQueue.DefaultQueue.AddTransactionObserver(mPaymentObserver);
        }

        protected override async System.Threading.Tasks.Task<bool> OnRequestProductsInformation(System.Collections.Generic.List<string> productIds)
        {
            mPurchaseManager.RequestProductsInformation(productIds);
            return false;
        }

        protected override async System.Threading.Tasks.Task<bool> OnPurchaseProduct(string productId)
        {
            mPurchaseManager.PurchaseProduct(productId);
            return false;
        }

        protected override System.Threading.Tasks.Task OnRestorePreviousPurchases()
        {
            mPurchaseManager.Restore();
            return null;
        }

        #endregion
    }
}

