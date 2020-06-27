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

        private void bt_print_selected_single(object sender, RoutedEventArgs e)
        {
            ((ItemMDViewModel) DataContext).PrintSelectedItem();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var file = (FileEntity)((Button) sender).DataContext;

            StaticUtilities.PrintObject(file);
        }

        private void bt_print_all(object sender, RoutedEventArgs e)
        {
            ((ItemMDViewModel) DataContext).PrintAll();
        }

        private void bt_print_missing(object sender, RoutedEventArgs e)
        {
            ((ItemMDViewModel) DataContext).PrintMissing();
        }

        private void bt_print_selection(object sender, RoutedEventArgs e)
        {
            ((ItemMDViewModel) DataContext).PrintSelection();
        }

        private void tb_item_selection_checked(object sender, RoutedEventArgs e)
        {
            var item =(Item) ((CheckBox)sender).DataContext;

            var isChecked = ((CheckBox)sender).IsChecked;
            if (isChecked == null)
            {
                return;
            }

            ((ItemMDViewModel) DataContext).SelectedItems.Add(item);
        }

        private void tb_item_selection_unchecked(object sender, RoutedEventArgs e)
        {
            var item =(Item) ((CheckBox)sender).DataContext;

            var isChecked = ((CheckBox)sender).IsChecked;
            if (isChecked == null)
            {
                return;
            }

            ((ItemMDViewModel) DataContext).SelectedItems.Remove(item);
        }

        private void bt_delete_selection(object sender, RoutedEventArgs e)
        {
            ((ItemMDViewModel) DataContext).DeleteSelection();
        }

        private void Bt_DescViewer(object sender, RoutedEventArgs e)
        {
            ((ItemMDViewModel) DataContext).ShowBigViewer();
        }

        private void Bt_RefreshContext(object sender, RoutedEventArgs e)
        {
            ((ItemMDViewModel) DataContext).RefreshDbContext();
        }
    }
}
