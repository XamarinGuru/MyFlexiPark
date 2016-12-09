using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using System.Collections.ObjectModel;

namespace FlexyPark.Core.ViewModels
{
    public class BuyCreditsViewModel : BaseViewModel, IInAppPurchaseManagerView
    {
        private readonly IInAppPurchaseService mInAppPurchaseService;

        #region Constructors

        public BuyCreditsViewModel(IApiService apiService, ICacheService cacheService, IInAppPurchaseService inAppPurchaseService) : base(apiService, cacheService)
        {
            this.mInAppPurchaseService = inAppPurchaseService;
        }

        #endregion

        #region Init

        public async void Init()
        {
            await Task.Delay(100);
            this.mInAppPurchaseService.View = this;
            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this,true));

            //get list product ids from server
            var rs = await mApiService.GetProductIds(Mvx.Resolve<IPlatformService>().OS == OS.Touch ? "apple" : "android");
            if(rs!=null && rs.Response != null && rs.Response.Products != null && rs.Response.Products.Count > 0)
            {
                List<string> productIds = new List<string>();
                foreach (var item in rs.Response.Products)
                {
                    productIds.Add(item.ProductId);
                }

                var productsInfo = await mInAppPurchaseService.RequestProductsInformation(productIds);
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, rs.Response != null ? rs.Response.ErrorCode : "Error"));
            }

            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this,false));
        }

        #endregion

        #region Properties

        #region CreditsValues

        private IList<int> mCreditsValues = new List<int> { 10, 25, 50, 100};

        public IList<int> CreditsValues
        {
            get
            {
                return mCreditsValues;
            }
            set
            {
                mCreditsValues = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ProductsInformation

        private ObservableCollection<Tuple<string, string, string, string, double>> mProductsInformation = new ObservableCollection<Tuple<string, string, string, string, double>>();

        public ObservableCollection<Tuple<string, string, string, string, double>> ProductsInformation
        {
            get
            {
                return mProductsInformation;
            }
            set
            {
                mProductsInformation = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region GotoDetailsCommand

        private MvxCommand<Tuple<string, string, string, string, double>> mGotoDetailsCommand = null;

        public MvxCommand<Tuple<string, string, string, string, double>> GotoDetailsCommand
        {
            get
            {
                if (mGotoDetailsCommand == null)
                {
                    mGotoDetailsCommand = new MvxCommand<Tuple<string, string, string, string, double>>(this.GotoDetails);
                }
                return mGotoDetailsCommand;
            }
        }

        private void GotoDetails(Tuple<string, string, string, string, double> productInfo)
        {
            ShowViewModel<BuyCreditsDetailsViewModel>(new 
                {
                    productId = productInfo.Item1,
                    productTitle = productInfo.Item2,
                    productDesc = productInfo.Item3,
                    price = productInfo.Item4,
                    productPrice = productInfo.Item5
                });
        }

        #endregion

        #endregion

        #region Methods

        #endregion

        #region IInAppPurchaseManagerView implementation

        public void ShowProductsInformation(List<Tuple<string, string, string, string, double>> productsInformation)
        {
            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this,false));
            ProductsInformation = new ObservableCollection<Tuple<string, string, string, string, double>>(productsInformation);
        }

        public void SendTransactionToServer(TransactionInfo transactionInfo)
        {
            //For debug pupose: you display the transaction result in a popup
        }

        public void RejectTransaction()
        {
            Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "You need the right to buy products in the store in order to buy MyflexyPark credits"));
        }


        #endregion
    }
}

