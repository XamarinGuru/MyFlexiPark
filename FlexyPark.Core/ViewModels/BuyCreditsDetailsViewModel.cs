using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FlexyPark.Core.ViewModels
{
    public class BuyCreditsDetailsViewModel : BaseViewModel, IInAppPurchaseManagerView
    {
        private readonly IInAppPurchaseService mInAppPurchaseService;

        #region Constructors

        public BuyCreditsDetailsViewModel(IApiService apiService, ICacheService cacheService, IInAppPurchaseService inAppPurchaseService) : base(apiService, cacheService)
        {
            this.mInAppPurchaseService = inAppPurchaseService;
        }

        #endregion

        #region Init

        public void Init(string productId, string productTitle, string productDesc, string price, double productPrice)
        {
            this.mInAppPurchaseService.View = this;
            ProductId = productId;
            DisplayName = "Title: " + productTitle;
            Description = "Description: " + productDesc;
            Price = "Price: " + price;

            ProductPrice = productPrice;
        }

        #endregion

        #region Properties

        #region ProductId

        private string mProductId = string.Empty;

        public string ProductId
        {
            get
            {
                return mProductId;
            }
            set
            {
                mProductId = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Price

        private string mPrice = string.Empty;

        public string Price
        {
            get
            {
                return mPrice;
            }
            set
            {
                mPrice = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region DisplayName

        private string mDisplayName = string.Empty;

        public string DisplayName
        {
            get
            {
                return mDisplayName;
            }
            set
            {
                mDisplayName = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Description

        private string mDescription = string.Empty;

        public string Description
        {
            get
            {
                return mDescription;
            }
            set
            {
                mDescription = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ProductPrice

        private double mProductPrice = 0;

        public double ProductPrice
        {
            get{
                return mProductPrice;
            }
            set{
                mProductPrice = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region BuyCreditsCommand

        private MvxCommand mBuyCreditsCommand = null;

        public MvxCommand BuyCreditsCommand
        {
            get
            {
                if (mBuyCreditsCommand == null)
                {
                    mBuyCreditsCommand = new MvxCommand(this.BuyCredits);
                }
                return mBuyCreditsCommand;
            }
        }

        private void BuyCredits()
        {
            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this,true));
            mInAppPurchaseService.PurchaseProduct(ProductId);
            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this,false));
        }

        #endregion

        #endregion

        #region Methods

        #endregion

        #region IInAppPurchaseManagerView implementation

        public void ShowProductsInformation(List<Tuple<string, string, string, string,double>> productsInformation)
        {

        }

        public async void SendTransactionToServer(TransactionInfo transactionInfo)
        {
            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this,false));

            //For debug pupose: you display the transaction result in a popup
            Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, transactionInfo.ToString()));


            if(Mvx.Resolve<IPlatformService>().OS == OS.Touch)
            {
                /*var rs = await mApiService.AppleVerifyReceipt(transactionInfo.TransactionReceiptEncoded);
                if(rs != null && rs.Receipt != null)
                {
                    var result = await mApiService.UpdateUserCredits(mCacheService.CurrentUser.UserId, "apple", ProductPrice, rs.Receipt);
                    if(result != null && result.Response != null)
                    {
                        mCacheService.CurrentUser.RemainingCredits = result.Response.RemainingCredits;
                        Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Update User Credits success"));
                    }
                }*/

                var result = await mApiService.UpdateUserCredits(mCacheService.CurrentUser.UserId, "apple", transactionInfo.Amount, transactionInfo.TransactionReceiptEncoded);
                if(result != null && result.Response != null)
                {
                    mCacheService.CurrentUser.RemainingCredits = result.Response.RemainingCredits;
                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Update User Credits success"));
                }
                else
                {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Error));
                }
            }
        }

        public void RejectTransaction()
        {
            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this,false));

            Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "You need the right to buy products in the store in order to buy MyflexyPark credits"));
        }


        #endregion
    }
}

