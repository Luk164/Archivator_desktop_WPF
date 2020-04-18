using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Archivator_desktop_WPF_WTS.Contracts.ViewModels;
using Archivator_desktop_WPF_WTS.ViewModels;

namespace Archivator_desktop_WPF_WTS.Views
{
    public partial class ItemMDPage : Page
    {
        public ItemMDPage(ItemMDViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            //dg_test.ItemsSource = viewModel.Selected.Files;
        }

        private void bt_edit(object sender, RoutedEventArgs e)
        {
            (DataContext as ItemMDViewModel).EditSelected();
        }

        private void bt_delete(object sender, RoutedEventArgs e)
        {
            (DataContext as ItemMDViewModel).DeleteSelected();
        }
    }
}
