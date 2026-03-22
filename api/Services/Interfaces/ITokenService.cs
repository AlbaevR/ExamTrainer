using api.Models;
using System.Threading.Tasks;

namespace api.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
