using Archivator_desktop_WPF_WTS.Contracts.Services;
using Archivator_desktop_WPF_WTS.Contracts.ViewModels;
using Archivator_desktop_WPF_WTS.Helpers;

namespace Archivator_desktop_WPF_WTS.ViewModels
{
    public class AboutPageViewModel : Observable, INavigationAware
    {
        private readonly IApplicationInfoService _applicationInfoService;
        private string _versionDescription;
        public string VersionDescription
        {
            get => _versionDescription;
            private set => Set(ref _versionDescription, value);
        }

        public AboutPageViewModel(IApplicationInfoService applicationInfoService)
        {
            _applicationInfoService = applicationInfoService;
        }

        public void OnNavigatedTo(object parameter)
        {
        }

        public void OnNavigatedFrom()
        {
            VersionDescription = $"Archivator_desktop - {_applicationInfoService.GetVersion()}";
        }
    }
}