using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Authenticate(string username, string password);

        Task Register(string username, string password);
        Task<bool> UserAlreadyExists(string username);
    }
}