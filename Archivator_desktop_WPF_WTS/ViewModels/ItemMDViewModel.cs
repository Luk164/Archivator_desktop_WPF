using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Archivator_desktop_WPF_WTS.Contracts.Services;
using Archivator_desktop_WPF_WTS.Contracts.ViewModels;
using Archivator_desktop_WPF_WTS.Helpers;
using Archivator_desktop_WPF_WTS.Models;
using ArchivatorDb;
using ArchivatorDb.Entities;
using Microsoft.Win32;

namespace Archivator_desktop_WPF_WTS.ViewModels
{
    /// <summary>
    /// View model for Item Master Detail page.
    /// </summary>
    public class ItemMDViewModel : Observable, INavigationAware
    {
        private Item _selected;
        private readonly ArchivatorDbContext _context;
        private readonly INavigationService _navigationService;
        private FileEntity _selectedFile;

        /// <summary>
        /// Proxy for accessing selected file object. It cannot be accessed directly.
        /// </summary>
        public FileEntity SelectedFile
        {
            get => _selectedFile;
            set => Set(ref _selectedFile, value);
        }

        /// <summary>
        /// Proxy for accessing selected item object. It cannot be accessed directly.
        /// </summary>
        public Item Selected
        {
            get => _selected;
            set => Set(ref _selected, value);
        }

        /// <summary>
        /// List of items used in GUI
        /// </summary>
        public ObservableCollection<Item> Items { get; private set; }

        /// <summary>
        /// Constructor for Item master-detail viewmodel.
        /// </summary>
        /// <param name="context">ArchivatorDbContext used to interact with database</param>
        /// <param name="navigationService">Service allowing navigation between different pages</param>
        public ItemMDViewModel(ArchivatorDbContext context, INavigationService navigationService)
        {
            _context = context;
            _navigationService = navigationService;
        }

        /// <summary>
        /// Navigates to Main page and sets current item as the edited one.
        /// </summary>
        public void EditSelected()
        {
            _navigationService.NavigateTo(typeof(MainViewModel).FullName, new EditModel(){context = _context, editedObject = Selected});
        }

        /// <summary>
        /// Deletes currently selected item, then sets it to first item in list or new item is list is empty.
        /// </summary>
        public void DeleteSelected()
        {
            try
            {
                var result = MessageBox.Show("Are you sure you want to delete this editedObject?", "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Remove(Selected);
                    Items.Remove(Selected);
                    Selected = Items.FirstOrDefault() ?? new Item();
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An unhandled exception just occurred: " + e.Message, "Exception Occured", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void OnNavigatedTo(object parameter) //can be async
        {
            try
            {
                //Items = new ObservableCollection<Item>(_context.Items.AsNoTracking());
                Items = new ObservableCollection<Item>(_context.Items);

                Selected = Items.FirstOrDefault() ?? new Item();
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show("An unhandled exception just occurred: " + e.Message, "Exception Occured", MessageBoxButton.OK, MessageBoxImage.Warning);
                throw;
            }
        }

        public void OnNavigatedFrom()
        {
        }

        /// <summary>
        /// Opens a dialog for saving a file from database to disk.
        /// </summary>
        public void SaveFile()
        {
            if (_selectedFile == null)
            {
                MessageBox.Show("ERROR: No file selected!", "General error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = StaticUtilities.FILE_FILTER_STRING,
                    DefaultExt = Path.GetExtension(_selectedFile.FileName) ?? throw new InvalidOperationException(),
                    FileName = Path.GetFileNameWithoutExtension(_selectedFile.FileName) ??
                               throw new InvalidOperationException()
                };
                if (saveFileDialog.ShowDialog() == false) //if cancelled
                {
                    return;
                }
                    
                if (saveFileDialog.FileName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                {
                    MessageBox.Show("Invalid character/s detected in fileName!\nInvalid characters: " + Path.GetInvalidFileNameChars());
                }
                else
                {
                    File.WriteAllBytesAsync(saveFileDialog.FileName, _selectedFile.Data);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("ERROR: A following exception has occured!: " + e.Message, "Unexpected exception",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Opens a print dialog for currently selected Item.
        /// </summary>
        public void PrintSelected()
        {
            StaticUtilities.PrintObject(Selected);
        }
    }
}
