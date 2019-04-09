using System.Threading.Tasks;
using Datingapp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Datingapp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string passowrd)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
             if (user == null) return null;
             
             if(!VerifyPasswordHash(passowrd, user.PasswordHash, user.PasswordSalt)) return null;

            return user;
        }

        public async Task<User> Register(User user, string passowrd)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(passowrd, out passwordHash, out passwordSalt);
            user.PasswordHash =  passwordHash;
            user.PasswordSalt = passwordSalt;

            // Save into the database
            await  _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UserExist(string username)
        {
            if( await _context.Users.AnyAsync(x => x.Username == username)) return true;
            return false;
        }

        private bool VerifyPasswordHash(string passowrd, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
              var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passowrd));
                for(int i =0 ; i< computedHash.Length; i++)
                {
                     if(computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        private void CreatePasswordHash(string passowrd, out byte[] passwordHash, out byte[] passwordSalt)
        {   
            // if you have out, the values changes -passing it as reference type
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
              passwordSalt = hmac.Key;
              passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passowrd));
            }
        }

    }
}