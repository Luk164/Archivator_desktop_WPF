using System;
using System.Collections.Generic;
using System.Linq;
using Archivator_desktop_WPF_WTS.Contracts.ViewModels;
using Archivator_desktop_WPF_WTS.Helpers;
using Archivator_desktop_WPF_WTS.Models;
using ArchivatorDb;
using ArchivatorDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace Archivator_desktop_WPF_WTS.ViewModels
{
    public class MainViewModel : Observable, INavigationAware
    {
        public Item CurrItem { get; set; }
        private ArchivatorDbContext _context { get; set; }
        public IList<Tag> Tags { get; }
        public EventEntity SelectedEvent { get; set; }

        public MainViewModel(ArchivatorDbContext context)
        {
            _context = context ?? throw new Exception("ERROR: DB context is null!");
            CurrItem = _context.CreateProxy<Item>();
            _context.Add(CurrItem);
            Tags = _context.Tags.ToList();
        }

        public void SyncEventWithTags(EventEntity Event, List<Tag> listOfSelectedTags)
        {
            //var eventEntity = _context.Events.Find(EventId);

            foreach (var tag in listOfSelectedTags)
            {
                //already inside
                if (Event.Tags.Any(event2Tag => event2Tag.Tag == tag))
                {
                    continue;
                }

                Event.Tags.Add(new Event2Tag(){Event = Event, Tag = tag});
            }

            //remove all tags that are not supposed to be there
            foreach (var event2Tag in Event.Tags.ToList().Where(event2Tag => !listOfSelectedTags.Contains(event2Tag.Tag)))
            {
                Event.Tags.Remove(event2Tag);
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void OnNavigatedTo(object parameter)
        {
            if (!(parameter is EditModel model) || !(model.editedObject is Item item)) return;

            _context.DisposeAsync();
            _context = model.context;
            CurrItem = item;

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

        public EventEntity GetNewEventEntity()
        {
            var newEntity = _context.CreateProxy<EventEntity>();
            _context.Add(newEntity);
            newEntity.ParenItem = CurrItem;
            return newEntity;
        }
    }
}
