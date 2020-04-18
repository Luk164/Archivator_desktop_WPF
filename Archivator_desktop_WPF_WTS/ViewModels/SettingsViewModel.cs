using System;
using System.Net.Mime;
using System.Windows.Input;

using Archivator_desktop_WPF_WTS.Contracts.Services;
using Archivator_desktop_WPF_WTS.Contracts.ViewModels;
using Archivator_desktop_WPF_WTS.Helpers;
using Archivator_desktop_WPF_WTS.Models;
using ArchivatorDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace Archivator_desktop_WPF_WTS.ViewModels
{
    public class SettingsViewModel : Observable, INavigationAware
    {
        private readonly AppConfig _config;
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly ISystemService _systemService;
        private readonly IApplicationInfoService _applicationInfoService;
        private AppTheme _theme;
        private string _versionDescription;
        private ICommand _setThemeCommand;
        private ICommand _privacyStatementCommand;

        private ArchivatorDbContext _context;

        public AppTheme Theme
        {
            get { return _theme; }
            set { Set(ref _theme, value); }
        }

        public string VersionDescription
        {
            get { return _versionDescription; }
            set { Set(ref _versionDescription, value); }
        }

        public ICommand SetThemeCommand => _setThemeCommand ??= new RelayCommand<string>(OnSetTheme);
        public ICommand PrivacyStatementCommand => _privacyStatementCommand ??= new RelayCommand(OnPrivacyStatement);

        public SettingsViewModel(IOptions<AppConfig> config, IThemeSelectorService themeSelectorService, ISystemService systemService, IApplicationInfoService applicationInfoService, ArchivatorDbContext context)
        {
            _config = config.Value;
            _themeSelectorService = themeSelectorService;
            _systemService = systemService;
            _applicationInfoService = applicationInfoService;
            _context = context;
        }

        public void OnNavigatedTo(object parameter)
        {
            VersionDescription = $"Archivator_desktop - {_applicationInfoService.GetVersion()}";
            Theme = _themeSelectorService.GetCurrentTheme();
        }

        public void OnNavigatedFrom()
        {
        }

        private void OnSetTheme(string themeName)
        {
            var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
            _themeSelectorService.SetTheme(theme);
        }

        private void OnPrivacyStatement()
            => _systemService.OpenInWebBrowser(_config.PrivacyStatement);

        public string GetDbConnString()
        {
            var currConnString = (string) App.Current.Properties[StaticUtilities.CONN_STRING_KEY];

            if (currConnString == null || string.IsNullOrWhiteSpace(currConnString))
            {
                currConnString = StaticUtilities.DEFAULT_CONNECTION_STRING;
            }

            return currConnString;
        }

        public bool OnSetConnString(string newConString)
        {
            if (newConString != null && !string.IsNullOrWhiteSpace(newConString))
            {
                if (_context.TestConnection())
                {
                    
                    App.Current.Properties[StaticUtilities.CONN_STRING_KEY] = newConString;
                    new ArchivatorDbContext();
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}
