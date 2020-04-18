using System.Collections.Generic;
using System.Threading.Tasks;

using Archivator_desktop_WPF_WTS.Core.Models;

namespace Archivator_desktop_WPF_WTS.Core.Contracts.Services
{
    public interface ISampleDataService
    {
        Task<IEnumerable<SampleOrder>> GetMasterDetailDataAsync();
    }
}
