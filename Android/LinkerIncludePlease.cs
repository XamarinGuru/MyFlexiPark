using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Falcon.UI.Droid
{
    class LinkerIncludePlease
    {
        private void IncludeVisibility(View widget)
        {
            widget.Visibility = widget.Visibility + 1;
        }

        private void IncludeClick(View widget)
        {
            widget.Click += (s, e) => { };
        }

        private void IncludeRelativeLayout(RelativeLayout relativeLayout)
        {
            relativeLayout.Click += (s, e) => { };
        }

        public void Include(EditText widget)
        {
            widget.Enabled = widget.Enabled;
        }

        public void IncludeIsCator(RatingBar widget)
        {
            widget.IsIndicator = widget.IsIndicator;
        }

		public void IncludeCheckBox(CheckBox widget)
		{
			widget.CheckedChange += (sender, e) => { };
		}

		public void IncludeButton(Button button)
		{
			button.Enabled = button.Enabled;
		}

    }
}