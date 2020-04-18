using System.Windows.Controls;

using Archivator_desktop_WPF_WTS.ViewModels;

namespace Archivator_desktop_WPF_WTS.Views
{
    public partial class MasterDetail2Page : Page
    {
        public MasterDetail2Page(MasterDetail2ViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
