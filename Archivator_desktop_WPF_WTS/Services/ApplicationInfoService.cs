using System;
using System.Reflection;
using Archivator_desktop_WPF_WTS.Contracts.Services;

using Windows.ApplicationModel;

namespace Archivator_desktop_WPF_WTS.Services
{
    public class ApplicationInfoService : IApplicationInfoService
    {
        public ApplicationInfoService()
        {
        }

        public Version GetVersion()
        {
            try
            {
                if (OSVersionHelper.WindowsVersionHelper.HasPackageIdentity)
                {
                    var version = Package.Current.Id.Version;
                    return new Version(version.Major, version.Minor, version.Build, version.Revision);
                }
                else
                {
                    var v = Assembly.GetExecutingAssembly().GetName().Version;
                    return new Version(v.Major, v.Minor, v.Build, v.Revision);
                }

                //// MSIX distribuition
                //// Setup the App Version in Archivator_desktop_WPF_WTS.Packaging > Package.appxmanifest > Packaging > PackageVersion
                //var version = Package.Current.Id.Version;
                //return new Version(version.Major, version.Minor, version.Build, version.Revision);
            }
            catch (Exception)
            {
                //Console.WriteLine("Debug mode detected!");
                return new Version(6, 6, 6);
            }
        }
    }
}
