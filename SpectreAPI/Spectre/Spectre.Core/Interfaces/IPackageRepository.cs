using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Core.Models;
using Spectre.Core.Models.Extenders;

namespace Spectre.Core.Interfaces
{
    public interface IPackageRepository
    {
        Task<Tuple<bool, List<Package>>> GetAll();
        Task<Tuple<bool, bool>> DeletePackage(int Id);
        Task<Tuple<bool, Package>> GetById(int Id);
        Task<Tuple<bool, List<PackagePermission_Extender>>> GetPermissionByPackageId(int PackageId);
        Task<Tuple<bool, ResponseMessage>> SavePackage(Package_View package);
        Task<Tuple<bool, List<PackagePermission_Details>>> GetPackagePagePermissionDetails(int? PackageId);
        Task<IEnumerable<Package>> GetVisiblePackags();
        //Task<Tuple<bool, Package>> SavePackage(Package package);

    }
}
