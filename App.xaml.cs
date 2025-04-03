using MusicPlayer.Services;
using MusicPlayer.ViewModels;
using MusicPlayer.Views;

namespace MusicPlayer;

public partial class App : Application
{
    public App(IAudioService audioService, IDataService dataService)
    {
        InitializeComponent();

        // Инициализация сервисов
        Task.Run(async () =>
        {
            await audioService.InitializeAsync();
            await dataService.InitializeAsync();
        });

        MainPage = new AppShell();
    }

    protected override void OnStart()
    {
        // Восстановление состояния плеера
        var audioService = Handler.MauiContext.Services.GetService<IAudioService>();
        Task.Run(audioService.InitializeAsync);
    }

    protected override void OnSleep()
    {
        // Сохранение состояния плеера
        var audioService = Handler.MauiContext.Services.GetService<IAudioService>();
        audioService.PauseAsync();
    }
}