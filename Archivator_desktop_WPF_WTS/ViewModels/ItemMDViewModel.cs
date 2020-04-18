using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Archivator_desktop_WPF_WTS.Contracts.Services;
using Archivator_desktop_WPF_WTS.Contracts.ViewModels;
using Archivator_desktop_WPF_WTS.Core.Contracts.Services;
using Archivator_desktop_WPF_WTS.Core.Models;
using Archivator_desktop_WPF_WTS.Helpers;
using Archivator_desktop_WPF_WTS.Models;
using ArchivatorDb;
using ArchivatorDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace Archivator_desktop_WPF_WTS.ViewModels
{
    public class ItemMDViewModel : Observable, INavigationAware
    {
        private Item _selected;
        private readonly ArchivatorDbContext _context;
        private readonly INavigationService _navigationService;

        public Item Selected
        {
            get => _selected;
            set => Set(ref _selected, value);
        }

        public IList<Tag> Tags { get; }

        public ObservableCollection<Item> Items { get; private set; }

        public ItemMDViewModel(ArchivatorDbContext context, INavigationService navigationService)
        {
            _context = context;
            Tags = context.Tags.ToList();
            _navigationService = navigationService;
        }

        public void EditSelected()
        {
            _navigationService.NavigateTo(typeof(MainViewModel).FullName, new EditModel(){context = _context, editedObject = Selected});
        }

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
    }
}
