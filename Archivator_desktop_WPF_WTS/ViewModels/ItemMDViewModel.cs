using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Archivator_desktop_WPF_WTS.Contracts.Services;
using Archivator_desktop_WPF_WTS.Contracts.ViewModels;
using Archivator_desktop_WPF_WTS.Helpers;
using Archivator_desktop_WPF_WTS.Models;
using Archivator_desktop_WPF_WTS.Views;
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
        private string _searchString;

        public List<Item> SelectedItems { get; } = new List<Item>();

        public List<Item> Items => _context.Items.ToList();

        public List<Item> ItemsWithoutTickets => _context.Items.Where(item => item.TicketPrintDateTime == null).ToList();

        /// <summary>
        /// Proxy for accessing selected file object. It cannot be accessed directly.
        /// </summary>
        public FileEntity SelectedFile
        {
            get => _selectedFile;
            set => Set(ref _selectedFile, value);
        }

        /// <summary>
        /// Accessor for value used as a filter parameter for searching items. Setter also applies filter to ItemsViewFiltered.
        /// </summary>
        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                ItemsViewFiltered.Filter = o => SearchString == null
                                                || ((Item) o).Name.IndexOf(SearchString,
                                                    StringComparison.OrdinalIgnoreCase) != -1
                                                || ((Item) o).Description.IndexOf(SearchString,
                                                    StringComparison.OrdinalIgnoreCase) != -1;
            }
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
        private ObservableCollection<Item> _items;

        /// <summary>
        /// Allows us to search list of items
        /// </summary>
        public ICollectionView ItemsViewFiltered => CollectionViewSource.GetDefaultView(_items);

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
            _navigationService.NavigateTo(typeof(MainViewModel).FullName,
                new EditModel() {context = _context, editedObject = Selected});
        }

        /// <summary>
        /// Deletes currently selected item, then sets it to first item in list or new item is list is empty.
        /// </summary>
        public void DeleteSelected()
        {
            try
            {
                var result = MessageBox.Show("Are you sure you want to delete this item?", "Confirmation",
                    MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Remove(Selected);
                    _items.Remove(Selected);
                    Selected = _items.FirstOrDefault() ?? new Item();
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An unhandled exception just occurred: " + e.Message, "Exception Occured",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Deletes all marked items.
        /// </summary>
        public void DeleteSelection()
        {
            try
            {
                var result = MessageBox.Show("Are you sure you want to delete all selected items? Selected count: " + SelectedItems.Count, "Confirmation",
                    MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    SelectedItems.ForEach(item =>
                    {
                        _context.Remove(item);
                        _items.Remove(item);
                        if (Selected == item)
                        {
                            Selected = _items.FirstOrDefault() ?? new Item();
                        }
                    });
                    _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An unhandled exception just occurred: " + e.Message, "Exception Occured",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void OnNavigatedTo(object parameter) //can be async
        {
            try
            {
                //Items = new ObservableCollection<Item>(_context.Items.AsNoTracking());
                _items = new ObservableCollection<Item>(_context.Items);

                Selected = _items.FirstOrDefault() ?? new Item();
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show("An unhandled exception just occurred: " + e.Message, "Exception Occured",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("ERROR: No file selected!", "General error", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
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
                    MessageBox.Show("Invalid character/s detected in fileName!\nInvalid characters: " +
                                    Path.GetInvalidFileNameChars());
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
        public void PrintSelectedItem()
        {
            StaticUtilities.PrintObject(Selected);
            Selected.TicketPrintDateTime = DateTime.Now;
            _context.SaveChangesAsync();
        }

        /// <summary>
        /// Print all items in database.
        /// </summary>
        public void PrintAll()
        {
            var result = MessageBox.Show(
                "Are you sure you want to print all items in database? Current item count: " + _context.Items.Count(),
                "Print all items", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                foreach (Item item in _context.Items)
                {
                    StaticUtilities.PrintObject(item);
                    item.TicketPrintDateTime = DateTime.Now;
                }
                _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Prints all items that were not yet printed.
        /// </summary>
        public void PrintMissing()
        {
            var toPrint = ItemsWithoutTickets;

            var result = MessageBox.Show(
                "Are you sure you want to print all items that do not have ticket yet? Current unprinted item count: " + toPrint.Count,
                "Print all items", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                StaticUtilities.PrintMultipleObjects(toPrint);
                toPrint.AsParallel().ForAll(item => item.TicketPrintDateTime = DateTime.Now);
                _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Prints all items that were selected by user.
        /// </summary>
        public void PrintSelection()
        {
            if (SelectedItems.Count == 0)
            {
                MessageBox.Show("No items selected", "Selected items printing", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to print all items you have selected? Current unprinted item count: " + SelectedItems.Count,
                "Print all items", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                StaticUtilities.PrintMultipleObjects(SelectedItems);
                SelectedItems.AsParallel().ForAll(item => item.TicketPrintDateTime = DateTime.Now);
                _context.SaveChangesAsync();
            }
        }

        public void ShowBigViewer()
        {
            new Window {Content = new BigViewer{DataContext = Selected}}.Show();
        }
    }
}