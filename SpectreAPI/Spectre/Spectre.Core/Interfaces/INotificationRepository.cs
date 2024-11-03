using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;

namespace Spectre.Core.Interfaces
{
    public interface INotificationRepository
    {
        Task<Tuple<bool, List<UserNotification_View>>> GetUserNotifications(int UserId, string Status);
        Task<Tuple<bool>> SetNotificationAsViewed(int Id);
        Task<Tuple<bool>> SendNotification(string TextEn, string TextAr, string TextFr);
        Task<Tuple<bool>> SetUserNotificationAsSeen(int UserId);
        Task<Tuple<bool, NotifcationMessage_View>> GetUnSeenNotifications(int UserId);
    }
}
