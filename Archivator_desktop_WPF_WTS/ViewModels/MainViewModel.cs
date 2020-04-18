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

        public void AddTag(int TagId)
        {
            if (SelectedEvent == null) return;

            _context.Add(new Event2Tag(){Tag = _context.Tags.Find(TagId), Event = SelectedEvent});
        }

        public void RemoveTag(int TagId)
        {
            if (SelectedEvent == null) return;
            _context.Remove(SelectedEvent.Tags.First(tag => tag.TagId == TagId));
            SelectedEvent.Tags.Remove(SelectedEvent.Tags.First(tag => tag.TagId == TagId));
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void OnNavigatedTo(object parameter)
        {
            if (parameter is EditModel model && model.editedObject is Item item)
            {
                _context.DisposeAsync();
                _context = model.context;
                CurrItem = item;
            }
        }

        public void OnNavigatedFrom()
        {
        }
    }
}
