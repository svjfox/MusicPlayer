using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    public class NotificationAction
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Icon { get; set; }

        public NotificationAction(int id, string title, int icon)
        {
            Id = id;
            Title = title;
            Icon = icon;
        }
    }
}
