using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FlexyPark.Core.Services;

namespace FlexyPark.UI.Droid.Services
{
    class InAppPurchaseService : IInAppPurchaseService
    {
        public IInAppPurchaseManagerView View { get; set; }
        public async Task<bool> RequestProductsInformation(List<string> productIds)
        {
            return false;
        }

        public async Task<bool> PurchaseProduct(string productId)
        {
            return false;
        }

        public async Task RestorePreviousPurchases()
        {
        }

        public void Initialize()
        {
        }
    }
}