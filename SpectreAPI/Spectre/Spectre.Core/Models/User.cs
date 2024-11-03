using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class User
    {
        public User()
        {
            Awards = new HashSet<Award>();
            Bands = new HashSet<Band>();
            Banners = new HashSet<Banner>();
            SavedFilters = new HashSet<SavedFilter>();
            SocioEonomics = new HashSet<SocioEonomic>();
            TemplateFilterDetails = new HashSet<TemplateFilterDetail>();
            TemplateFilters = new HashSet<TemplateFilter>();
            UserActionLogs = new HashSet<UserActionLog>();
            UserNotifications = new HashSet<UserNotification>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? Dob { get; set; }
        public DateTime? CreationDate { get; set; }
        public string IdentityUserId { get; set; }
        public bool? IsDeleted { get; set; }
        public string LastName { get; set; }
        public string IdentityNumber { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsLocked { get; set; }
        public int? CompanyId { get; set; }
        public string UserType { get; set; }
        public string JobTitle { get; set; }
        public bool? LoggedInBefore { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Award> Awards { get; set; }
        public virtual ICollection<Band> Bands { get; set; }
        public virtual ICollection<Banner> Banners { get; set; }
        public virtual ICollection<SavedFilter> SavedFilters { get; set; }
        public virtual ICollection<SocioEonomic> SocioEonomics { get; set; }
        public virtual ICollection<TemplateFilterDetail> TemplateFilterDetails { get; set; }
        public virtual ICollection<TemplateFilter> TemplateFilters { get; set; }
        public virtual ICollection<UserActionLog> UserActionLogs { get; set; }
        public virtual ICollection<UserNotification> UserNotifications { get; set; }
    }
}
