using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows.Input;

using Archivator_desktop_WPF_WTS.Contracts.Services;
using Archivator_desktop_WPF_WTS.Contracts.ViewModels;
using Archivator_desktop_WPF_WTS.Helpers;
using Archivator_desktop_WPF_WTS.Models;
using ArchivatorDb;
using ArchivatorDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using OfficeOpenXml;

namespace Archivator_desktop_WPF_WTS.ViewModels
{
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

        private ArchivatorDbContext _context;

        struct Simple_item
        {
            public int ItemId { get; set;}
            public string Name { get; set;}
            public string Description { get; set;}
            public string RelatedItems { get; set;}
            public string Events { get; set;}
            public string Files { get; set;}
            public string UserId { get; set;}
            public string CreateDateTime { get; set;}
        }

        private struct Simple_file
        {
            public int Id { get; set;}
            public string FileName { get; set;}
            public string Description { get; set;}
            public int ParentItem { get; set;}
        }

        private struct Simple_event
        {
            public int Id { get; set;}
            public string Name { get; set;}
            public string Description { get; set;}
            public string Date { get; set;}
            public string AuxDate { get; set;}
            public string Location { get; set;}
            public string Tags { get; set;}
            public int ParenItem { get; set;}
            public int UserId { get; set;}
        }

        private struct Simple_tag
        {
            public int Id { get; set;}
            public string Name { get; set;}
            public string Events { get; set;}
        }

        public AppTheme Theme
        {
            get { return _theme; }
            set { Set(ref _theme, value); }
        }

        public string VersionDescription
        {
            get { return _versionDescription; }
            set { Set(ref _versionDescription, value); }
        }

        public ICommand SetThemeCommand => _setThemeCommand ??= new RelayCommand<string>(OnSetTheme);
        public ICommand PrivacyStatementCommand => _privacyStatementCommand ??= new RelayCommand(OnPrivacyStatement);

        public SettingsViewModel(IOptions<AppConfig> config, IThemeSelectorService themeSelectorService, ISystemService systemService, IApplicationInfoService applicationInfoService, ArchivatorDbContext context)
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
            var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
            _themeSelectorService.SetTheme(theme);
        }

        private void OnPrivacyStatement()
            => _systemService.OpenInWebBrowser(_config.PrivacyStatement);

        public async void ExportDb()
        {
            ExcelPackage excel = new ExcelPackage();

            var ws_items = excel.Workbook.Worksheets.Add("Items");
            ws_items.Cells[1, 1].LoadFromCollection(await GenerateSimpleItemList(), true);

            var ws_files = excel.Workbook.Worksheets.Add("Files");
            ws_files.Cells[1, 1].LoadFromCollection(await GenerateSimpleFileList(), true);

            var ws_events = excel.Workbook.Worksheets.Add("Events");
            ws_events.Cells[1, 1].LoadFromCollection(await GenerateSimpleEventList(), true);

            var ws_tags = excel.Workbook.Worksheets.Add("Tags");
            ws_tags.Cells[1, 1].LoadFromCollection(await GenerateSimpleTagList(), true);


            SaveFileDialog _SD = new SaveFileDialog
            {
                Filter = "ExcelFile (*.xlsx)|*.xlsx|Show All Files (*.*)|*.*",
                FileName = "ArchivatorDbExport",
                Title = "Save As"
            };

            if (_SD.ShowDialog() == true)
            {
                //Write the file to the disk
                FileInfo fi = new FileInfo(_SD.FileName);
                excel.SaveAs(fi);
            }
        }

        private async Task<List<Simple_item>> GenerateSimpleItemList()
        {
            return (from item in await _context.Items.ToListAsync()
                select new Simple_item()
                {
                    Name = item.Name,
                    CreateDateTime = item.CreateDateTime.ToLongDateString(),
                    Description = item.Description,
                    Events = item.Events.Aggregate("", (current, o) => current + ";" + o.Id),
                    Files = item.Files.Aggregate("", (current, o) => current + ";" + o.Id),
                    ItemId = item.Id,
                    RelatedItems = item.RelatedItems.Aggregate("", (current, o) => current + ";" + o.ToId),
                    UserId = item.UserId.ToString()
                }).ToList();
        }

        private async Task<List<Simple_file>> GenerateSimpleFileList()
        {
            return (from file in await _context.Files.ToListAsync()
                select new Simple_file()
                {
                    FileName = file.FileName,
                    Id = file.Id,
                    Description = file.Description,
                    ParentItem = file.ParentItem.Id
                }).ToList();
        }

        private async Task<List<Simple_event>> GenerateSimpleEventList()
        {
            return (from eventEntity in await _context.Events.ToListAsync()
                select new Simple_event()
                {
                    Id = eventEntity.Id,
                    AuxDate = eventEntity.AuxDate,
                    Date = eventEntity.Date.ToLongDateString(),
                    Description = eventEntity.Description,
                    Location = eventEntity.Location,
                    Name = eventEntity.Name,
                    ParenItem = eventEntity.ParenItem.Id,
                    Tags = eventEntity.Tags.Aggregate("", (current, o) => current + ";" + o.TagId),
                    UserId = eventEntity.UserId
                }).ToList();
        }

        private async Task<List<Simple_tag>> GenerateSimpleTagList()
        {
            return (from tag in await _context.Tags.ToListAsync()
                select new Simple_tag()
                {
                    Id = tag.Id,
                    Events = tag.Events.Aggregate("", (current, o) => current + ";" + o.EventId),
                    Name = tag.Name
                }).ToList();
        }
    }
}
