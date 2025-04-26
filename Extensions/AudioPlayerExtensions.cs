using MusicPlayer.Models;
using Plugin.Maui.Audio;
using System.Collections.Generic;

namespace MusicPlayer.Extensions
{
    public static class AudioPlayerExtensions
    {
        public static void SetNotificationOptions(
            this IAudioPlayer player,
            string title,
            string subtitle,
            string description,
            string image,
            List<NotificationAction> actions = null)
        {
            // Проверяем, что плеер не null    
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            // Создаем уведомление    
            var notification = new Notification
            {
                Title = title,
                Subtitle = subtitle,
                Description = description,
                Image = image,
                Actions = actions ?? new List<NotificationAction>()
            };

            // Привязываем уведомление к плееру    
            if (player is INotificationSupport notificationSupport)
            {
                notificationSupport.SetNotification(notification);
            }
            else
            {
                throw new NotSupportedException("IAudioPlayer does not support notifications.");
            }
        }
    }

    public interface INotificationSupport
    {
        void SetNotification(Notification notification);
    }

    // Добавляем недостающий класс Notification  
    public class Notification
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public List<NotificationAction> Actions { get; set; }
    }
}
