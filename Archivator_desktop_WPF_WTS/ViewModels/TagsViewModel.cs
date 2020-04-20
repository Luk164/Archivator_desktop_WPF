using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Archivator_desktop_WPF_WTS.Contracts.ViewModels;
using Archivator_desktop_WPF_WTS.Core.Contracts.Services;
using Archivator_desktop_WPF_WTS.Core.Models;
using Archivator_desktop_WPF_WTS.Helpers;
using ArchivatorDb;
using ArchivatorDb.Entities;

namespace Archivator_desktop_WPF_WTS.ViewModels
{
    public class TagsViewModel : Observable, INavigationAware
    {
        private readonly ArchivatorDbContext _context;

        private Tag _selected;
        public ObservableCollection<Tag> Items { get; private set; }

        public TagsViewModel(ArchivatorDbContext context)
        {
            _context = context;
        }

        public Tag Selected
        {
            get => _selected;
            set => Set(ref _selected, value);
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
                    Selected = Items.FirstOrDefault() ?? new Tag();
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An unhandled exception just occurred: " + e.Message, "Exception Occured", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void SubmitChanges()
        {
            _context.SaveChangesAsync();
        }

        public void CreateTag(string name)
        {
            _context.Add(new Tag(){Name = name});
            _context.SaveChangesAsync();
        }

        public void OnNavigatedTo(object parameter)
        {
            Items = new ObservableCollection<Tag>(_context.Tags);

            Selected = Items.FirstOrDefault() ?? new Tag();
        }

        public void OnNavigatedFrom()
        {
        }
    }
}
