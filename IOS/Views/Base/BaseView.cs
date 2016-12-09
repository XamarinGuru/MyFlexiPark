using System;
using Foundation;
using Cirrious.MvvmCross.Touch.Views;
using UIKit;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using FlexyPark.Core.Messengers;
using BigTed;
using FlexyPark.UI.Touch.Helpers;
using FlexyPark.Core.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace FlexyPark.UI.Touch.Views
{
	public class BaseView : MvxViewController, IBaseView
	{
		public BaseView(string nibName, NSBundle bundle)
			: base(nibName, bundle)
		{
		}

		public new BaseViewModel ViewModel
		{
			get
			{
				return base.ViewModel as BaseViewModel;
			}
			set
			{
				base.ViewModel = value;
			}
		}

		private MvxSubscriptionToken mToastToken;
		private MvxSubscriptionToken mProgressToken;
		private MvxSubscriptionToken mAlertToken;
		private MvxSubscriptionToken mTextSourceToken;
		private ProgressHUD mProgressHud;
		private UIAlertView mAlertView;

		protected void HideBackButton()
		{
			NavigationItem.SetHidesBackButton(true, true);
		}

		protected void HideNavigationBar(bool flag)
		{
			NavigationController.SetNavigationBarHidden(flag, false);
		}

		protected void SetTitle(string title)
		{
			NavigationItem.Title = title;
		}

		protected void SetBackButtonTitle(string title)
		{
			NavigationItem.BackBarButtonItem = new UIBarButtonItem(title, UIBarButtonItemStyle.Plain, null);
		}

		public override void ViewWillAppear(bool animated)
		{
			ViewModel.RaisePropertyChanged("TextSource");
			ViewModel.RaisePropertyChanged("SharedTextSource");

			if (this is ParkingListsView || this is DelayedParkingMapView)
				return;

			if (!(this is ParkingMapView))
				SetTitle(ViewModel.TextSource.GetText("PageTitle"));

			if (this is AddEventView)
				SetTitle((ViewModel as AddEventViewModel).IsReadOnly ? ViewModel.TextSource.GetText("BookingTitle") : ViewModel.TextSource.GetText("UnavailabilityTitle"));
			if (this is AddSpotView)
				SetTitle((ViewModel as AddSpotViewModel).IsEditMode ? ViewModel.TextSource.GetText("EditPageTitle") : ViewModel.TextSource.GetText("PageTitle"));
			if (this is AddVehicleView)
				SetTitle((ViewModel as AddVehicleViewModel).IsEditMode ? ViewModel.TextSource.GetText("EditPageTitle") : ViewModel.TextSource.GetText("PageTitle"));

			SetBackButtonTitle(ViewModel.SharedTextSource.GetText("BackTitle"));

			base.ViewWillAppear(animated);
		}

		/*public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            #region Unsubscribe Message

            if(mAlertToken != null)
                Mvx.Resolve<IMvxMessenger>().Unsubscribe<AlertMessage>(mAlertToken);
            if(mProgressToken != null)
                Mvx.Resolve<IMvxMessenger>().Unsubscribe<ProgressMessage>(mProgressToken);
            if(mToastToken != null)
                Mvx.Resolve<IMvxMessenger>().Unsubscribe<ToastMessage>(mToastToken);
            
            #endregion
        }*/

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			if (!(this is ParkingSummaryView) && !(this is ParkingMapView) && !(this is CommonProfileView) /*&& !(this is RentProfileView)*/ && !(this is OwnProfileView))
				NavigationController.SetNavigationBarHidden(false, true);

			ViewModel.BaseView = this;

			#region Subscribe Message
			mTextSourceToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<TextSourceMessage>((TextSourceMessage message) =>
				{
					ViewModel.RaisePropertyChanged("TextSource");
					ViewModel.RaisePropertyChanged("SharedTextSource");
					SetTitle(ViewModel.TextSource.GetText("PageTitle"));
					SetBackButtonTitle(ViewModel.SharedTextSource.GetText("BackTitle"));
				});

			mToastToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<ToastMessage>((ToastMessage message) =>
				{
					if (message.Sender != ViewModel)
						return;

					if (!string.IsNullOrEmpty(message.Message))
					{
						var progressHud = new ProgressHUD() { ForceiOS6LookAndFeel = true };

						progressHud.ShowToast(message.Message, ProgressHUD.MaskType.None, ProgressHUD.ToastPosition.Bottom, 1500);

						progressHud.Dispose();
						progressHud = null;
					}

				});

			mProgressToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<ProgressMessage>(message =>
				{
					if (message.Sender != ViewModel)
						return;

					if (message.IsShow)
					{
						if (mProgressHud == null)
							mProgressHud = new ProgressHUD() { ForceiOS6LookAndFeel = true };

						mProgressHud.Show(message.Message, -1, message.IsAllowInteraction ? ProgressHUD.MaskType.None : ProgressHUD.MaskType.Clear);
					}
					else
					{
						if (mProgressHud != null)
						{
							mProgressHud.Dismiss();
							mProgressHud.Dispose();
							mProgressHud = null;
						}
					}
				});

			mAlertToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<AlertMessage>(message =>
				{
					if (message.Sender != ViewModel)
						return;

					//TODO : use UIAlertView in iOS 7.x
					if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
					{
						//  show alert view with more than 1 buttons (e.g OK and Cancel )
						if (message.OtherTitles != null && message.OtherActions != null)
						{
							if (mAlertView == null)
								mAlertView = new UIAlertView(message.Title, message.Message, null, message.CancelTitle);

							if (message.OtherTitles.Length == message.OtherActions.Length)
							{
								foreach (var title in message.OtherTitles)
								{
									mAlertView.AddButton(title);
								}
							}
							else
							{
								Console.WriteLine("Please check your message constructor");
								return;
							}

							mAlertView.Clicked += (object sender, UIButtonEventArgs e) =>
								{
									if (e.ButtonIndex != 0)
										message.OtherActions[(int)e.ButtonIndex - 1]();
									else
									{
										if (message.CancelAction != null)
											message.CancelAction();
									}

								};

							mAlertView.Dismissed += (object sender, UIButtonEventArgs e) =>
								{
									if (mAlertView != null)
									{
										mAlertView.Dispose();
										mAlertView = null;
									}
								};

							mAlertView.Show();
						}
						//this is just a normal alert view :)
						else
						{
							if (mAlertView == null)
								mAlertView = new UIAlertView(message.Title, message.Message, null, message.CancelTitle);

							mAlertView.Clicked += (object sender, UIButtonEventArgs e) =>
								{
									if (message.CancelAction != null)
										message.CancelAction();

								};

							mAlertView.Dismissed += (object sender, UIButtonEventArgs e) =>
								{
									if (mAlertView != null)
									{
										mAlertView.Dispose();
										mAlertView = null;
									}
								};

							mAlertView.Show();
						}

					}
					else
					{


						//show alert view with more than 1 buttons (e.g OK and Cancel )
						if (message.OtherTitles != null && message.OtherActions != null)
						{
							UIAlertController alertController = UIAlertController.Create(message.Title, message.Message, UIAlertControllerStyle.Alert);
							UIAlertAction cancelAction = UIAlertAction.Create(message.CancelTitle, UIAlertActionStyle.Cancel,
								action =>
								{
									if (message.CancelAction != null)
										message.CancelAction();
								});
							alertController.AddAction(cancelAction);

							if (message.OtherTitles.Length == message.OtherActions.Length)
							{
								for (int i = 0; i < message.OtherTitles.Length; i++)
								{
									UIAlertAction alertAction = UIAlertAction.Create(message.OtherTitles[i], UIAlertActionStyle.Default,
										action =>
										{
											if ((i - 1 >= 0) && (message.OtherActions[i - 1] != null))
												message.OtherActions[i - 1]();
										});
									alertController.AddAction(alertAction);
								}
							}
							else
							{
								Console.WriteLine("Please check your message constructor");
								return;
							}

							PresentViewController(alertController, true, null);
						}
						//this is just a normal alert view :)
						else
						{
							UIAlertController alertController = UIAlertController.Create(message.Title, message.Message, UIAlertControllerStyle.Alert);
							UIAlertAction cancelAction = UIAlertAction.Create(message.CancelTitle, UIAlertActionStyle.Cancel,
								action =>
								{
									if (message.CancelAction != null)
										message.CancelAction();
								});

							alertController.AddAction(cancelAction);

							PresentViewController(alertController, true, null);
						}
					}
				});
			#endregion

			#region UI Settings

			if (!DeviceHelper.IsPad)
				AdjustFontSize(this.View);

			//SetBackButtonTitle("Back");

			this.AutomaticallyAdjustsScrollViewInsets = false;

			#endregion

		}

		public void AdjustFontSize(UIView view)
		{
			foreach (var subview in view.Subviews)
			{
				if (subview.Subviews != null && subview.Subviews.Length != 0)
				{
					AdjustFontSize(subview);
				}
				else
				{
					if (subview is UIButton)
					{
						(subview as UIButton).Font = FontHelper.AdjustFontSize((subview as UIButton).Font);
					}
					else if (subview is UILabel)
					{
						(subview as UILabel).Font = FontHelper.AdjustFontSize((subview as UILabel).Font);
					}
				}
			}
		}

		#region IBaseView implementation

		public bool CheckInternetConnection()
		{
			if (Reachability.InternetConnectionStatus() == NetworkStatus.NotReachable)
				return false;
			else
				return true;
		}

		public string SHA256StringHash(String input)
		{
			SHA256 shaM = new SHA256Managed();
			// Convert the input string to a byte array and compute the hash.
			byte[] data = shaM.ComputeHash(Encoding.UTF8.GetBytes(input));
			// Create a new Stringbuilder to collect the bytes
			// and create a string.
			StringBuilder sBuilder = new StringBuilder();
			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}
			// Return the hexadecimal string.
			input = sBuilder.ToString();
			return (input);
		}

		#endregion


	}
}

