using System.Windows.Controls;

namespace Archivator_desktop_WPF_WTS.Contracts.Views
{
    public interface IShellWindow
    {
        Frame GetNavigationFrame();

        void ShowWindow();

        void CloseWindow();
    }
}
