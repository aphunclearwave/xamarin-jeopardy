using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xamarin.Forms;

namespace Jeopardy.ViewModels
{
    public abstract class BaseViewModel : ExtendedBindableObject, IViewModel
    {
        protected ILogger Log;
        protected INavigation Navigation;

        protected BaseViewModel(INavigation navigation, ILogger logger)
        {
            Navigation = navigation;
            Log = logger;
        }

        public abstract void Init();

        public async Task NavigateToPage(Page toPage)
        {
            if (toPage != null)
            {
                await Navigation.PushAsync(toPage, true);
            }
        }
    }

    public abstract class BaseViewModel<TParameter> : BaseViewModel, IViewModel<TParameter>
    {
        protected BaseViewModel(INavigation navigation, ILogger logger)
            : base(navigation, logger)
        {
        }

        public override void Init()
        {
        }

        public abstract void Init(TParameter initData);
    }

    public interface IViewModel
    {
        void Init();
    }

    public interface IViewModel<TParameter> : IViewModel
    {
        void Init(TParameter initData);

    }
}
