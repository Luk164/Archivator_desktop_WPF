using System;
using System.Windows.Controls;

namespace Archivator_desktop_WPF_WTS.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);

        Page GetPage(string key);
    }
}
