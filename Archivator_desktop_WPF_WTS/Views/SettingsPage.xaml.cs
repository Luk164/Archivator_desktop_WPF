using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Archivator_desktop_WPF_WTS.Models;
using Archivator_desktop_WPF_WTS.ViewModels;
using Microsoft.Extensions.Options;

namespace Archivator_desktop_WPF_WTS.Views
{
    public partial class SettingsPage : Page
    {


        public SettingsPage(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            //tb_connection_string.Text = viewModel.GetDbConnString();
        }

        // TODO bring back simple connection string change
        //private void Tb_connection_string_OnKeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.Key != Key.Return) return;

        //    var textBox_connString = (TextBox) sender;
        //    var viewModel = (SettingsViewModel) DataContext;

        //    if (!viewModel.OnSetConnString(textBox_connString.Text))
        //    {
        //        MessageBox.Show("There was something wrong with the connection string, please check it again. No changes were applied.",
        //            "Faulty connection string");
        //    }
        //}
    }
}
