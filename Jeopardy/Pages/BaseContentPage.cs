using System;
using Autofac;
using Jeopardy.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Jeopardy.Pages
{
    public class BaseContentPage<TViewModel>
        : ContentPage, IBindableView<TViewModel> where TViewModel : class, IViewModel
    {
        public TViewModel ViewModel { get; set; }
        IViewModel IBindableView.ViewModel { get => ViewModel as IViewModel; set => ViewModel = value as TViewModel; }

        protected BaseContentPage()
        {
            On<iOS>().SetUseSafeArea(true);

            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, true);

            if (App.Container != null)
            {
                // The BindingContext = ViewModel call is left to the derived Page to do, 
                // Xamarin best practice says to bind it after InitializeComponent() so we can't do it here.
                using (var scope = App.Container.BeginLifetimeScope(builder =>
                    builder.RegisterInstance(Navigation).As<INavigation>()))
                {
                    ViewModel = scope.Resolve<TViewModel>();
                    ViewModel.Init();
                }
            }
        }
    }

    public class BaseContentPage<TViewModel, TParameter>
        : BaseContentPage<TViewModel> where TViewModel : class, IViewModel<TParameter>, IViewModel
    {
        // This paramaterless constructor is needed by XAML
        public BaseContentPage()
        { }

        protected BaseContentPage(TParameter initData)
        {
            if (ViewModel != null)
            {
                ViewModel.Init(initData);
            }
        }
    }

    public interface IBindableView
    {
        IViewModel ViewModel { get; set; }
        object BindingContext { get; set; }
    }

    public interface IBindableView<TViewModel> : IBindableView where TViewModel : class, IViewModel
    {
        new TViewModel ViewModel { get; set; }
    }






    public class BaseBgContentPage<TViewModel>
        : BaseContentPage<TViewModel>, IBindableView<TViewModel> where TViewModel : class, IViewModel
    {
        protected BaseBgContentPage()
        {
        }
    }

    public class BaseBgContentPage<TViewModel, TParameter>
        : BaseContentPage<TViewModel, TParameter> where TViewModel : class, IViewModel<TParameter>, IViewModel
    {
        // This paramaterless constructor is needed by XAML
        public BaseBgContentPage()
        { }

        protected BaseBgContentPage(TParameter initData) : base(initData)
        {
        }
    }
}
