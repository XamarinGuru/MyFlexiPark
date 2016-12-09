using System;
using StoreKit;

namespace FlexyPark.UI.Touch.Services.InAppPurchaseService
{
    public class FPPaymentObserver : SKPaymentTransactionObserver
    {
        private PurchaseManager mPurchaseManger;

        public FPPaymentObserver(PurchaseManager purchaseManager)
        {
            mPurchaseManger = purchaseManager;
        }

        #region implemented abstract members of SKPaymentTransactionObserver

        public override void UpdatedTransactions(SKPaymentQueue queue, SKPaymentTransaction[] transactions)
        {
            Console.WriteLine("UpdatedTransactions");
            foreach (SKPaymentTransaction transaction in transactions)
            {
                switch (transaction.TransactionState)
                {
                    case SKPaymentTransactionState.Purchased:
                        mPurchaseManger.CompleteTransaction(transaction);
                        break;
                    case SKPaymentTransactionState.Failed:
                        mPurchaseManger.FailedTransaction(transaction);
                        break;
                    case SKPaymentTransactionState.Restored:
                        mPurchaseManger.RestoreTransaction(transaction);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion
    }
}

