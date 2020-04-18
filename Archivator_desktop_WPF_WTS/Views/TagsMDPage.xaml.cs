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

        private void tb_new_tag_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                (DataContext as TagsViewModel).CreateTag(tb_new_tag.Text);
                MessageBox.Show("Success");
            }
        }

        private void SubmitChanges(object sender, KeyEventArgs e)
        {
            (DataContext as TagsViewModel).SubmitChanges();
        }
    }
}
