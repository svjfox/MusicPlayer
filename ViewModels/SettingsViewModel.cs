using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace MusicPlayer.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _selectedTheme = "System Default";

        [ObservableProperty]
        private string _selectedAudioQuality = "High";

        public string[] AvailableThemes { get; } =
            ["System Default", "Light", "Dark"];

        public string[] AudioQualities { get; } =
            ["Low", "Medium", "High"];

        public string CacheSize { get; } = "125 MB";
        public string AppVersion { get; } = "MusicPlayer v1.0.0";

        [RelayCommand]
        private async Task ClearCache()
        {
            try
            {
                IsBusy = true;
                // Логика очистки кэша
                await Task.Delay(500); // Имитация
                await Shell.Current.DisplayAlert("Success", "Cache cleared", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task RateApp()
        {
            try
            {
                await Launcher.OpenAsync("market://details?id=com.yourcompany.musicplayer");
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "Cannot open app store", "OK");
            }
        }
    }
}