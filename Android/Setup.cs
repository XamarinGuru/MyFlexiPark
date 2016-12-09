using System;
using System.Collections.Generic;

using Android.Content;
using Android.Views;
using Cirrious.CrossCore.Converters;
using Cirrious.MvvmCross.Droid.Platform;
using Cirrious.MvvmCross.Droid.Views;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Binding.Bindings.Target.Construction;
using Cirrious.MvvmCross.Binding.Droid.Target;
using System.Reflection;
using Cirrious.CrossCore.Plugins;
using FlexyPark.Core;
using FlexyPark.Core.Converters;
using FlexyPark.Core.Services;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Converters;
using FlexyPark.UI.Droid.Services;
using Cirrious.MvvmCross.Localization;
using FlexyPark.Core.Helpers;

namespace FlexyPark.UI.Droid
{
    public class Setup : MvxAndroidSetup
    {
        #region Constructors
        public Setup(Context applicationContext)
            : base(applicationContext)
        {

        }
        #endregion

        #region Create App

        protected override IMvxApplication CreateApp()
        {


            Mvx.RegisterSingleton<ICacheService>(new CacheService());
            Mvx.RegisterSingleton<IFixMvvmCross>(new FixMvvmCross());
            Mvx.RegisterSingleton<IApiService>(new ApiService());
            Mvx.RegisterSingleton<ICalendarService>(new CalendarService());
            Mvx.RegisterSingleton<IPlatformService>(new PlatformService());
            Mvx.RegisterSingleton<IInAppPurchaseService>(new InAppPurchaseService());
			Mvx.RegisterSingleton<IStripeService>(new StripeService());
            var app = new App();

            return app;
        }
        #endregion

        #region Converter
        protected override void FillValueConverters(IMvxValueConverterRegistry registry)
        {
            base.FillValueConverters(registry);

            registry.AddOrOverwrite("ValidTime", new ValidTimeConverter());
            registry.AddOrOverwrite("BookingTimer", new BookingTimerConverter());
            registry.AddOrOverwrite("InverterBool", new InverterBoolConverter());
			registry.AddOrOverwrite("ReInverterBool", new ReInverterBoolConverter());
            registry.AddOrOverwrite("Money", new MoneyConverter());
            registry.AddOrOverwrite("SeekBarConverter", new SeekBarConverter());
            registry.AddOrOverwrite("ValidityTimeConverter", new ValidityTimeConverter());
            registry.AddOrOverwrite("ParkingTimerConverter", new ParkingTimerConverter());
			registry.AddOrOverwrite("DateOfBirthConverter", new DateOfBirthConverter());
            registry.AddOrOverwrite("DateTimeToStringConverter", new DateTimeToStringConverter());
            registry.AddOrOverwrite("SearchModeConverter", new SearchModeConverter());
            registry.AddOrOverwrite("HideSummaryConverter", new HideSummaryConverter());
            registry.AddOrOverwrite("ParkingTimeConverter", new ParkingTimeConverter());
            registry.AddOrOverwrite("TitleToDrawableConverter", new TitleToDrawableConverter());
            registry.AddOrOverwrite("ColorConverter", new ColorConverter());
            registry.AddOrOverwrite("StatusToImageResourConverter", new StatusToImageResourConverter());
            registry.AddOrOverwrite("StatusToBool", new StatusToBool());
            registry.AddOrOverwrite("OfferTimeToLeaveConverter", new OfferTimeToLeaveConverter());
            registry.AddOrOverwrite("DistanceConverter", new DistanceConverter());
            registry.AddOrOverwrite("ExpectedTimeConverter", new ExpectedTimeConverter());
            registry.AddOrOverwrite("AddEditButtonTitleConverter", new AddEditButtonTitleConverter());
            registry.AddOrOverwrite("AddEditPageTitleConverter", new AddEditPageTitleConverter());
            registry.AddOrOverwrite("ParkingSearchTitleConverter", new ParkingSearchTitleConverter());
            registry.AddOrOverwrite("MyProfileTitleConverter", new MyProfileTitleConverter());
            registry.AddOrOverwrite("ParkingReservedOverviewResumeConverter", new ParkingReservedOverviewResumeConverter());
            registry.AddOrOverwrite("BuyCreditItemConverter", new BuyCreditItemConverter());
            registry.AddOrOverwrite("MeterDoubleConverter", new MeterDoubleConverter());
            registry.AddOrOverwrite("CoordinatesConverter", new CoordinatesConverter());
            registry.AddOrOverwrite("DateTimeToHourConverter", new DateTimeToHourConverter());
            registry.AddOrOverwrite("BookingToWaitConverter", new BookingToWaitConverter());
            registry.AddOrOverwrite("BookingDurationConverter", new BookingDurationConverter());
            registry.AddOrOverwrite("RepeatSpecialCaseConverter", new RepeatSpecialCaseConverter());
            registry.AddOrOverwrite("ReportToBool", new ReportToBoolConverter());
            registry.AddOrOverwrite("RepeatSpecialCaseConverter", new RepeatSpecialCaseConverter());
            registry.AddOrOverwrite("EventItemTitleAndDateConverter", new EventItemTitleAndDateConverter());
            registry.AddOrOverwrite("IsShowDeleteConverter", new IsShowDeleteConverter());
            registry.AddOrOverwrite("MoneyStringConverter", new MoneyStringConverter());
            registry.AddOrOverwrite("RoundedDistanceConverter", new RoundedDistanceConverter());
            registry.AddOrOverwrite("DurationConverter", new DurationConverter());
            registry.AddOrOverwrite("StatusShowTitleConverter", new StatusShowTitleConverter());
            registry.AddOrOverwrite("MeterConverter", new MeterConverter());
            registry.AddOrOverwrite("AddEditButtonTitleAddEvent", new AddEditButtonTitleAddEvent());
            registry.AddOrOverwrite("EventRedGrayConverter", new EventRedGrayConverter());


        }
        #endregion

        protected override List<Assembly> ValueConverterAssemblies
        {
            get
            {
                var toReturn = base.ValueConverterAssemblies;
                toReturn.Add(typeof(MvxLanguageConverter).Assembly);
                return toReturn;
            }
        }

        protected override void FillTargetFactories(Cirrious.MvvmCross.Binding.Bindings.Target.Construction.IMvxTargetBindingFactoryRegistry registry)
        {
            base.FillTargetFactories(registry);


            registry.RegisterCustomBindingFactory<View>("Visible",
                view => new MvxViewVisibleBinding(view));
        }

        #region CreateViewPresenter

        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            var presenter = Mvx.IocConstruct<DroidPresenter>();
            Mvx.RegisterSingleton<IMvxAndroidViewPresenter>(presenter);
            return presenter;
        }

        #endregion

        //        protected override List<Assembly> ValueConverterAssemblies
        //		{
        //			get
        //			{
        //				var toReturn = base.ValueConverterAssemblies;
        //				toReturn.Add(typeof (MvxNativeColorValueConverter).Assembly);
        //				return toReturn;
        //			}
        //		}

        //		override plugin
        //
        //		protected override void AddPluginsLoaders(MvxLoaderPluginRegistry loaders)
        //		{
        //			base.AddPluginsLoaders(loaders);
        //			loaders.AddConventionalPlugin<Cirrious.MvvmCross.Plugins.Color.Droid.Plugin>();
        //		}

        //		public override void LoadPlugins(IMvxPluginManager pluginManager)
        //		{
        //			pluginManager.EnsurePluginLoaded<Cirrious.MvvmCross.Plugins.Color.PluginLoader>();
        //            pluginManager.EnsurePluginLoaded<Cirrious.MvvmCross.Plugins.Json.PluginLoader>();
        //			base.LoadPlugins(pluginManager);
        //		}

    }

    public class DroidPresenter : MvxAndroidViewPresenter
    {
        public override void Close(IMvxViewModel viewModel)
        {
            if (viewModel is CommonProfileViewModel || viewModel is RentProfileViewModel ||
                viewModel is OwnProfileViewModel)
            {
                Activity.Finish();
            }
            else
                base.Close(viewModel);
        }

        public override void Show(MvxViewModelRequest request)
        {
            if (request.PresentationValues != null)
            {
                if (request.PresentationValues.ContainsKey(PresentationBundleFlagKeys.ClearStack))
                {
                    var requestTranslator = Mvx.Resolve<IMvxAndroidViewModelRequestTranslator>();
                    var intent = requestTranslator.GetIntentFor(request);
                    intent.SetFlags(ActivityFlags.ClearTask);
                    Activity.StartActivity(intent);
                    return;
                }
            }

            base.Show(request);
        }
    }
}