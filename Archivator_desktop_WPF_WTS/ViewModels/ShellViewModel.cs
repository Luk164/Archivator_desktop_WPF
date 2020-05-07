using Archivator_desktop_WPF_WTS.Contracts.Services;
using Archivator_desktop_WPF_WTS.Helpers;
using Archivator_desktop_WPF_WTS.Strings;
using MahApps.Metro.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Archivator_desktop_WPF_WTS.ViewModels
{
    public class ShellViewModel : Observable
    {
        private readonly INavigationService _navigationService;
        private HamburgerMenuItem _selectedMenuItem;
        private HamburgerMenuItem _selectedOptionsMenuItem;
        private RelayCommand _goBackCommand;
        private ICommand _menuItemInvokedCommand;
        private ICommand _optionsMenuItemInvokedCommand;
        private ICommand _loadedCommand;
        private ICommand _unloadedCommand;

        public HamburgerMenuItem SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set { Set(ref _selectedMenuItem, value); }
        }

        public HamburgerMenuItem SelectedOptionsMenuItem
        {
            get { return _selectedOptionsMenuItem; }
            set { Set(ref _selectedOptionsMenuItem, value); }
        }

        // TODO WTS: Change the icons and titles for all HamburgerMenuItems here.
        public ObservableCollection<HamburgerMenuItem> MenuItems { get; } =
            new ObservableCollection<HamburgerMenuItem>()
            {
                new HamburgerMenuGlyphItem()
                    {Label = Resources.ShellMainPage, Glyph = "\uECC8", TargetPageType = typeof(MainViewModel)},
                new HamburgerMenuGlyphItem()
                    {Label = Resources.ShellItemMDPage, Glyph = "\uE71D", TargetPageType = typeof(ItemMDViewModel)},
                new HamburgerMenuGlyphItem()
                    {Label = Resources.ShellTagsMDPage, Glyph = "\uE8EC", TargetPageType = typeof(TagsViewModel)},
                //new HamburgerMenuGlyphItem() { Label = Resources.ShellMasterDetail2Page, Glyph = "\uE8A5", TargetPageType = typeof(MasterDetail2ViewModel) },
            };

        public ObservableCollection<HamburgerMenuItem> OptionMenuItems { get; } =
            new ObservableCollection<HamburgerMenuItem>()
            {
                new HamburgerMenuGlyphItem()
                    {Label = Resources.ShellSettingsPage, Glyph = "\uE713", TargetPageType = typeof(SettingsViewModel)}
            };

        public RelayCommand GoBackCommand => _goBackCommand ??= new RelayCommand(OnGoBack, CanGoBack);

        public ICommand MenuItemInvokedCommand => _menuItemInvokedCommand ??= new RelayCommand(OnMenuItemInvoked);

        public ICommand OptionsMenuItemInvokedCommand =>
            _optionsMenuItemInvokedCommand ??= new RelayCommand(OnOptionsMenuItemInvoked);

        public ICommand LoadedCommand => _loadedCommand ??= new RelayCommand(OnLoaded);

        public ICommand UnloadedCommand => _unloadedCommand ??= new RelayCommand(OnUnloaded);

        public ShellViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _navigationService.Navigated += OnNavigated;
        }

        private void OnLoaded()
        {
        }

        private void OnUnloaded()
        {
            _navigationService.Navigated -= OnNavigated;
        }

        private bool CanGoBack()
            => _navigationService.CanGoBack;

        private void OnGoBack()
            => _navigationService.GoBack();

        private void OnMenuItemInvoked()
            => NavigateTo(SelectedMenuItem.TargetPageType);

        private void OnOptionsMenuItemInvoked()
            => NavigateTo(SelectedOptionsMenuItem.TargetPageType);

        private void NavigateTo(Type targetViewModel)
        {
            if (targetViewModel != null)
            {
                _navigationService.NavigateTo(targetViewModel.FullName);
            }
        }

        private void OnNavigated(object sender, string viewModelName)
        {
            HamburgerMenuItem item = MenuItems
                .OfType<HamburgerMenuItem>()
                .FirstOrDefault(i => viewModelName == i.TargetPageType?.FullName);
            if (item != null)
            {
                SelectedMenuItem = item;
            }
            else
            {
                SelectedOptionsMenuItem = OptionMenuItems
                    .OfType<HamburgerMenuItem>()
                    .FirstOrDefault(i => viewModelName == i.TargetPageType?.FullName);
            }

            GoBackCommand.OnCanExecuteChanged();
        }
    }
}