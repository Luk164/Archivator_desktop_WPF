﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Archivator_desktop_WPF_WTS.Contracts.Services;
using Archivator_desktop_WPF_WTS.Contracts.ViewModels;
using Archivator_desktop_WPF_WTS.Helpers;
using Archivator_desktop_WPF_WTS.Models;
using ArchivatorDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using OfficeOpenXml;

// ReSharper disable UnusedAutoPropertyAccessor.Local Disabled because structs need to be accessible to EPPlus library for xlsx generation

namespace Archivator_desktop_WPF_WTS.ViewModels
{
    /// <summary>
    /// View-model for settings page. It changes settings for the entire app and allows exporting database.
    /// </summary>
    public class SettingsViewModel : Observable, INavigationAware
    {
        private readonly AppConfig _config;
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly ISystemService _systemService;
        private readonly IApplicationInfoService _applicationInfoService;
        private AppTheme _theme;
        private string _versionDescription;
        private ICommand _setThemeCommand;
        private ICommand _privacyStatementCommand;
        private readonly ArchivatorDbContext _context;

        /// <summary>
        /// Used as a representation of Item for xlsx generation purposes
        /// </summary>
        private struct Simple_item
        {
            public int ItemId { get; set; }
            public string SecondaryId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string RelatedItems { get; set; }
            public string Events { get; set; }
            public string Files { get; set; }
            public string UserId { get; set; }
            public string CreateDateTime { get; set; }
        }

        /// <summary>
        /// Used as a representation of FileEntity for xlsx generation purposes
        /// </summary>
        private struct Simple_file
        {
            public int Id { get; set; }
            public string FileName { get; set; }
            public string Description { get; set; }
            public int ParentItem { get; set; }
        }

        /// <summary>
        /// Used as a representation of EventEntity for xlsx generation purposes
        /// </summary>
        private struct Simple_event
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Date { get; set; }
            public string AuxDate { get; set; }
            public string Location { get; set; }
            public string Tags { get; set; }
            public int ParenItem { get; set; }
            public int UserId { get; set; }
        }

        /// <summary>
        /// Used as a representation of Tag for xlsx generation purposes
        /// </summary>
        private struct Simple_tag
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Events { get; set; }
        }

        private struct Simple_Conn_Item2Item
        {
            public int ItemId1 { get; set; }
            public int ItemId2 { get; set; }
        }

        private struct Simple_Conn_Event2Tag
        {
            public int EventId { get; set; }
            public int TagId { get; set; }
        }

        public AppTheme Theme
        {
            get => _theme;
            set => Set(ref _theme, value);
        }

        public string VersionDescription
        {
            get => _versionDescription;
            private set => Set(ref _versionDescription, value);
        }

        /// <summary>
        /// Command that sets theme for the entire application
        /// </summary>
        public ICommand SetThemeCommand => _setThemeCommand ??= new RelayCommand<string>(OnSetTheme);

        /// <summary>
        /// Command that opens web browser using privacy statement link
        /// </summary>
        public ICommand PrivacyStatementCommand => _privacyStatementCommand ??= new RelayCommand(OnPrivacyStatement);

        public SettingsViewModel(IOptions<AppConfig> config, IThemeSelectorService themeSelectorService,
            ISystemService systemService, IApplicationInfoService applicationInfoService, ArchivatorDbContext context)
        {
            _config = config.Value;
            _themeSelectorService = themeSelectorService;
            _systemService = systemService;
            _applicationInfoService = applicationInfoService;
            _context = context;
        }

        public void OnNavigatedTo(object parameter)
        {
            VersionDescription = $"Archivator_desktop - {_applicationInfoService.GetVersion()}";
            Theme = _themeSelectorService.GetCurrentTheme();
        }

        public void OnNavigatedFrom()
        {
        }

        private void OnSetTheme(string themeName)
        {
            var theme = (AppTheme) Enum.Parse(typeof(AppTheme), themeName);
            _themeSelectorService.SetTheme(theme);
        }

        private void OnPrivacyStatement()
            => _systemService.OpenInWebBrowser(_config.PrivacyStatement);

        /// <summary>
        /// Opens a dialog for saving database contents, then if confirmed generates the file and saves to specified location.
        /// </summary>
        public async void ExportDb()
        {
            SaveFileDialog _SD = new SaveFileDialog
            {
                Filter = "ExcelFile (*.xlsx)|*.xlsx|Show All Files (*.*)|*.*",
                FileName = "ArchivatorDbExport",
                Title = "Save As"
            };

            if (_SD.ShowDialog() == true)
            {
                ExcelPackage excel = new ExcelPackage();

                ExcelWorksheet ws_items = excel.Workbook.Worksheets.Add("Items");
                ws_items.Cells[1, 1].LoadFromCollection(await GenerateSimpleItemList(), true);

                ExcelWorksheet ws_files = excel.Workbook.Worksheets.Add("Files");
                ws_files.Cells[1, 1].LoadFromCollection(await GenerateSimpleFileList(), true);

                ExcelWorksheet ws_events = excel.Workbook.Worksheets.Add("Events");
                ws_events.Cells[1, 1].LoadFromCollection(await GenerateSimpleEventList(), true);

                ExcelWorksheet ws_tags = excel.Workbook.Worksheets.Add("Tags");
                ws_tags.Cells[1, 1].LoadFromCollection(await GenerateSimpleTagList(), true);

                var ws_item2item = excel.Workbook.Worksheets.Add("item2item");
                ws_item2item.Cells[1, 1].LoadFromCollection(await GenerateSimple_Conn_Item2Item(), true);

                var ws_event2tag = excel.Workbook.Worksheets.Add("event2tag");
                ws_event2tag.Cells[1, 1].LoadFromCollection(await GenerateSimple_Conn_Event2Tag(), true);

                //Write the file to the disk
                FileInfo fi = new FileInfo(_SD.FileName);
                excel.SaveAs(fi);
            }
            else
            {
                MessageBox.Show("Export has been cancelled by user!", "Export cancelled", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Generates list of simple representations of all Items in database.
        /// </summary>
        /// <returns>List of simple representations</returns>
        private async Task<List<Simple_item>> GenerateSimpleItemList()
        {
            return (from item in await _context.Items.ToListAsync()
                select new Simple_item
                {
                    Name = item.Name,
                    CreateDateTime = item.CreateDateTime.ToShortDateString(),
                    Description = item.Description,
                    Events = item.Events.Aggregate("", (current, o) => current + ";" + o.Id),
                    Files = item.Files.Aggregate("", (current, o) => current + ";" + o.Id),
                    ItemId = item.Id,
                    SecondaryId = item.AlternateKey,
                    RelatedItems = item.RelatedItems.Aggregate("", (current, o) => current + ";" + o.ToId),
                    UserId = item.UserId.ToString()
                }).ToList();
        }

        /// <summary>
        /// Generates list of simple representations of all FileEntities in database.
        /// </summary>
        /// <returns>List of simple representations</returns>
        private async Task<List<Simple_file>> GenerateSimpleFileList()
        {
            return (from file in await _context.Files.ToListAsync()
                select new Simple_file
                {
                    FileName = file.FileName,
                    Id = file.Id,
                    Description = file.Description,
                    ParentItem = file.ParentItem.Id
                }).ToList();
        }

        /// <summary>
        /// Generates list of simple representations of all EventEntities in database.
        /// </summary>
        /// <returns>List of simple representations</returns>
        private async Task<List<Simple_event>> GenerateSimpleEventList()
        {
            return (from eventEntity in await _context.Events.ToListAsync()
                select new Simple_event
                {
                    Id = eventEntity.Id,
                    AuxDate = eventEntity.AuxDate,
                    Date = eventEntity.Date.ToShortDateString(),
                    Description = eventEntity.Description,
                    Location = eventEntity.Location,
                    Name = eventEntity.Name,
                    ParenItem = eventEntity.ParenItem.Id,
                    Tags = eventEntity.Tags.Aggregate("", (current, o) => current + ";" + o.TagId),
                    UserId = eventEntity.UserId
                }).ToList();
        }

        /// <summary>
        /// Generates list of simple representations of all Tags in database.
        /// </summary>
        /// <returns>List of simple representations</returns>
        private async Task<List<Simple_tag>> GenerateSimpleTagList()
        {
            return (from tag in await _context.Tags.ToListAsync()
                select new Simple_tag
                {
                    Id = tag.Id,
                    Events = tag.Events.Aggregate("", (current, o) => current + ";" + o.EventId),
                    Name = tag.Name
                }).ToList();
        }

        private async Task<List<Simple_Conn_Item2Item>> GenerateSimple_Conn_Item2Item()
        {
            return (from item in await _context.Items.ToListAsync() 
                from item2Item in item.RelatedItems select new Simple_Conn_Item2Item {ItemId1 = item2Item.FromId, ItemId2 = item2Item.ToId}).ToList();
        }

        private async Task<List<Simple_Conn_Event2Tag>> GenerateSimple_Conn_Event2Tag()
        {
            return (from eventEntity in await _context.Events.ToListAsync() 
                from event2Tag in eventEntity.Tags select new Simple_Conn_Event2Tag {EventId = event2Tag.EventId, TagId = event2Tag.TagId}).ToList();
        }

        public async Task PurgeDatabase()
        {
            var choice = MessageBox.Show("Are you sure you want to delete the entire database? There is no way back!",
                "!!!WARNING!!!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (choice == MessageBoxResult.Yes)
            {
                await _context.Database.EnsureDeletedAsync();
                await _context.Database.MigrateAsync();
                MessageBox.Show("Database deleted, program will now exit", "Database deleted", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Application.Current.Shutdown();
            }
            else
            {
                MessageBox.Show("Database purge cancelled", "Cancelled by user", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}