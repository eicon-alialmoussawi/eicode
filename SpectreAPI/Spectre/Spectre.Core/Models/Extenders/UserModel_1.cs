using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Core.Models.Extenders
{
    public class UserModel_1
    {
        public int Id { get; set; }
        public string UserName { set; get; }
        public string FirstNameEn { get; set; }
        public string FirstNameAr { get; set; }
        public string LastNameEn { get; set; }
        public string LastNameAr { get; set; }
        public string RecordNumber { set; get; }
        public string MiddleNameAr { set; get; }
        public string MiddleNameEn { set; get; }
        public string MotherFirstNameAr { set; get; }
        public string MotherFirstNameEn { set; get; }
        public string MotherLastNameAr { set; get; }
        public string MotherLastNameEn { set; get; }
        public string IdentityUserId { set; get; }
        public string IdentityNumber { set; get; }
        public Nullable<DateTime> CreationDate { set; get; }
        public Nullable<DateTime> DOB { set; get; }
        public string PhoneNumber { set; get; }
        public Nullable<bool> IsAdmin { set; get; }
        public Nullable<bool> IsDeleted { set; get; }
        public Nullable<bool> IsLocked { set; get; }
        public Nullable<bool> IsApproved { set; get; }
        public string Email { set; get; }
        public Nullable<bool> IsFarmer { set; get; }
        public Nullable<bool> IsSupplier { set; get; }
        public string RoleId { get; set; }
        public string Password { get; set; }
        public Nullable<int> CenterId { get; set; }
        public string JobTitle { get; set; }

        public Nullable<int> ProvinceId { get; set; }
        public Nullable<int> CountyId { get; set; }
        public Nullable<int> DistrictId { get; set; }
        public Nullable<bool> IsSuperAdmin { get; set; }

    }
}
