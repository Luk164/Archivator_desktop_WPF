using Archivator_desktop_WPF_WTS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Archivator_desktop_WPF_WTS.Core.Contracts.Services
{
    public interface ISampleDataService
    {
        Task<IEnumerable<SampleOrder>> GetMasterDetailDataAsync();
    }
}
