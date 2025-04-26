using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MusicPlayer;
using MusicPlayer.Services;
using MusicPlayer.ViewModels;
using MusicPlayer.Views;
using Plugin.Maui.Audio;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement() // Убедитесь, что пакет установлен
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
            });

        // Регистрация сервисов  
        builder.Services.AddSingleton<IAudioManager>(AudioManager.Current); // Используем AudioManager из Plugin.Maui.Audio
        builder.Services.AddSingleton<IAudioService, AudioService>(); // Регистрация IAudioService
        builder.Services.AddSingleton<IEqualizerService, EqualizerService>();
        builder.Services.AddSingleton<IDataService, DataService>();

#if ANDROID
        // Регистрация платформоспецифичных сервисов для Android
        builder.Services.AddSingleton<MusicPlayer.Platforms.Android.Services.AudioService>();
#endif

        // Регистрация ViewModels  
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<PlayerViewModel>();
        builder.Services.AddTransient<PlaylistsViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        // Регистрация страниц  
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<PlayerPage>();
        builder.Services.AddTransient<PlaylistsPage>();
        builder.Services.AddTransient<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

