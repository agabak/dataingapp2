using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datingapp.API.Helpers;
using Datingapp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Datingapp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
           _context = context;
        }
        
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes
                                 .FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<User> GetUser(int id)
        {
            return await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Id == id);
        }

        public  async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =   _context.Users.Include(p => p.Photos)
                                        .OrderByDescending(u => u.LastActive)
                                        .AsQueryable();

               users = users.Where(x => x.Id != userParams.UserId);
               users = users.Where(x => x.Gender == userParams.Gender);

               if(userParams.MinAge != 18 || userParams.MaxAge != 99) 
               {
                   var minDob = DateTime.Today.AddYears(-userParams.MaxAge -1);
                   var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
                   
                    users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
               }

               if(!string.IsNullOrEmpty(userParams.OrderBy)) {
                   switch(userParams.OrderBy) {
                       case "created":
                          users = users.OrderByDescending(u => u.Created);
                        break;
                        default:
                          users = users.OrderByDescending(u => u.LastActive);
                         break;
                         
                   }
               }

            return await PagedList<User>.GreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public  async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}