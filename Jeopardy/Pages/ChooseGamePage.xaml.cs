using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Jeopardy.Pages
{
    public partial class ChooseGamePage
    {
        public ChooseGamePage()
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }
    }
}
