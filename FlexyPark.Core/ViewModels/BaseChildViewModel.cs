using System;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using System.Collections.Generic;

namespace FlexyPark.Core.ViewModels
{
    public class BaseChildViewModel : MvxViewModel
    {
		protected void ShowViewModelParameter<TViewModel>(object parameter) where TViewModel : BaseViewModel
		{
			var text = Mvx.Resolve<IMvxJsonConverter>().SerializeObject(parameter);
			//var text = JsonConvert.SerializeObject(new JobViewModel(new Job() {Id = 1} ));

			base.ShowViewModel<TViewModel>(new Dictionary<string, string>()
				{
					{"parameter", text}
				});
		}
    }
}

