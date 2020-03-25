using System;
using System.Collections.Generic;
using Jeopardy.VMParameters;
using Xamarin.Forms;

namespace Jeopardy.Pages
{
    public partial class RoundsPage
    {
        public RoundsPage(RoundsVMParameter initData) : base(initData)
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }
    }
}
