using System;
using Autofac;
using Jeopardy.Pages;
using Jeopardy.ViewModels;
using Microsoft.Extensions.Logging;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Jeopardy
{
    public partial class App : Application
    {
        public static IContainer Container;
        static readonly ContainerBuilder _builder = new ContainerBuilder();

        public App()
        {
            InitializeComponent();

            DependencyResolver.ResolveUsing(type => Container.IsRegistered(type) ? Container.Resolve(type) : null);

            Register();

            // set the start page
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        void Register()
        {
            // register types with interfaces
            _builder.RegisterType<LoggerFactory>().As<ILoggerFactory>().SingleInstance();
            _builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();

            // register view models
            _builder.RegisterType<MainViewModel>().AsSelf();
            _builder.RegisterType<ChooseGameViewModel>().AsSelf();
            _builder.RegisterType<GameViewModel>().AsSelf();
            _builder.RegisterType<RoundsViewModel>().AsSelf();
            _builder.RegisterType<QuestionViewModel>().AsSelf();

            Container = _builder.Build();
        }
    }
}
