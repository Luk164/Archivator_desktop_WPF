using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Archivator_desktop_WPF_WTS.Contracts.Services;
using Archivator_desktop_WPF_WTS.ViewModels;
using ArchivatorDb.Entities;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;
using MessageBox = System.Windows.MessageBox;

namespace Archivator_desktop_WPF_WTS.Views
{
    /// <summary>
    /// CodeBehind of the main page
    /// </summary>
    public partial class MainPage : Page
    {
        private OpenFileDialog fileDialog;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly MainViewModel _viewModel;
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Constructor for main page
        /// </summary>
        /// <param name="viewModel">MainPage view-model</param>
        /// <param name="navigationService">Service allowing navigation between pages</param>
        public MainPage(MainViewModel viewModel, INavigationService navigationService)
        {
            InitializeComponent();
            InitOpenFileDialog();
            DataContext = viewModel;
            _viewModel = viewModel;
            _navigationService = navigationService;
            ComboBox.ItemsSource = Item.CategoryList;
        }

        /// <summary>
        /// Initializes dialog for opening files.
        /// </summary>
        private void InitOpenFileDialog()
        {
            fileDialog = new OpenFileDialog {InitialDirectory = "c:\\", Multiselect = true, Title = "File selector"};
        }

        /// <summary>
        /// Starts process of adding files, disables submit button until it is finished.
        /// </summary>
        /// <param name="sender">Sender of action</param>
        /// <param name="e">Extra arguments</param>
        private async void btn_add_file_Click(object sender, RoutedEventArgs e)
        {
            bt_submit.IsEnabled=false;
            _viewModel.CurrItem.Files.AddRange(await MakeFileEntityListAsync());
            dg_files.Items.Refresh();
            progress_bar.Dispatcher.Invoke(() => progress_bar.Value = 100);
            bt_submit.IsEnabled=true;
        }

        /// <summary>
        /// Loads file in parallel and returns List of strings with their contents
        /// </summary>
        /// <returns>List of FileEntities containing the contents of selected files</returns>
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

        /// <summary>
        /// Saves all changes and navigates to Item master-detail page.
        /// </summary>
        /// <param name="sender">Sender of action</param>
        /// <param name="e">Extra parameters</param>
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

        /// <summary>
        /// Generates a list of files from provided list of paths.
        /// </summary>
        /// <param name="pathList">List of paths to be used for fileEntity generation</param>
        /// <returns></returns>
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
                    MessageBox.Show($"File \"{Path.GetFileName(filePath)}\" skipped because it was too large. Maximum allowed size is 25MB.\nThis file is {(fileInfo.Length - StaticUtilities.MAX_FILE_SIZE)} bytes over this limit.");
                }
                tasks.Add(Task.Run(() => {
                    var newFileEntity = new FileEntity
                    {
                        FileName = Path.GetFileName(filePath),
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

        private void Selector_OnItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            if (!(((CheckComboBox)sender).DataContext is EventEntity eventEntity))
            {
                return;
            }

            ((CheckComboBox) sender).SelectedItemsOverride = eventEntity.SelectedTags; //should fix some bugs

            StaticUtilities.SyncEventWithTags(eventEntity, eventEntity.SelectedTags.ToList()); //, _viewModel._context
        }

        private void DataGrid_OnAddingEvent(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = _viewModel.GetNewEventEntity();
        }

        private void Tb_PreviewTextInput_numbersOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void Tb_PreviewTextInput_5Max(object sender, TextCompositionEventArgs e)
        {
            e.Handled = ((TextBox) sender).Text.Length > 5;
        }

        private void Tb_new_tag_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((MainViewModel) DataContext).CreateTag(((TextBox) sender).Text);
                ((TextBox) sender).Clear();
            }
        }

        private void Bt_DescEditor(object sender, RoutedEventArgs e)
        {
            ((MainViewModel) DataContext).ShowBigEditor();
        }
    }
}
