using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (DataContext as ItemMDViewModel).SaveFile();
        }
    }
}
