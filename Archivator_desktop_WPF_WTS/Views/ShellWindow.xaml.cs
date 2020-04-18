using System.Windows.Controls;

using Archivator_desktop_WPF_WTS.Contracts.Views;
using Archivator_desktop_WPF_WTS.ViewModels;

using MahApps.Metro.Controls;

namespace Archivator_desktop_WPF_WTS.Views
{
    public partial class ShellWindow : MetroWindow, IShellWindow
    {
        public ShellWindow(ShellViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public Frame GetNavigationFrame()
            => shellFrame;

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();
    }
}
