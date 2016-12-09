using System;
using Cirrious.MvvmCross.Plugins.JsonLocalisation;
using System.Collections.Generic;
using System.Linq;
using Cirrious.CrossCore.IoC;
using System.Reflection;

namespace FlexyPark.Core.Services
{
    public class TextProviderBuilder : MvxTextProviderBuilder
    {
        public TextProviderBuilder() : base(AppConstants.NameSpace, AppConstants.FolderResources)
        {
        }

        protected override IDictionary<string, string> ResourceFiles
        {
            get
            {
                var dictionary = this.GetType().GetTypeInfo().Assembly.CreatableTypes().Where(t => t.Name.EndsWith("ViewModel")).ToDictionary(t => t.Name, t => t.Name);
                dictionary.Add("Shared", "SharedResources");

                return dictionary;
            }

        }
    }
}
