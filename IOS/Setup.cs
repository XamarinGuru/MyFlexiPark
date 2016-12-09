using System;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.CrossCore;
using System.Reflection;
using System.Collections.Generic;
using Cirrious.CrossCore.Plugins;
using UIKit;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Touch.Views;
using FlexyPark.Core.ViewModels;
using FlexyPark.Core.Helpers;
using FlexyPark.Core;
using FlexyPark.Core.Services;
using FlexyPark.UI.Touch.Services;
using Cirrious.MvvmCross.Localization;
using Cirrious.CrossCore.Converters;
using FlexyPark.UI.Touch.Converters;
using Cirrious.MvvmCross.Binding.Bindings.Target.Construction;
using Cirrious.MvvmCross.Binding.Touch.Target;
using System.Linq.Expressions;
using FlexyPark.UI.Touch.Services.InAppPurchaseService;
using FlexyPark.Core.Converters;

namespace FlexyPark.UI.Touch
{
	public class Setup: MvxTouchSetup
	{
		public Setup(MvxApplicationDelegate appDelegate, IMvxTouchViewPresenter presenter)
			: base(appDelegate, presenter)
		{
		}
			
		#region CreateApp
		protected override Cirrious.MvvmCross.ViewModels.IMvxApplication CreateApp()
		{
//			Mvx.RegisterSingleton<IImageService>(new ImageService());
//			Mvx.RegisterSingleton<ITouchService>(new TouchService());
//			Mvx.RegisterSingleton<IRecordService>(new RecordService());
            Mvx.RegisterSingleton<IDelegateManager>(new DelegateManager());
			Mvx.RegisterSingleton<IPlatformService>(new PlatformService());
			Mvx.RegisterSingleton<ICacheService>(new CacheService());
            Mvx.RegisterSingleton<IApiService>(new ApiService());
            Mvx.RegisterSingleton<IStripeService> (new StripeService ());

            var iapService = new InAppPurchaseService(new PurchaseManager());
            Mvx.RegisterSingleton<IInAppPurchaseService>(iapService);
            iapService.Initialize();
//            var dataService = new DataService();
//            dataService.Initialize();
//
//            Mvx.RegisterSingleton<IDataService>(() => dataService);

            return new App();
		}
		#endregion

        #region Converter
        protected override void FillValueConverters(IMvxValueConverterRegistry registry)
        {
            base.FillValueConverters(registry);
            registry.AddOrOverwrite("BooleanToHidden", new BooleanToHiddenConverter());
            registry.AddOrOverwrite("BytesToUIImage", new BytesToUIImageConverter());
            registry.AddOrOverwrite("ParkingStatusToBool", new ParkingStatusToBoolConverter());
            registry.AddOrOverwrite("SearchModeToBool", new SearchModeToBoolConverter());
            registry.AddOrOverwrite("Step", new StepConverter());
            registry.AddOrOverwrite("ReportModeToBoolean", new ReportModeToBooleanConverter());
			//registry.AddOrOverwrite("DateOfBirth", new DateOfBirthConverter());

        }
        #endregion

		protected override List<Assembly> ValueConverterAssemblies
		{
			get
			{
				var toReturn = base.ValueConverterAssemblies;
                toReturn.Add(typeof (MvxLanguageConverter).Assembly);
				return toReturn;
			}
		}

		protected override void AddPluginsLoaders(MvxLoaderPluginRegistry loaders)
		{
            loaders.AddConventionalPlugin<Cirrious.MvvmCross.Plugins.File.Touch.Plugin>();
            loaders.AddConventionalPlugin<Cirrious.MvvmCross.Plugins.ResourceLoader.Touch.Plugin>();
            base.AddPluginsLoaders(loaders);
		}

        public override void LoadPlugins(IMvxPluginManager pluginManager)
		{
            pluginManager.EnsurePluginLoaded<Cirrious.MvvmCross.Plugins.Messenger.PluginLoader>();
            pluginManager.EnsurePluginLoaded<Cirrious.MvvmCross.Plugins.Json.PluginLoader>();
            pluginManager.EnsurePluginLoaded<Cirrious.MvvmCross.Plugins.JsonLocalisation.PluginLoader>();
            pluginManager.EnsurePluginLoaded<Cirrious.MvvmCross.Plugins.File.PluginLoader>();
            pluginManager.EnsurePluginLoaded<Cirrious.MvvmCross.Plugins.ResourceLoader.PluginLoader>();
			base.LoadPlugins(pluginManager);
		}

        protected override void FillBindingNames(Cirrious.MvvmCross.Binding.BindingContext.IMvxBindingNameRegistry obj)
        {
            base.FillBindingNames(obj);
        }

        protected override void FillTargetFactories(Cirrious.MvvmCross.Binding.Bindings.Target.Construction.IMvxTargetBindingFactoryRegistry registry)
        {
            registry.RegisterCustomBindingFactory<UISlider>(
                "Value",
                target => new MvxUISliderValueTargetBinding(target, PropertyHelper<UISlider>.GetProperty(x => x.Value)));
            registry.RegisterCustomBindingFactory<UISwitch>(
                "On",
                target => new MvxUISwitchOnTargetBinding(target, PropertyHelper<UISwitch>.GetProperty(x=>x.On)));
            base.FillTargetFactories(registry);
        }

		#region Get View Model Assemblies
		protected override Assembly[] GetViewModelAssemblies()
		{
			var list = new List<Assembly>();
			list.AddRange(base.GetViewModelAssemblies());
            list.Add(typeof(SignInViewModel).Assembly);
            list.Add(typeof(ParkingListsViewModel).Assembly);
			return list.ToArray();
		}


		#endregion

        public static class PropertyHelper<T>
        {
            public static PropertyInfo GetProperty<TValue>(
                Expression<Func<T, TValue>> selector)
            {
                Expression body = selector;
                if (body is LambdaExpression)
                {
                    body = ((LambdaExpression)body).Body;
                }
                switch (body.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        return (PropertyInfo)((MemberExpression)body).Member;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
	}

}

