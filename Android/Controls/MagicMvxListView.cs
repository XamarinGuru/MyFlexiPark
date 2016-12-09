using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid.ResourceHelpers;
using Cirrious.MvvmCross.Binding.Droid.Views;

namespace FlexyPark.UI.Droid.Controls
{
    public class MagicMvxListView : ListView
    {
        public static class MvxListViewHelpers
        {
            public static int ReadAttributeValue(Context context, IAttributeSet attrs, int[] groupId,
                                                 int requiredAttributeId)
            {
                var typedArray = context.ObtainStyledAttributes(attrs, groupId);

                try
                {
                    var numStyles = typedArray.IndexCount;
                    for (var i = 0; i < numStyles; ++i)
                    {
                        var attributeId = typedArray.GetIndex(i);
                        if (attributeId == requiredAttributeId)
                        {
                            return typedArray.GetResourceId(attributeId, 0);
                        }
                    }
                    return 0;
                }
                finally
                {
                    typedArray.Recycle();
                }
            }
        }

        public MagicMvxListView(Context context, IAttributeSet attrs)
            : this(context, attrs, new MvxAdapter(context))
        {
        }

        public MagicMvxListView(Context context, IAttributeSet attrs, MvxAdapter adapter)
            : base(context, attrs)
        {
            var itemTemplateId = MvxListViewHelpers.ReadAttributeValue(context, attrs,
                                                                       MvxAndroidBindingResource.Instance
                                                                                                .ListViewStylableGroupId,
                                                                       MvxAndroidBindingResource.Instance
                                                                                                .ListItemTemplateId);
            adapter.ItemTemplateId = itemTemplateId;
            Adapter = adapter;
            SetupItemClickListeners();
        }

        public new MvxAdapter Adapter
        {
            get { return base.Adapter as MvxAdapter; }
            set
            {
                var existing = Adapter;
                if (existing == value)
                    return;

                if (existing != null && value != null)
                {
                    value.ItemsSource = existing.ItemsSource;
                    value.ItemTemplateId = existing.ItemTemplateId;
                }

                base.Adapter = value;
            }
        }

        public IEnumerable ItemsSource
        {
            get { return Adapter.ItemsSource; }
            set { Adapter.ItemsSource = value; }
        }

        public int ItemTemplateId
        {
            get { return Adapter.ItemTemplateId; }
            set { Adapter.ItemTemplateId = value; }
        }

        public new ICommand ItemClick { get; set; }

        public new ICommand ItemLongClick { get; set; }

        protected void SetupItemClickListeners()
        {
            base.ItemClick += (sender, args) => ExecuteCommandOnItem(this.ItemClick, args.Position);
            base.ItemLongClick += (sender, args) => ExecuteCommandOnItem(this.ItemLongClick, args.Position);
        }

        protected virtual void ExecuteCommandOnItem(ICommand command, int position)
        {
            if (command == null)
                return;

            if (!command.CanExecute(position))
                return;

            command.Execute(position);
        }
    }
}