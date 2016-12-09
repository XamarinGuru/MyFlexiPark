using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexyPark.Core.Services
{
    public interface IInAppPurchaseService
    {
        IInAppPurchaseManagerView View { get; set; }

        Task<bool> RequestProductsInformation(List<string> productIds);

        Task<bool> PurchaseProduct(string productId);

        Task RestorePreviousPurchases();

        void Initialize();
    }

    public abstract class InAppPurchaseServiceBase : IInAppPurchaseService
    {
        #region View

        public IInAppPurchaseManagerView View { get; set; }

        #endregion

        #region IInAppPurchaseService implementation

        public void Initialize()
        {
            OnInitialize();
        }

        protected abstract void OnInitialize();

        public async Task<bool> RequestProductsInformation(List<string> productIds)
        {
            return await OnRequestProductsInformation(productIds);
        }

        protected abstract Task<bool> OnRequestProductsInformation(List<string> productIds);

        public async Task<bool> PurchaseProduct(string productId)
        {
            return await OnPurchaseProduct(productId);
        }

        protected abstract Task<bool> OnPurchaseProduct(string productId);

        public async Task RestorePreviousPurchases()
        {
            await OnRestorePreviousPurchases();
        }

        protected abstract Task OnRestorePreviousPurchases();

        #endregion

    }

    public interface IInAppPurchaseManagerView
    {
        void ShowProductsInformation(List<Tuple<string,string,string,string,double>> productsInformation);
        void SendTransactionToServer(TransactionInfo transactionInfo);
        void RejectTransaction();
    }

    public class TransactionInfo
    {
        public string TransactionId { get; set; }
        public string SubscriptionType { get; set; }
        public DateTimeOffset PurchasedDate {get;set;}
        public DateTimeOffset ExpiredDate {get;set;}
        public double Amount {get;set;}

        public string TransactionReceiptEncoded { get; set;}

        public override string ToString()
        {
            return string.Format("[TransactionInfo: TransactionId={0}, SubscriptionType={1}, PurchasedDate={2}, ExpiredDate={3}]", TransactionId, SubscriptionType, PurchasedDate, ExpiredDate);
        }
    }
}

