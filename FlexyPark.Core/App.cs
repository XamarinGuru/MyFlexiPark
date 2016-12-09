using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.ViewModels;
using FlexyPark.Core.Services;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Localization;
using Cirrious.MvvmCross.Plugins.JsonLocalisation;
using Akavache;
using FlexyPark.Core.Models;
using System.Diagnostics;

namespace FlexyPark.Core
{
    public class App : MvxApplication
    {
        public App()
        {
            //initialize text provider builder
            var builder = new TextProviderBuilder();
            Mvx.RegisterSingleton<IMvxTextProvider>(builder.TextProvider);
            Mvx.RegisterSingleton<IMvxTextProviderBuilder>(builder);

            //must set application name . for akavache to work correctly
            BlobCache.ApplicationName = "AkavacheExperiment";

            RegisterAppStart(new FlexyParkAppStart(Mvx.Resolve<IPlatformService>()));

            //RegisterAppStart<SignInViewModel>();

//            User user = null;
//            BlobCache.UserAccount.GetObject<User>("CurrentUser")
//                .Subscribe(x => {
//                    user = x; Debug.WriteLine("Got user in cache");
//                    Debug.WriteLine(user);
//                    if(user != null)
//                    {
//                        Mvx.Resolve<ICacheService>().CurrentUser = user;
//                        RegisterAppStart<MenuViewModel>();
//                    }
//                    else
//                        RegisterAppStart<SignInViewModel>();
//                }, ex => 
//                    {
//                        Debug.WriteLine("No Key!");
//                        RegisterAppStart<SignInViewModel>();
//                    });
            
        }
    }
}

