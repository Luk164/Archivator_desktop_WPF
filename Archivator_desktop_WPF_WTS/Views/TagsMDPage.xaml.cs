using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Archivator_desktop_WPF_WTS.ViewModels;

namespace Archivator_desktop_WPF_WTS.Views
{
    public partial class MasterDetailPage : Page
    {
        public MasterDetailPage(TagsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void tb_new_tag_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((TagsViewModel) DataContext).CreateTag(((TextBox) sender).Text);
                ((TextBox) sender).Clear();
            }
        }

        private void SubmitChanges(object sender, KeyEventArgs e)
        {
            ((TagsViewModel) DataContext).SubmitChanges();
        }

        private void bt_delete(object sender, RoutedEventArgs e)
        {
            ((TagsViewModel) DataContext).DeleteSelected();
        }
    }
}
