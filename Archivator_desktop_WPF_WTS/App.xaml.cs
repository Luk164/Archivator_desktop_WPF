using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

using Archivator_desktop_WPF_WTS.Contracts.Services;
using Archivator_desktop_WPF_WTS.Contracts.Views;
using Archivator_desktop_WPF_WTS.Core.Contracts.Services;
using Archivator_desktop_WPF_WTS.Core.Services;
using Archivator_desktop_WPF_WTS.Models;
using Archivator_desktop_WPF_WTS.Services;
using Archivator_desktop_WPF_WTS.ViewModels;
using Archivator_desktop_WPF_WTS.Views;
using ArchivatorDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NavigationService = Archivator_desktop_WPF_WTS.Services.NavigationService;

namespace Archivator_desktop_WPF_WTS
{
    // For more inforation about application lifecyle events see https://docs.microsoft.com/dotnet/framework/wpf/app-development/application-management-overview
    public partial class App : Application
    {
        private IHost _host;
        private IConfiguration _configuration;
        private readonly DbContextOptionsBuilder<ArchivatorDbContext> _builder = new DbContextOptionsBuilder<ArchivatorDbContext>();

        public App()
        {
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // For more information about .NET generic host see  https://docs.microsoft.com/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0
            _host = Host.CreateDefaultBuilder(e.Args)
                    .ConfigureAppConfiguration(c => c.SetBasePath(appLocation))
                    .ConfigureServices(ConfigureServices)
                    .Build();

            await _host.StartAsync();
            var test = _host.Services.GetService<ArchivatorDbContext>();

            InitDatabase();
        }

        private void InitDatabase()
        {
            try
            {
                var dbContext = new ArchivatorDbContext(_builder.Options);
                dbContext.Database.Migrate();
            }
            catch (Exception e)
            {
                var result = MessageBox.Show("An error occured while trying to migrate database: " + e.Message + " please check connection string. If you do not already have SQL Express installed, click yes to install it.", "Exception Occured",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                switch(result)
                {
                    case MessageBoxResult.Yes:
                        System.Diagnostics.Process.Start("~\\SQL2019-SSEI-Expr.exe");
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            
        }

        private void SetupBuilder(DbContextOptionsBuilder builder)
        {
            try
            {
                builder.UseLazyLoadingProxies();
                if (_configuration.GetSection("SQLProvider").Value == "MSSQL")
                {
                    builder.UseSqlServer(_configuration.GetSection(StaticUtilities.CONN_STRING_KEY).Value);
                }
                else if (_configuration.GetSection("SQLProvider").Value == "MYSQL")
                {
                    builder.UseMySql(_configuration.GetSection(StaticUtilities.CONN_STRING_KEY).Value);
                }
                else
                {
                    MessageBox.Show("ERROR: Unknown SQL provider specified! Use either \"MSSQL\" or \"MYSQL\"!\n Provided: " + _configuration.GetSection("SQLProvider").Value, "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("ERROR: Unknown error has occured: " + e.Message, "Unknown error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            
            
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // WTS: Register your services, viewmodels and pages here
            _configuration = context.Configuration; //TODO Find prettier way to do this
            SetupBuilder(_builder);

            // App Host
            services.AddHostedService<ApplicationHostService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            // Services
            services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
            services.AddSingleton<ISystemService, SystemService>();
            services.AddSingleton<IPersistAndRestoreService, PersistAndRestoreService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<ISampleDataService, SampleDataService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddDbContext<ArchivatorDbContext>(SetupBuilder, ServiceLifetime.Transient);

            // Views and ViewModels
            services.AddTransient<IShellWindow, ShellWindow>();
            services.AddTransient<ShellViewModel>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();

            services.AddTransient<ItemMDViewModel>();
            services.AddTransient<ItemMDPage>();

            services.AddTransient<TagsViewModel>();
            services.AddTransient<MasterDetailPage>();

            services.AddTransient<MasterDetail2ViewModel>();
            services.AddTransient<MasterDetail2Page>();

            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();

            // Configuration
            services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            _host = null;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Occured", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }
    }
}
