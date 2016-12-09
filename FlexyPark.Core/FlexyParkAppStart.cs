using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Models;
using Akavache;
using System.Collections.Generic;
using FlexyPark.Core.ViewModels;
using FlexyPark.Core.Services;

namespace FlexyPark.Core
{
    public class FlexyParkAppStart :  MvxNavigatingObject, IMvxAppStart
    {
        private IPlatformService mPlatformService;
        public FlexyParkAppStart(IPlatformService platformService)
        {
            this.mPlatformService = platformService;
        }

        public async void Start(object hint = null)
        {
            if (mPlatformService.IsLogin())
                ShowViewModel<MenuViewModel>(new {credits = -1});
            else
                //ShowViewModel<SignInViewModel>();
				ShowViewModel<InitialMapViewModel>();
        }
    }
}

