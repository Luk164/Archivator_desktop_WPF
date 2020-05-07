using Archivator_desktop_WPF_WTS.Contracts.Services;
using Archivator_desktop_WPF_WTS.Core.Contracts.Services;
using Archivator_desktop_WPF_WTS.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.IO;

namespace Archivator_desktop_WPF_WTS.Services
{
    public class PersistAndRestoreService : IPersistAndRestoreService
    {
        private readonly IFileService _fileService;
        private readonly AppConfig _config;
        private readonly string _localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public PersistAndRestoreService(IFileService fileService, IOptions<AppConfig> config)
        {
            _fileService = fileService;
            _config = config.Value;
        }

        public void PersistData()
        {
            if (App.Current.Properties != null)
            {
                string folderPath = Path.Combine(_localAppData, _config.ConfigurationsFolder);
                string fileName = _config.AppPropertiesFileName;
                _fileService.Save(folderPath, fileName, App.Current.Properties);
            }
        }

        public void RestoreData()
        {
            string folderPath = Path.Combine(_localAppData, _config.ConfigurationsFolder);
            string fileName = _config.AppPropertiesFileName;
            IDictionary properties = _fileService.Read<IDictionary>(folderPath, fileName);
            if (properties != null)
            {
                foreach (DictionaryEntry property in properties)
                {
                    App.Current.Properties.Add(property.Key, property.Value);
                }
            }
        }
    }
}
