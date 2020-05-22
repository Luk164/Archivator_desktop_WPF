using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Archivator_desktop_WPF_WTS.Contracts.ViewModels;
using Archivator_desktop_WPF_WTS.Helpers;
using ArchivatorDb;
using ArchivatorDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace Archivator_desktop_WPF_WTS.ViewModels
{
    /// <summary>
    /// View-model of tags page. Used to view, add and delete Tag objects from database.
    /// </summary>
    public class TagsViewModel : Observable, INavigationAware
    {
        private readonly ArchivatorDbContext _context;

        private Tag _selected;
        private ObservableCollection<Tag> Tags { get; }

        public TagsViewModel(ArchivatorDbContext context)
        {
            _context = context;
            Tags = new ObservableCollection<Tag>(_context.Tags);
            Selected = Tags.FirstOrDefault() ?? new Tag();
        }

        public Tag Selected
        {
            get => _selected;
            set => Set(ref _selected, value);
        }

        private string _searchString;

        /// <summary>
        /// Value used as a filter parameter for searching items. Setter also applies filter to ItemsViewFiltered.
        /// </summary>
        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                TagsViewFiltered.Filter = o => SearchString == null
                                                || ((Tag) o).Name.IndexOf(SearchString,
                                                    StringComparison.OrdinalIgnoreCase) != -1;
            }
        }

        /// <summary>
        /// Allows us to search list of items
        /// </summary>
        public ICollectionView TagsViewFiltered => CollectionViewSource.GetDefaultView(Tags);

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
                    Tags.Remove(Selected);
                    Selected = Tags.FirstOrDefault() ?? new Tag();
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
            var newTag = _context.CreateProxy<Tag>();
            newTag.Name = name;

            _context.Add(newTag);
            _context.SaveChangesAsync();
            Tags.Add(newTag);
        }

        public void OnNavigatedTo(object parameter)
        {
            
        }

        public void OnNavigatedFrom()
        {
        }
    }
}
