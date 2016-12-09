using System;
using System.Collections.ObjectModel;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Validators;
using FluentValidation.Results;
using System.Text;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using Cirrious.MvvmCross.Plugins.JsonLocalisation;
using FlexyPark.Core.Models;
using Akavache;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Globalization;
using Cirrious.MvvmCross.Localization;


namespace FlexyPark.Core.ViewModels
{
	public class InitialMapViewModel : BaseViewModel
	{
		#region Constructors

		public InitialMapViewModel(IApiService apiService, ICacheService cacheService)
			: base(apiService, cacheService)
		{
		}

		#endregion

		#region Init

		public void Init()
		{
			Debug.WriteLine(Mvx.Resolve<IPlatformService>().GetTimeZone());
		}

		#endregion

		#region Properties

		#region ParkingSlots

		private ObservableCollection<Parking> mParkings = new ObservableCollection<Parking>();

		public ObservableCollection<Parking> Parkings
		{
			get
			{
				return mParkings;
			}
			set
			{
				mParkings = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#endregion

		#region Commands

		#region GotoSignInCommand

		private MvxCommand mGotoSignInCommand = null;

		public MvxCommand GotoSignInCommand
		{
			get
			{
				if (mGotoSignInCommand == null)
				{
					mGotoSignInCommand = new MvxCommand(this.GotoSignIn);
				}
				return mGotoSignInCommand;
			}
		}

		private void GotoSignIn()
		{
			ShowViewModel<SignInViewModel>();
		}

		#endregion

		#endregion

		#region Methods

		#region Get Parking Lists

		public async void AvaiableParkingsInMap()
		{
			if (BaseView != null && BaseView.CheckInternetConnection())
			{
				Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true, string.Empty, true));

				var gpsLatitude = AppConstants.locCenterOfBelgium[0];
				var gpsLongitude = AppConstants.locCenterOfBelgium[1];
				var maxDistance = AppConstants.MaxDistance;

				ObservableCollection<Parking> Tmp = new ObservableCollection<Parking>();

				do
				{
					var parkings = await mApiService.AvaiableParkingsInMap(gpsLatitude, gpsLongitude, maxDistance, Tmp.Count);
					if (parkings != null && parkings.Response.Count != 0)
					{
						foreach (var parking in parkings.Response)
						{
							Tmp.Add(parking);
						}
					}

					if (parkings == null || parkings.Response.Count < AppConstants.LimitPerPage)
					{
						break;
					}
				} while (true);

				if (Tmp.Count == 0)
				{
					Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "No parkings"));
				}
				Parkings = Tmp;

				Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
			}
			else
			{
				Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
			}
		}

		#endregion

		#endregion
	}
}

