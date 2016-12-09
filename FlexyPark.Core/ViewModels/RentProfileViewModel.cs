
using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Services;
using System.Text;
using FluentValidation.Validators;
using FluentValidation.Results;
using Cirrious.CrossCore;
using FlexyPark.Core.Messengers;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Collections.Generic;
using FlexyPark.Core.Validators;
using Akavache;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FlexyPark.Core.ViewModels
{
	public interface IRentProfileView
	{
		/// <summary>
		/// Shows the date picker.
		/// </summary>
		void ShowPicker();

		/// <summary>
		/// Shows the credit picker.
		/// </summary>
		void ShowCreditPicker();

		void CloseKeyboard();
	}

	public class RentProfileViewModel : BaseViewModel
	{
		#region Constructors

		public RentProfileViewModel(IApiService apiService, ICacheService cacheService) : base(apiService, cacheService)
		{
		}

		#endregion

		#region Init

		public async void Init()
		{
			SelectedCredit = CreditsValues[0];

			await Task.Delay(500);
			GetCardInformationsFromAPI();
		}

		#endregion

		public IRentProfileView View { get; set; }

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

		#region Cryptogram

		private string mCryptogram = string.Empty;

		public string Cryptogram
		{
			get
			{
				return mCryptogram;
			}
			set
			{
				mCryptogram = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region HolderName

		private string mHolderName = string.Empty;

		public string HolderName
		{
			get
			{
				return mHolderName;
			}
			set
			{
				mHolderName = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region Number

		private string mNumber = string.Empty;

		public string Number
		{
			get
			{
				return mNumber;
			}
			set
			{
				
				if (value.Length <= 16)
				{
					mNumber = value;

					StringBuilder sb = new StringBuilder();

					if (IsHaveCreditCard)
					{
						for (int i = 0; i < mNumber.Length; i++)
						{
							if (i >= 12)
							{
								sb.Insert(i, mNumber[i]);
							}
							else {
								sb.Insert(i, "*");
							}
						}
						mNumber = sb.ToString();
					}

					sb = new StringBuilder(RealCardNumber);
					if (value.Length == RealCardNumber.Length)
					{

					}
					else if (value.Length > RealCardNumber.Length)
					{
						sb.Append(value.Substring(value.Length - 1));
					}
					else {
						sb.Remove(RealCardNumber.Length - 1, 1);
					}

					RealCardNumber = sb.ToString();
				}

				RaisePropertyChanged();
			}
		}

		#endregion

		#region RealCardNumber

		private string mRealCardNumber = string.Empty;

		public string RealCardNumber
		{
			get
			{
				return mRealCardNumber;
			}
			set
			{
				if (value.Length <= 16)
				{
					mRealCardNumber = value;
				}

				RaisePropertyChanged();
			}
		}

		#endregion

		#region ValidityTime

		private DateTime mValidityTime = DateTime.UtcNow.ToLocalTime();

		public DateTime ValidityTime
		{
			get
			{
				return mValidityTime;
			}
			set
			{
				mValidityTime = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region CreditsValues

		private IList<int> mCreditsValues = new List<int> { 5, 10, 25, 50 };

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

		#region SelectedCredit

		private int mSelectedCredit = 0;

		public int SelectedCredit
		{
			get
			{
				return mSelectedCredit;
			}
			set
			{
				mSelectedCredit = value;
				RaisePropertyChanged();

				SelectedCreditStr = string.Format("{0} €", mSelectedCredit);
			}
		}

		#endregion

		#region SelectedCreditStr

		private string mSelectedCreditStr = string.Empty;

		public string SelectedCreditStr
		{
			get
			{
				return mSelectedCreditStr;
			}
			set
			{
				mSelectedCreditStr = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#endregion

		#region Commands

		#region PickValidityCommand

		private MvxCommand mPickValidityCommand = null;

		public MvxCommand PickValidityCommand
		{
			get
			{
				if (mPickValidityCommand == null)
				{
					mPickValidityCommand = new MvxCommand(this.PickValidity);
				}
				return mPickValidityCommand;
			}
		}

		private void PickValidity()
		{
			if (View != null)
				View.ShowPicker();
		}

		#endregion

		#region CreditPickCommand

		private MvxCommand mCreditPickCommand = null;

		public MvxCommand CreditPickCommand
		{
			get
			{
				if (mCreditPickCommand == null)
				{
					mCreditPickCommand = new MvxCommand(this.CreditPick);
				}
				return mCreditPickCommand;
			}
		}

		private void CreditPick()
		{
			if (View != null)
				View.ShowCreditPicker();
		}

		#endregion

		#region GotoChooseVehicleCommand

		private MvxCommand mGotoChooseVehicleCommand = null;

		public MvxCommand GotoChooseVehicleCommand
		{
			get
			{
				if (mGotoChooseVehicleCommand == null)
				{
					mGotoChooseVehicleCommand = new MvxCommand(this.GotoChooseVehicle);
				}
				return mGotoChooseVehicleCommand;
			}
		}

		private void GotoChooseVehicle()
		{
			ShowViewModel<ChooseVehicleViewModel>(new { mode = ChooseVehicleMode.NoAction });
		}

		#endregion

		#region AddOrRemoveCardCommand
		private MvxCommand mAddOrRemoveCardCommand = null;

		public MvxCommand AddOrRemoveCardCommand
		{
			get
			{
				if (mAddOrRemoveCardCommand == null)
				{
					mAddOrRemoveCardCommand = new MvxCommand(this.AddOrRemoveCard);
				}
				return mAddOrRemoveCardCommand;
			}
		}

		private async void AddOrRemoveCard()
		{
			if (View != null)
				View.CloseKeyboard();

			if (BaseView != null && BaseView.CheckInternetConnection())
			{
				Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
				if (IsHaveCreditCard)
				{
					Mvx.Resolve<IMvxMessenger>().Publish(
						new AlertMessage(this, string.Empty, "Please confirm you want to remove your payment card from our server", "No", () => { }, new string[] { "Yes" }, async () =>
						{
							Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
							var result = await mApiService.RemoveCard(mCacheService.CurrentUser.UserId);
							if (result)
							{
								Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result ? "Success" : "Error"));
								Close(this);
							}

							Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
						}));
				}
				else
				{
					var token = await Mvx.Resolve<IStripeService>().GetCardToken(HolderName, RealCardNumber, ValidityTime.Month, ValidityTime.Year % 100, Cryptogram);
					if (!string.IsNullOrEmpty(token))
					{
						var result = await mApiService.AddCard(mCacheService.CurrentUser.UserId, token);
						if (result != null && result.Response != null)
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
				}

				Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
			}
			else
			{
				Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
			}

		}
		#endregion

		#region BuyNowCommand

		private MvxCommand mBuyNowCommand = null;

		public MvxCommand BuyNowCommand
		{
			get
			{
				if (mBuyNowCommand == null)
				{
					mBuyNowCommand = new MvxCommand(this.BuyNow);
				}
				return mBuyNowCommand;
			}
		}

		private async void BuyNow()
		{
			Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

			//validate
			var toValidate = new string[] { RealCardNumber, HolderName, Cryptogram, SelectedCredit.ToString() };
			CustomCreditCardValidator validator = new CustomCreditCardValidator();
			ValidationResult results = validator.Validate(toValidate);

			if (results.IsValid)
			{

				SaveCardInformations();

				//get card token from Stripe
				var token = await Mvx.Resolve<IStripeService>().GetCardToken(HolderName, RealCardNumber, ValidityTime.Month, ValidityTime.Year % 100, Cryptogram);
				if (!string.IsNullOrEmpty(token))
				{
					//call API
					var result = await mApiService.UpdateUserCreditsViaPayment(mCacheService.CurrentUser.UserId, token, (double)SelectedCredit);
					if (result != null && result.Response != null)
					{
						Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result.Response.Status, result.Response.RemainingCredits), "Ok", null));
					}
					else {
						Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result.Response.Status, result.Response.ErrorCode), "Ok", null));
					}
				}
				else {
					Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Failed when get card token with Stripe"));
				}
			}
			else {
				Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, results.Errors[0].ToString().Trim()));
			}

			Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));

		}

		#endregion

		#endregion

		#region Methods

		#region SaveCardInformations
		public void SaveCardInformations()
		{
			BlobCache.UserAccount.InsertObject<DateTime>(AppConstants.ValidityTime, ValidityTime);
			BlobCache.UserAccount.InsertObject<string>(AppConstants.CCCardNumber, RealCardNumber);
			BlobCache.UserAccount.InsertObject<string>(AppConstants.CCHolderName, HolderName);
			BlobCache.UserAccount.InsertObject<string>(AppConstants.CCCryptogram, Cryptogram);
		}
		#endregion

		#region GetCardInformationsFromAPI
		public async void GetCardInformationsFromAPI()
		{
			Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
			var result = await mApiService.PersonGetCard(mCacheService.CurrentUser.UserId);
			if (result != null && result.Response != null)
			{
				IsHaveCreditCard = result.Response.Card.Name != null;
				if (IsHaveCreditCard)
				{
					Number = string.Format("••••••••••••{0}", result.Response.Card.Last4);
					HolderName = result.Response.Card.Name;
					Cryptogram = "•••";
					ValidityTime = new DateTime(result.Response.Card.ExpYear, result.Response.Card.ExpMonth, 1).ToLocalTime();
				}
			}

			Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
		}
		#endregion

		#region GetCardInformations
		public async void GetCardInformations()
		{
			try
			{
				ValidityTime = await BlobCache.UserAccount.GetObject<DateTime>(AppConstants.ValidityTime);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
			}
			try
			{
				RealCardNumber = await BlobCache.UserAccount.GetObject<string>(AppConstants.CCCardNumber);
				Number = RealCardNumber;
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
			}

			try
			{
				HolderName = await BlobCache.UserAccount.GetObject<string>(AppConstants.CCHolderName);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
			}

			try
			{
				Cryptogram = await BlobCache.UserAccount.GetObject<string>(AppConstants.CCCryptogram);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
			}
		}
		#endregion

		#endregion
	}
}

