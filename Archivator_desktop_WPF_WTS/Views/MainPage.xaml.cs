using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Windows.ApplicationModel.UserDataTasks;
using Archivator_desktop_WPF_WTS.Contracts.Services;
using Archivator_desktop_WPF_WTS.Services;
using Archivator_desktop_WPF_WTS.ViewModels;
using ArchivatorDb;
using ArchivatorDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace Archivator_desktop_WPF_WTS.Views
{
    public partial class MainPage : Page
    {
        private OpenFileDialog fileDialog;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly MainViewModel _viewModel;
        private readonly INavigationService _navigationService;

        public MainPage(MainViewModel viewModel, INavigationService navigationService)
        {
            InitializeComponent();
            InitOpenFileDialog();
            DataContext = viewModel;
            _viewModel = viewModel;
            _navigationService = navigationService;
        }

        private void InitOpenFileDialog()
        {
            fileDialog = new OpenFileDialog {InitialDirectory = "c:\\", Multiselect = true, Title = "File selector"};
        }

        private async void btn_add_file_Click(object sender, RoutedEventArgs e)
        {
            bt_submit.IsEnabled=false;
            _viewModel.CurrItem.Files.AddRange(await MakeFileEntityListAsync());
            dg_files.Items.Refresh();
            progress_bar.Dispatcher.Invoke(() => progress_bar.Value = 100);
            bt_submit.IsEnabled=true;
        }

        /// <summary>
        /// Loads file in parallel and returns List<string> with their contents
        /// </summary>
        /// <returns>Task<List<string>> Containing the contents of selected files</returns>
        private async Task<List<FileEntity>> MakeFileEntityListAsync()
        {
            if (fileDialog.ShowDialog() == true) //check if show dialog successful
            {
                return await makeFileEntitiesFromPathListAsync(fileDialog.FileNames);
            }
            return new List<FileEntity>();
        }

        private void bt_cancel_all_operations(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
            progress_bar.Dispatcher.Invoke(() => progress_bar.Value = 0);
        }

        private void Bt_submit_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveChanges();
            _navigationService.NavigateTo(typeof(ItemMDViewModel).FullName);
        }

        private async void files_Drop_action(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            // Note that you can have more than one file.
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            _viewModel.CurrItem.Files.AddRange(await makeFileEntitiesFromPathListAsync(files));
            dg_files.Items.Refresh();
        }

        private async Task<List<FileEntity>> makeFileEntitiesFromPathListAsync(IEnumerable<string> pathList)
        {
            var filePaths = pathList.ToList();
            if (!filePaths.Any())
            {
                return new List<FileEntity>();
            }

            var tasks = new List<Task<FileEntity>>();
            int percentage = 100/filePaths.Count;
            progress_bar.Dispatcher.Invoke(() => progress_bar.Value = 0);

            foreach (var filePath in filePaths)
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > StaticUtilities.MAX_FILE_SIZE)
                {
                    MessageBox.Show($"File \"{System.IO.Path.GetFileName(filePath)}\" skipped because it was too large. Maximum allowed size is 25MB.\nThis file is {(fileInfo.Length - StaticUtilities.MAX_FILE_SIZE)} bytes over this limit.");
                }
                tasks.Add(Task.Run(() => {
                    var newFileEntity = new FileEntity()
                    {
                        FileName = System.IO.Path.GetFileName(filePath),
                        Data=File.ReadAllBytes(filePath)
                    };

                    progress_bar.Dispatcher.Invoke(() =>
                    {
                        if (progress_bar.Value + percentage > 100)
                        {
                            progress_bar.Value = 100;
                        }
                        else
                        {
                            progress_bar.Value += percentage;
                        }
                    }, DispatcherPriority.Render);
                    
                    return newFileEntity;
                }, cancellationTokenSource.Token));
            }
            var results = new List<FileEntity>(await Task.WhenAll(tasks));
            progress_bar.Dispatcher.Invoke(() => progress_bar.Value = 0);
            return results;
        }

        private void Tag_checked(object sender, RoutedEventArgs e)
        {
            _viewModel.AddTag((int) ((CheckBox)sender).Tag);
        }

        private void Tag_unchecked(object sender, RoutedEventArgs e)
        {
            _viewModel.RemoveTag((int) ((CheckBox)sender).Tag);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
            {
                //Size pageSize = new Size(dialog.PrintableAreaWidth , dialog.PrintableAreaHeight );
                //bt_submit.Measure(pageSize);
                dialog.PrintVisual(dg_files, "Rapport");
            }
            //dialog.PrintVisual(bt_submit, "");
        }
    }
}
