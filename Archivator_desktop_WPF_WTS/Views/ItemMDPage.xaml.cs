using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Archivator_desktop_WPF_WTS.Converters;
using Archivator_desktop_WPF_WTS.ViewModels;
using MahApps.Metro.Controls;
using FontStyle = System.Windows.FontStyle;
using Image = System.Windows.Controls.Image;

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
            ((ItemMDViewModel) DataContext).Print();
        }
    }
}
