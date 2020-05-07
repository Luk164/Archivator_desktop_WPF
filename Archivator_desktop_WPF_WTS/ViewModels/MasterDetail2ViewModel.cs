using Archivator_desktop_WPF_WTS.Contracts.ViewModels;
using Archivator_desktop_WPF_WTS.Core.Contracts.Services;
using Archivator_desktop_WPF_WTS.Core.Models;
using Archivator_desktop_WPF_WTS.Helpers;
using System.Collections.ObjectModel;
using System.Linq;

namespace Archivator_desktop_WPF_WTS.ViewModels
{
    /// <summary>
    /// Blank template viewModel left for reference purposes.
    /// </summary>
    public class MasterDetail2ViewModel : Observable, INavigationAware
    {
        private readonly ISampleDataService _sampleDataService;
        private SampleOrder _selected;

        public SampleOrder Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        public ObservableCollection<SampleOrder> SampleItems { get; private set; } = new ObservableCollection<SampleOrder>();

        public MasterDetail2ViewModel(ISampleDataService sampleDataService)
        {
            _sampleDataService = sampleDataService;
        }

        public async void OnNavigatedTo(object parameter)
        {
            SampleItems.Clear();

            System.Collections.Generic.IEnumerable<SampleOrder> data = await _sampleDataService.GetMasterDetailDataAsync();

            foreach (SampleOrder item in data)
            {
                SampleItems.Add(item);
            }

            Selected = SampleItems.First();
        }

        public void OnNavigatedFrom()
        {
        }
    }
}
