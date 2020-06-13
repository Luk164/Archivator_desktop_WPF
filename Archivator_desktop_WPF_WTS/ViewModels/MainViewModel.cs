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
using System;
using System.Collections.Generic;
using System.Linq;

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

        public ArchivatorDbContext _context { get; private set; }

        /// <summary>
        /// Provides access to list of all tags in database
        /// </summary>
        public IList<Tag> Tags { get; private set; }

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
        /// Saves changes to database, checks if internal ID not already present
        /// </summary>
        public void SaveChanges()
        {
            if (CurrItem.AlternateKey != CurrItem.Category + "-" + CurrItem.InternalId + "-" + CurrItem.SubCategory)
            {
                throw new NotImplementedException("ERROR: Changes to alternate key composition were made. Please make appropriate adjustments!");
            }

            if (_context.Items.Any(i =>
                i.InternalId == CurrItem.InternalId && i.Category == CurrItem.Category && i.SubCategory == CurrItem.SubCategory) && CurrItem.Id == 0)
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

            _context.DisposeAsync(); //get rid of old context
            _context = model.context; //load new items context
            CurrItem = item; //load new item
            CurrItem.ModifyDateTime = DateTime.Now; //save modification time
            Tags = _context.Tags.ToList(); //fixes a bug with editing tags for eventEntity

            foreach (EventEntity eventEntity in CurrItem.Events)
            {
                foreach (Event2Tag event2Tag in eventEntity.Tags)
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
            EventEntity newEntity = _context.CreateProxy<EventEntity>();
            _context.Add(newEntity);
            newEntity.ParenItem = CurrItem;
            return newEntity;
        }

        /// <summary>
        /// Creates new tag and it adds to database. Does not save changes.
        /// </summary>
        /// <param name="name">Name of new Tag</param>
        public void CreateTag(string name)
        {
            var newTag = _context.CreateProxy<Tag>();
            newTag.Name = name;

            _context.Add(newTag);
            Tags.Add(newTag);
        }

        /// <summary>
        /// Opens description editor on separate window
        /// </summary>
        public void ShowBigEditor()
        {
            new Window {Content = new BigEditor{DataContext = CurrItem}}.Show();
        }
    }
}
