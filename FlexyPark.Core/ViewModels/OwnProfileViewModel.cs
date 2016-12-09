using System;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Models;
using FlexyPark.Core.Services;

namespace FlexyPark.Core.ViewModels
{
	public interface IOwnProfileView
	{
		/// <summary>
		/// Shows the date picker.
		/// </summary>
		void ShowPicker();

		void CloseKeyboard();
	}

    public class OwnProfileViewModel : BaseViewModel
    {

        #region Constructors

        public OwnProfileViewModel(IApiService apiService, ICacheService cacheService) : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public async void Init()
        {
			await Task.Delay(500);
			GetBankAccountInformations();
        }

        #endregion

		public IOwnProfileView View { get; set; }

        #region Properties

		#region IsHaveCreditCard
		private bool mIsHaveCreditCard = false;

		public bool IsHaveCreditCard
		{
			get
			{
				return mIsHaveCreditCard;
			}
			set
			{
				mIsHaveCreditCard = value;
				RaisePropertyChanged();
			}
		}
		#endregion

        #region BankAccount

        private string mBankAccount = string.Empty;

        public string BankAccount
        {
            get
            {
                return mBankAccount;
            }
            set
            {
                mBankAccount = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region AccountName

        private string mAccountName = string.Empty;

        public string AccountName
        {
            get
            {
                return mAccountName;
            }
            set
            {
                mAccountName = value;
                RaisePropertyChanged();
            }
        }

        #endregion

		 #region User

        /// <summary>
        /// Temp user information.
        /// </summary>
        private User mUser = new User();

		public User User
		{
			get
			{
				return mUser;
			}
			set
			{
				mUser = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		//#region Birthday

		//private string mBirthday;

		//public string Birthday
		//{
		//	get
		//	{
		//		return mBirthday;
		//	}
		//	set
		//	{
		//		mBirthday = value;
		//		RaisePropertyChanged();
		//	}
		//}

		//#endregion

		#endregion

		#region Commands

		#region AddOrReplaceCardCommand
		private MvxCommand mAddOrReplaceBankCommand = null;

		public MvxCommand AddOrReplaceBankCommand
		{
			get
			{
				if (mAddOrReplaceBankCommand == null)
				{
					mAddOrReplaceBankCommand = new MvxCommand(this.AddOrReplaceBank);
				}
				return mAddOrReplaceBankCommand;
			}
		}

		private async void AddOrReplaceBank()
		{
			if (View != null)
				View.CloseKeyboard();

			if (BaseView != null && BaseView.CheckInternetConnection())
			{
				Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

				var token = await StripeServiceForBank.GetCardToken(AccountName, BankAccount);
				if (!string.IsNullOrEmpty(token))
				{
					var result = await mApiService.AddBank(mCacheService.CurrentUser.UserId, token);
					if (result != null && result.Response != null && result.Response.Status == "success")
					{
						Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Success"));
						Close(this);
					}
					else
					{
						Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, string.Format("Error {0}", result.ApiError.Error)));
					}
				}
				else
				{
					Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Failed when get card token with Stripe"));
				}

				Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));

				if (IsHaveCreditCard)
					GetBankAccountInformations();
			}
			else
			{
				Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
			}

		}
		#endregion

		#region PickBirthdayCommand

		private MvxCommand mPickBirthdayCommand = null;

		public MvxCommand PickBirthdayCommand
		{
			get
			{
				if (mPickBirthdayCommand == null)
				{
					mPickBirthdayCommand = new MvxCommand(this.PickBirthday);
				}
				return mPickBirthdayCommand;
			}
		}

		private void PickBirthday()
		{
			if (View != null)
				View.ShowPicker();
		}

		#endregion

		#region UpdateCommand

		private MvxCommand mUpdateCommand = null;

		public MvxCommand UpdateCommand
		{
			get
			{
				if (mUpdateCommand == null)
				{
					mUpdateCommand = new MvxCommand(this.Update);
				}
				return mUpdateCommand;
			}
		}

		private async void Update()
		{
			if (View != null)
				View.CloseKeyboard();

			if (BaseView != null && BaseView.CheckInternetConnection())
			{
				Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

				var result = await mApiService.PersonPut(mCacheService.CurrentUser, User.DateOfBirth, User.Street, User.City, User.PostalCode.ToString(), User.Country);
				if (result != null)
				{
					if (result.Response.Status.Equals("success"))
					{
						Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Status));
					}
					else {
						Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result.Response.Status, result.Response.ErrorCode), "Ok", null));
					}

				}
				Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
				Close(this);
			}
			else
			{
				Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
			}
		}

		#endregion

		#endregion

		#region Methods

		#region GetCardInformationsFromAPI
		private async void GetBankAccountInformations()
		{
			Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
			var result = await mApiService.PersonGetBank(mCacheService.CurrentUser.UserId);
			if (result != null && result.Response != null)
			{
				IsHaveCreditCard = result.Response.BankAccount.Name != null;
				if (IsHaveCreditCard)
				{
					BankAccount = string.Format("**** **** **** **** **** {0}", result.Response.BankAccount.Last4);
					AccountName = result.Response.BankAccount.Name;
				}
			}

			Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
		}
		#endregion

        #endregion
    }
}

