using Archivator_desktop_WPF_WTS.ViewModels;
using ArchivatorDb.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            ((ItemMDViewModel)DataContext).EditSelected();
        }

        private void bt_delete(object sender, RoutedEventArgs e)
        {
            ((ItemMDViewModel)DataContext).DeleteSelected();
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ((ItemMDViewModel)DataContext).SaveFile();
        }

        private void bt_print(object sender, RoutedEventArgs e)
        {
            ((ItemMDViewModel)DataContext).PrintSelected();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            FileEntity file = (FileEntity)((Button)sender).DataContext;

            StaticUtilities.PrintObject(file);
        }
    }
}
