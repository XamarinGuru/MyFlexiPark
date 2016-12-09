using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Droid.Fragging;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.ViewModels;


namespace FlexyPark.UI.Droid.Activity
{
    public class BaseView : MvxFragmentActivity, IBaseView, View.IOnTouchListener
    {
        private MvxSubscriptionToken mToastToken;
        private IMvxMessenger mMessenger;
        private MvxSubscriptionToken mProgressToken;
        private MvxSubscriptionToken mAlertToken;
        private AlertDialog.Builder builder;
        private AlertDialog dialog;
        private Dialog mProgressDialog;

        #region Overrides

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            (ViewModel as BaseViewModel).BaseView = this;
            mToastToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<ToastMessage>((ToastMessage message) =>
            {
                if (message.Sender != ViewModel) return;

                if (!string.IsNullOrEmpty(message.Message))
                {
                    try
                    {
                        Toast.MakeText(this, message.Message, ToastLength.Short).Show();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            });

            mProgressToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<ProgressMessage>(message =>
            {
                try
                {
                    if (message.Sender != ViewModel) return;
                    if (message.IsShow)
                    {
                        if (mProgressDialog != null)
                        {
                            mProgressDialog.Dismiss();
                            mProgressDialog = null;
                        }

                        if (!string.IsNullOrEmpty(message.Message))
                        {
                            mProgressDialog = ProgressDialog.Show(this, null, message.Message, true, true);
                            mProgressDialog.SetCancelable(false);
                            mProgressDialog.SetCanceledOnTouchOutside(false);
                        }
                        else
                        {
                            mProgressDialog = new Dialog(this);
                            mProgressDialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
                            mProgressDialog.SetContentView(Resource.Layout.CustomProgressDialog);
                            mProgressDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                            mProgressDialog.SetCancelable(false);
                            mProgressDialog.SetCanceledOnTouchOutside(false);
                            mProgressDialog.Show();
                        }
                    }
                    else
                    {
                        if (mProgressDialog != null)
                        {
                            mProgressDialog.Dismiss();
                            mProgressDialog = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            });

            mAlertToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<AlertMessage>(message =>
            {
                try
                {
                    if (message.Sender != ViewModel) return;
                    //show alert view with more than 1 buttons (e.g OK and Cancel )
                    if (message.OtherTitles != null && message.OtherActions != null)
                    {
                        // if (message.OtherTitles.Count() >= 2)
                        {
                            Dialog mDialog = new Dialog(this);
                            mDialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                            mDialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
                            mDialog.SetContentView(Resource.Layout.CustomDialog);
                            mDialog.SetCancelable(false);
                            TextView Title = mDialog.FindViewById<TextView>(Resource.Id.title);
                            TextView Message = mDialog.FindViewById<TextView>(Resource.Id.message);
                            if (!string.IsNullOrEmpty(message.Title))
                            {
                                Title.Text = message.Title;
                            }
                            if (!string.IsNullOrEmpty(message.Message))
                            {
                                Message.Text = message.Message;
                            }

                            TextView option1 = mDialog.FindViewById<TextView>(Resource.Id.option1);
                            TextView option2 = mDialog.FindViewById<TextView>(Resource.Id.option2);
                            TextView option3 = mDialog.FindViewById<TextView>(Resource.Id.option3);
                            TextView option4 = mDialog.FindViewById<TextView>(Resource.Id.option4);
                            TextView option5 = mDialog.FindViewById<TextView>(Resource.Id.option5);
                            TextView option6 = mDialog.FindViewById<TextView>(Resource.Id.option6);
                            TextView option7 = mDialog.FindViewById<TextView>(Resource.Id.option7);
                            TextView option8 = mDialog.FindViewById<TextView>(Resource.Id.option8);
                            TextView optionCancel = mDialog.FindViewById<TextView>(Resource.Id.optionCancel);

                            View view1 = mDialog.FindViewById<View>(Resource.Id.view1);
                            View view2 = mDialog.FindViewById<View>(Resource.Id.view2);
                            View view3 = mDialog.FindViewById<View>(Resource.Id.view3);
                            View view4 = mDialog.FindViewById<View>(Resource.Id.view4);
                            View view5 = mDialog.FindViewById<View>(Resource.Id.view5);
                            View view6 = mDialog.FindViewById<View>(Resource.Id.view6);
                            View view7 = mDialog.FindViewById<View>(Resource.Id.view7);
                            View view8 = mDialog.FindViewById<View>(Resource.Id.view8);
                            View viewCancel = mDialog.FindViewById<View>(Resource.Id.viewCancel);

                            if (!string.IsNullOrEmpty(message.CancelTitle))
                            {
                                optionCancel.Text = message.CancelTitle;
                                optionCancel.Visibility = ViewStates.Visible;
                                viewCancel.Visibility = ViewStates.Visible;
                                optionCancel.Click += (sender, args) =>
                                {
                                    if (message.CancelAction != null)
                                    {
                                        message.CancelAction();
                                    }

                                    if (mDialog != null)
                                    {
                                        mDialog.Dismiss();
                                    }
                                };
                            }

                            for (int i = 0; i < message.OtherTitles.Length; i++)
                            {
                                int index = i;
                                switch (index)
                                {
                                    case 0:
                                        view1.Visibility = ViewStates.Visible;
                                        option1.Visibility = ViewStates.Visible;
                                        option1.Text = message.OtherTitles[index].ToString();
                                        option1.Click += (sender, args) =>
                                        {
                                            if (message.OtherActions[index] != null)
                                            {
                                                message.OtherActions[index]();
                                            }
                                            mDialog.Dismiss();
                                        };
                                        break;

                                    case 1:
                                        view2.Visibility = ViewStates.Visible;
                                        option2.Visibility = ViewStates.Visible;
                                        option2.Text = message.OtherTitles[index].ToString();
                                        option2.Click += (sender, args) =>
                                        {
                                            if (message.OtherActions[index] != null)
                                            {
                                                message.OtherActions[index]();
                                            }
                                            mDialog.Dismiss();
                                        };
                                        break;

                                    case 2:
                                        view3.Visibility = ViewStates.Visible;
                                        option3.Visibility = ViewStates.Visible;
                                        option3.Text = message.OtherTitles[index].ToString();
                                        option3.Click += (sender, args) =>
                                        {
                                            if (message.OtherActions[index] != null)
                                            {
                                                message.OtherActions[index]();
                                            }
                                            mDialog.Dismiss();
                                        };
                                        break;
                                    case 3:
                                        view4.Visibility = ViewStates.Visible;
                                        option4.Visibility = ViewStates.Visible;
                                        option4.Text = message.OtherTitles[index].ToString();
                                        option4.Click += (sender, args) =>
                                        {
                                            if (message.OtherActions[index] != null)
                                            {
                                                message.OtherActions[index]();
                                            }
                                            mDialog.Dismiss();
                                        };
                                        break;
                                    case 4:
                                        view5.Visibility = ViewStates.Visible;
                                        option5.Visibility = ViewStates.Visible;
                                        option5.Text = message.OtherTitles[index].ToString();
                                        option5.Click += (sender, args) =>
                                        {
                                            if (message.OtherActions[index] != null)
                                            {
                                                message.OtherActions[index]();
                                            }
                                            mDialog.Dismiss();
                                        };
                                        break;

                                    case 5:
                                        view6.Visibility = ViewStates.Visible;
                                        option6.Visibility = ViewStates.Visible;
                                        option6.Text = message.OtherTitles[index].ToString();
                                        option6.Click += (sender, args) =>
                                        {
                                            if (message.OtherActions[index] != null)
                                            {
                                                message.OtherActions[index]();
                                            }
                                            mDialog.Dismiss();
                                        };
                                        break;

                                    case 6:
                                        view7.Visibility = ViewStates.Visible;
                                        option7.Visibility = ViewStates.Visible;
                                        option7.Text = message.OtherTitles[index].ToString();
                                        option7.Click += (sender, args) =>
                                        {
                                            if (message.OtherActions[index] != null)
                                            {
                                                message.OtherActions[index]();
                                            }
                                            mDialog.Dismiss();
                                        };
                                        break;

                                    case 7:
                                        view8.Visibility = ViewStates.Visible;
                                        option8.Visibility = ViewStates.Visible;
                                        option8.Text = message.OtherTitles[index].ToString();
                                        option8.Click += (sender, args) =>
                                        {
                                            if (message.OtherActions[index] != null)
                                            {
                                                message.OtherActions[index]();
                                            }
                                            mDialog.Dismiss();
                                        };
                                        break;
                                }
                            }

                            mDialog.Show();
                        }

                    }
                    // this is just a normal alert view :)
                    else
                    {
                        try
                        {
                            if (dialog != null)
                            {
                                dialog.Dismiss();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        builder = new AlertDialog.Builder(this);
                        dialog = builder.Create();
                        dialog.SetTitle(message.Title);
                        dialog.SetMessage(message.Message);
                        dialog.SetCancelable(false);
                        dialog.SetButton(message.CancelTitle, (s, ev) =>
                        {
                            if (message.CancelAction != null)
                                message.CancelAction();
                            dialog.Dismiss();
                        });

                        dialog.Show();
                    }
                }
                catch (Exception ex)
                {
                }
            });

        }

        protected override void OnResume()
        {
            base.OnResume();
            (ViewModel as BaseViewModel).RaisePropertyChanged("TextSource");
        }
        
        #endregion

        #region Implemments

        public bool OnTouch(View v, MotionEvent e)
        {
            HideKeyboard();
            return false;
        }

        #endregion

        #region Method


        public bool CheckInternetConnection()
        {
            ConnectivityManager connectivityManager
              = (ConnectivityManager)GetSystemService(Context.ConnectivityService);
            NetworkInfo activeNetworkInfo = connectivityManager.ActiveNetworkInfo;
            return activeNetworkInfo != null && activeNetworkInfo.IsConnected;
        }

        protected void SetButtonEffects(List<int> viewIds)
        {
            foreach (var viewId in viewIds)
            {
                var view = FindViewById(viewId);
                if (view != null)
                {
                    view.Touch += (sender, args) =>
                    {
                        args.Handled = false;

                        switch (args.Event.Action)
                        {
                            case MotionEventActions.Down:
                                var alphaAnim = new AlphaAnimation(1F, 0.5F) { Duration = 0, FillAfter = true };
                                (sender as View).StartAnimation(alphaAnim);
                                break;

                            case MotionEventActions.Up:
                                alphaAnim = new AlphaAnimation(0.5F, 1F) { Duration = 0, FillAfter = true };
                                (sender as View).StartAnimation(alphaAnim);
                                break;

                            case MotionEventActions.Cancel:
                                alphaAnim = new AlphaAnimation(0.5F, 1F) { Duration = 0, FillAfter = true };
                                (sender as View).StartAnimation(alphaAnim);
                                break;
                        }
                    };
                }
            }
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

        #region Setup UI

        public void SetClickOutsideToHideKeyboard(View view)
        {

            if (!(view is EditText))
            {
                view.SetOnTouchListener(this);
            }

            //If a layout container, iterate over children and seed recursion.
            if (view is ViewGroup)
            {

                for (int i = 0; i < ((ViewGroup)view).ChildCount; i++)
                {

                    View innerView = ((ViewGroup)view).GetChildAt(i);

                    SetClickOutsideToHideKeyboard(innerView);
                }
            }
        }

        #endregion

        #region HideKeyboard

        public void HideKeyboard()
        {
            View view = this.CurrentFocus;
            if (view != null)
            {
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }
        }

        #endregion

        #endregion

       


    }
}