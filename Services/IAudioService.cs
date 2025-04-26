using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicPlayer.Models;

namespace MusicPlayer.Services
{
    /// <summary>
    /// Интерфейс для управления воспроизведением аудио.
    /// </summary>
    public interface IAudioService : IDisposable
    {
        /// <summary>
        /// Инициализирует аудиосервис.
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Загружает трек для воспроизведения.
        /// </summary>
        /// <param name="track">Трек для загрузки.</param>
        Task LoadTrackAsync(Track track);

        /// <summary>
        /// Начинает воспроизведение текущего трека.
        /// </summary>
        Task PlayAsync();

        /// <summary>
        /// Ставит воспроизведение на паузу.
        /// </summary>
        Task PauseAsync();

        /// <summary>
        /// Останавливает воспроизведение.
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// Переходит к следующему треку в плейлисте.
        /// </summary>
        Task NextAsync();

        /// <summary>
        /// Переходит к предыдущему треку в плейлисте.
        /// </summary>
        Task PreviousAsync();

        /// <summary>
        /// Перематывает текущий трек на указанную позицию.
        /// </summary>
        /// <param name="position">Позиция в секундах.</param>
        Task SeekToAsync(double position);

        /// <summary>
        /// Устанавливает громкость воспроизведения.
        /// </summary>
        /// <param name="volume">Громкость (от 0 до 1).</param>
        Task SetVolumeAsync(double volume);

        /// <summary>
        /// Устанавливает скорость воспроизведения.
        /// </summary>
        /// <param name="speed">Скорость (от 0.5 до 2.0).</param>
        Task SetSpeedAsync(double speed);

        /// <summary>
        /// Переключает режим случайного воспроизведения.
        /// </summary>
        Task ToggleShuffleAsync();

        /// <summary>
        /// Переключает режим повторения трека.
        /// </summary>
        Task ToggleRepeatAsync();

        /// <summary>
        /// Устанавливает плейлист для воспроизведения.
        /// </summary>
        /// <param name="tracks">Список треков.</param>
        /// <param name="startIndex">Индекс начального трека.</param>
        Task SetPlaylistAsync(IEnumerable<Track> tracks, int startIndex = 0);

        /// <summary>
        /// Устанавливает таймер сна.
        /// </summary>
        /// <param name="minutes">Время в минутах до остановки воспроизведения.</param>
        Task SetSleepTimerAsync(int minutes);

        /// <summary>
        /// Отменяет таймер сна.
        /// </summary>
        Task CancelSleepTimer();

        /// <summary>
        /// Переключает воспроизведение между паузой и воспроизведением.
        /// </summary>
        Task TogglePlay();

        /// <summary>
        /// Возвращает состояние воспроизведения.
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// Возвращает состояние режима случайного воспроизведения.
        /// </summary>
        bool IsShuffleEnabled { get; }

        /// <summary>
        /// Возвращает состояние режима повторения.
        /// </summary>
        bool IsRepeatEnabled { get; }

        /// <summary>
        /// Возвращает состояние таймера сна.
        /// </summary>
        bool IsSleepTimerActive { get; }

        /// <summary>
        /// Текущая позиция воспроизведения в секундах.
        /// </summary>
        double CurrentPosition { get; }

        /// <summary>
        /// Длительность текущего трека в секундах.
        /// </summary>
        double Duration { get; }

        /// <summary>
        /// Текущий трек.
        /// </summary>
        Track CurrentTrack { get; }

        /// <summary>
        /// Текущий плейлист.
        /// </summary>
        IReadOnlyList<Track> CurrentPlaylist { get; }

        /// <summary>
        /// Событие, вызываемое при изменении состояния воспроизведения.
        /// </summary>
        event EventHandler<bool> IsPlayingChanged;

        /// <summary>
        /// Событие, вызываемое при изменении позиции воспроизведения.
        /// </summary>
        event EventHandler<double> PositionChanged;

        /// <summary>
        /// Событие, вызываемое при изменении текущего трека.
        /// </summary>
        event EventHandler<Track> TrackChanged;

        /// <summary>
        /// Событие, вызываемое при завершении воспроизведения трека.
        /// </summary>
        event EventHandler PlaybackEnded;

        /// <summary>
        /// Событие, вызываемое при изменении состояния режима случайного воспроизведения.
        /// </summary>
        event EventHandler<bool> ShuffleChanged;

        /// <summary>
        /// Событие, вызываемое при изменении состояния режима повторения.
        /// </summary>
        event EventHandler<bool> RepeatChanged;

        /// <summary>
        /// Событие, вызываемое при активации таймера сна.
        /// </summary>
        event EventHandler SleepTimerActivated;
    }
}


