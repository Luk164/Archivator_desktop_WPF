using System.Windows.Controls;
using Archivator_desktop_WPF_WTS.ViewModels;

namespace Archivator_desktop_WPF_WTS.Views
{
    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage(AboutPageViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
