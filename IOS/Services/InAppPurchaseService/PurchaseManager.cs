using System;
using StoreKit;
using System.Collections.Generic;
using Foundation;
using System.Linq;
using FlexyPark.Core.Services;
using System.Diagnostics;
using System.Globalization;

namespace FlexyPark.UI.Touch.Services.InAppPurchaseService
{
    public class PurchaseManager : SKProductsRequestDelegate
    {
        private SKProductsRequest mProductsRequest;
        public EventHandler<SKProduct[]> OnReceivedProductInformation;
        public EventHandler<TransactionInfo> OnCompleteTransaction;
        public EventHandler OnRestoreTransaction;
        public EventHandler OnRejectTransaction;

        public PurchaseManager()
        {
        }

        public bool CanMakePayments()
        {
            return SKPaymentQueue.CanMakePayments;
        }

        public void RequestProductsInformation(List<string> productIds)
        {
            NSString[] array = productIds.Select(pId => (NSString)pId).ToArray();
            NSSet productIdentifiers = NSSet.MakeNSObjectSet<NSString>(array);

            //set up product request for in-app purchase
            mProductsRequest = new SKProductsRequest(productIdentifiers);
            mProductsRequest.Delegate = this; // SKProductsRequestDelegate.ReceivedResponse
            mProductsRequest.Start();
        }

        // received response to RequestProductData - with price,title,description info
        public override void ReceivedResponse(SKProductsRequest request, SKProductsResponse response)
        {
            SKProduct[] products = response.Products;

            if (products != null && products.Length > 0)
            {
                if (OnReceivedProductInformation != null)
                    OnReceivedProductInformation(this, products);
            }

            foreach (string invalidProductId in response.InvalidProducts)
                Console.WriteLine("Invalid product id: {0}", invalidProductId);
        }

        public void PurchaseProduct(string appStoreProductId)
        {
            Console.WriteLine("PurchaseProduct {0}", appStoreProductId);
            if (CanMakePayments())
            {
                SKPayment payment = SKPayment.PaymentWithProduct(appStoreProductId);
                SKPaymentQueue.DefaultQueue.AddPayment(payment);
            }
            else
            {
                OnRejectTransaction(this, null);
            }
        }

        public void CompleteTransaction(SKPaymentTransaction transaction)
        {
            Console.WriteLine("CompleteTransaction {0}" + transaction.TransactionIdentifier);
            string productId = transaction.Payment.ProductIdentifier;

            Debug.WriteLine("Base64 " + transaction.TransactionReceipt.GetBase64EncodedString(NSDataBase64EncodingOptions.None));

            if (!string.IsNullOrEmpty(productId) && OnCompleteTransaction != null)
            {
                var info = new TransactionInfo()
                {
                    Amount = double.Parse(productId.Replace("com.myflexypark.crowd.", "").Replace("credits", ""), CultureInfo.InvariantCulture),
                    TransactionId = transaction.TransactionIdentifier,
                    TransactionReceiptEncoded = transaction.TransactionReceipt.GetBase64EncodedString(NSDataBase64EncodingOptions.None)
                };
                OnCompleteTransaction(this, info);
            }
                
            FinishTransaction(transaction, true);
        }

        public void FailedTransaction(SKPaymentTransaction transaction)
        {
            //SKErrorPaymentCancelled == 2
//            string errorDescription = transaction.Error.Code == 2 ? "User CANCELLED FailedTransaction" : "FailedTransaction";
//            Console.WriteLine("{0} Code={1} {2}", errorDescription, transaction.Error.Code, transaction.Error.LocalizedDescription);

            FinishTransaction(transaction, false);
        }


        public void FinishTransaction(SKPaymentTransaction transaction, bool wasSuccessful)
        {
            Console.WriteLine("FinishTransaction {0}", wasSuccessful);
            // remove the transaction from the payment queue.
            SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);        // THIS IS IMPORTANT - LET'S APPLE KNOW WE'RE DONE !!!!
        }

        /// <summary>
        /// Probably could not connect to the App Store (network unavailable?)
        /// </summary>
        public override void RequestFailed(SKRequest request, NSError error)
        {
            Console.WriteLine(" ** RequestFailed ** {0}", error.LocalizedDescription);
        }

        /// <summary>
        /// Restore any transactions that occurred for this Apple ID, either on
        /// this device or any other logged in with that account.
        /// </summary>
        public void Restore()
        {
            Console.WriteLine(" ** Restore **");
            // theObserver will be notified of when the restored transactions start arriving <- AppStore
            SKPaymentQueue.DefaultQueue.RestoreCompletedTransactions();
        }

        public virtual void RestoreTransaction(SKPaymentTransaction transaction)
        {
            // Restored Transactions always have an 'original transaction' attached
            Console.WriteLine("RestoreTransaction {0}; OriginalTransaction {1}", transaction.TransactionIdentifier, transaction.OriginalTransaction.TransactionIdentifier);
            string productId = transaction.OriginalTransaction.Payment.ProductIdentifier;

            if (OnRestoreTransaction != null)
                OnRestoreTransaction(this, null);

            FinishTransaction(transaction, true);
        }
    }
}

