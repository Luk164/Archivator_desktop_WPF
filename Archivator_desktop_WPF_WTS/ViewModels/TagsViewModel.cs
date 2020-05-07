using Archivator_desktop_WPF_WTS.Contracts.ViewModels;
using Archivator_desktop_WPF_WTS.Helpers;
using ArchivatorDb;
using ArchivatorDb.Entities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Archivator_desktop_WPF_WTS.ViewModels
{
    /// <summary>
    /// View-model of tags page. Used to view, add and delete Tag objects from database.
    /// </summary>
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

        /// <summary>
        /// Opens a confirmation dialog and if confirmed deletes currently selected tag.
        /// </summary>
        public void DeleteSelected()
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this editedObject?", "Confirmation", MessageBoxButton.YesNo);

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

        /// <summary>
        /// Saves changes to tags to database
        /// </summary>
        public void SubmitChanges()
        {
            _context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates new tag, adds to database and saves changes
        /// </summary>
        /// <param name="name"></param>
        public void CreateTag(string name)
        {
            _context.Add(new Tag() { Name = name });
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
