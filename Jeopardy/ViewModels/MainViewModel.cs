using System.Threading.Tasks;
using Jeopardy.Data;
using Jeopardy.Pages;
using Microsoft.Extensions.Logging;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Jeopardy.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel(INavigation navigation, ILogger<MainViewModel> logger)
            : base(navigation, logger)
        {

        }

        public override void Init()
        {
            Task.Run(async () =>
            {
                await InitData();
            });
        }

        async Task InitData()
        {
            DataRepository.Cleanup();
            var data = await SampleDataEngine.GetSampleData();
            if (data != null)
            {
                await SampleDataEngine.SampleDataToLocal(data);
            }

            Device.BeginInvokeOnMainThread(async () =>
            {
                await NavigateToPage(new ChooseGamePage());
            });
        }
    }
}
