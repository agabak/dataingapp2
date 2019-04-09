using System.Threading.Tasks;
using Datingapp.API.Models;

namespace Datingapp.API.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user, string passowrd);
         Task<User> Login(string username, string passowrd);
         Task<bool> UserExist(string username);
    }
}