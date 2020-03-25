using System;
using System.Collections.Generic;
using Jeopardy.VMParameters;
using Xamarin.Forms;

namespace Jeopardy.Pages
{
    public partial class QuestionPage
    {
        public QuestionPage(QuestionVMParameter initData) : base(initData)
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }
    }
}
