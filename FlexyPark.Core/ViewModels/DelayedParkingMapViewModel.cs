using System;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using System.Threading.Tasks;
using FlexyPark.Core.Helpers;
using FlexyPark.Core.Services;
using System.Globalization;
using System.Collections.Generic;

namespace FlexyPark.Core.ViewModels
{

	public class DelayedParkingMapViewModel : BaseViewModel
	{
		#region Constructors

		public DelayedParkingMapViewModel(IApiService apiService, ICacheService cacheService)
			: base(apiService, cacheService)
		{
		}

		#endregion

		#region Init

		public void Init(long parkingid, long startTimestamp, long endTimestamp)
		{
			ParkingURL = new Dictionary<string, string>
			{
				{"BaseParkingURL", ApiUrls.BaseWebURL},
				{"Email", "?email=" + Mvx.Resolve<ICacheService>().CurrentUser.Email},
				{"Password", Mvx.Resolve<ICacheService>().CurrentUser.Password},
				{"ParkingId", "&parking_id=" + parkingid},
				{"StartTimestamp", "&startTimestamp=" + startTimestamp},
				{"EndTimestamp", "&endTimestamp=" + endTimestamp},
				{"PlateNumber", "&plateNumber=" + mCacheService.SelectedVehicle.PlateNumber}
			};
		}
		#endregion

		#region Properties

		#region ParkingURL

		private Dictionary<string, string> mParkingURL;

		public Dictionary<string, string> ParkingURL
		{
			get
			{
				return mParkingURL;
			}
			set
			{
				mParkingURL = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#endregion

		#region Methods

		#endregion
	}
}

