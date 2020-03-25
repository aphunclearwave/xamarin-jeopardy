using System;
using System.Collections.Generic;
using Jeopardy.VMParameters;
using Xamarin.Forms;

namespace Jeopardy.Pages
{
    public partial class GamePage
    {
        public GamePage(GameVMParameter initData) : base(initData)
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }
    }
}
