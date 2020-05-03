using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Archivator_desktop_WPF_WTS.ViewModels;
using ArchivatorDb.Entities;

namespace Archivator_desktop_WPF_WTS.Views
{
    public partial class ItemMDPage : Page
    {
        public ItemMDPage(ItemMDViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void bt_edit(object sender, RoutedEventArgs e)
        {
            ((ItemMDViewModel) DataContext).EditSelected();
        }

        private void bt_delete(object sender, RoutedEventArgs e)
        {
            ((ItemMDViewModel) DataContext).DeleteSelected();
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ((ItemMDViewModel) DataContext).SaveFile();
        }

        private void bt_print(object sender, RoutedEventArgs e)
        {
            ((ItemMDViewModel) DataContext).PrintItem();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var file = (FileEntity)((Button) sender).DataContext;

            ((ItemMDViewModel) DataContext).PrintFile(file);
        }
    }
}
