using System.Collections.Generic;
using Datingapp.API.Models;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Datingapp.API.Data
{
    public class Seed
    {
        
        private readonly UserManager<User> _userManager;
        public Seed(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

         public void SeedUser()
        {
            // check if the database is empty before you insert any user
            if(!_userManager.Users.Any())
            {
                var userData = File.ReadAllText("Data/userSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach(var user in users)
                {
                    _userManager.CreateAsync(user,"password").Wait();
                }
            }
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