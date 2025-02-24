using System.Threading.Tasks;

namespace Spectre.Core.Interfaces
{
    public interface IMapDataRepository
    {
        Task<string> GetJsonByName(string name);
    }
}
