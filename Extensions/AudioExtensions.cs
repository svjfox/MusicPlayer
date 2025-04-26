using MusicPlayer.Models;
using Plugin.Maui.Audio;

namespace MusicPlayer.Extensions
{
    /// <summary>
    /// Расширения для работы с IAudioPlayer.
    /// </summary>
    public static class AudioExtensions
    {
        /// <summary>
        /// Устанавливает параметры уведомления для аудиоплеера.
        /// </summary>
        /// <param name="player">Экземпляр IAudioPlayer.</param>
        /// <param name="title">Заголовок уведомления.</param>
        /// <param name="subtitle">Подзаголовок уведомления.</param>
        /// <param name="description">Описание уведомления.</param>
        /// <param name="image">URL или путь к изображению для уведомления.</param>
        /// <param name="actions">Список действий для уведомления.</param>
        public static void SetNotificationOptions(
            this IAudioPlayer player,
            string title,
            string subtitle,
            string description,
            string image,
            List<NotificationAction>? actions = null)
        {
#if ANDROID
            // Проверяем, что player реализует Android-специфичный интерфейс
            if (player is IAndroidAudioPlayer androidPlayer)
            {
                androidPlayer.SetNotificationOptions(
                    title, subtitle, description, image, actions);
            }
#endif
        }
    }

    /// <summary>
    /// Интерфейс для Android-специфичного аудиоплеера с поддержкой уведомлений.
    /// </summary>
    public interface IAndroidAudioPlayer : IAudioPlayer
    {
        /// <summary>
        /// Устанавливает параметры уведомления.
        /// </summary>
        /// <param name="title">Заголовок уведомления.</param>
        /// <param name="subtitle">Подзаголовок уведомления.</param>
        /// <param name="description">Описание уведомления.</param>
        /// <param name="image">URL или путь к изображению для уведомления.</param>
        /// <param name="actions">Список действий для уведомления.</param>
        void SetNotificationOptions(
            string title,
            string subtitle,
            string description,
            string image,
            List<NotificationAction>? actions);
    }
}
