using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Archivator_desktop_WPF_WTS.Contracts.ViewModels;
using Archivator_desktop_WPF_WTS.Helpers;
using Archivator_desktop_WPF_WTS.Models;
using Archivator_desktop_WPF_WTS.Views;
using ArchivatorDb;
using ArchivatorDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace Archivator_desktop_WPF_WTS.ViewModels
{
    /// <summary>
    /// ViewModel for Main page. Used for adding and editing items
    /// </summary>
    public class MainViewModel : Observable, INavigationAware
    {
        /// <summary>
        /// Stores a reference to currently created/edited item
        /// </summary>
        public Item CurrItem { get; set; }

        private ArchivatorDbContext _context;

        /// <summary>
        /// Provides access to list of all tags in database
        /// </summary>
        public IList<Tag> Tags { get; }

        public static IEnumerable<char> Alphabet => Item.CategoryList;

        /// <summary>
        /// Constructor for MainViewModel
        /// </summary>
        /// <param name="context">ArchivatorDbContext used to interact with database</param>
        public MainViewModel(ArchivatorDbContext context)
        {
            _context = context ?? throw new Exception("ERROR: DB context is null!");
            CurrItem = _context.CreateProxy<Item>();
            CurrItem.InternalId = _context.Items.Count() + 1;
            _context.Add(CurrItem);
            Tags = _context.Tags.ToList();
        }

        /// <summary>
        /// Saves changes to database, checks if internal ID not already present.
        /// </summary>
        public void SaveChanges()
        {
            if (_context.Items.Any(i =>
                i.InternalId == CurrItem.InternalId))
            {
                MessageBox.Show(
                    "ERROR: Item with this internal ID already exists in database! Please choose a different one!",
                    "Id not unique", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            _context.SaveChanges();
        }

        public void OnNavigatedTo(object parameter)
        {
            if (!(parameter is EditModel model) || !(model.editedObject is Item item)) return;

            _context.DisposeAsync();
            _context = model.context;
            CurrItem = item;
            CurrItem.ModifyDateTime = DateTime.Now;

            foreach (var eventEntity in CurrItem.Events)
            {
                foreach (var event2Tag in eventEntity.Tags)
                {
                    eventEntity.SelectedTags.Add(event2Tag.Tag);
                }
            }
        }

        public void OnNavigatedFrom()
        {
        }

        /// <summary>
        /// Creates new entity proxy using ArchivatorDbContext _context and tracks it
        /// </summary>
        /// <returns>New tracked entity proxy object</returns>
        public EventEntity GetNewEventEntity()
        {
            var newEntity = _context.CreateProxy<EventEntity>();
            _context.Add(newEntity);
            newEntity.ParenItem = CurrItem;
            return newEntity;
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
            Tags.Add(newTag);
        }

        public void ShowBigEditor()
        {
            Window test = new Window {Content = new BigEditor{DataContext = CurrItem}};
            test.Show();
        }
    }
}
