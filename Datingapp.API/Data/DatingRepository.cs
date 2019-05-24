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

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages
                                   .Include(u => u.Sender).ThenInclude(p => p.Photos)
                                   .Include(u => u.Recipient).ThenInclude(p => p.Photos).AsQueryable();
                switch(messageParams.MessageContainer)
                {
                    case "Inbox":
                       messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false);
                       break;
                    case  "Outbox":
                       messages = messages.Where(u => u.SenderId == messageParams.UserId && u.SenderDeleted == false);
                       break;
                    default:
                        messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.IsRead == false && u.RecipientDeleted == false);
                        break;
                } 
            messages = messages.OrderByDescending(m => m.MessageSent);
            return  await PagedList<Message>.GreateAsync(messages,messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages
                                   .Include(u => u.Sender).ThenInclude(p => p.Photos)
                                   .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                                   .Where(m => m.RecipientId == userId && m.RecipientDeleted == false && m.SenderId == recipientId ||
                                          m.RecipientId == recipientId && m.SenderDeleted == false && m.SenderId == userId)
                                          .OrderByDescending(m => m.MessageSent).ToListAsync();
                return messages;
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

               if(userParams.Likers) 
               {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
               }

               if(userParams.Likees) 
               {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u =>  userLikees.Contains(u.Id));
               }

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

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
             var user = await _context.Users.Include(x => x.Likers)
                                        .Include(x => x.Likees).FirstOrDefaultAsync(x => x.Id == id);
               if(likers) 
               {
                   return  user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
               }
               else
               {
                   return   user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
               }                           
        }
    }
}