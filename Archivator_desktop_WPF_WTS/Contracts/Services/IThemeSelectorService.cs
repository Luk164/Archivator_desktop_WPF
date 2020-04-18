using Archivator_desktop_WPF_WTS.Models;

namespace Archivator_desktop_WPF_WTS.Contracts.Services
{
    public interface IThemeSelectorService
    {
        bool SetTheme(AppTheme? theme = null);

        AppTheme GetCurrentTheme();
    }
}
