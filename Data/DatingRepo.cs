using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.helper;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data
{
    public class DatingRepo : IDatingRepo
    {
        private readonly ApplicationDbContext _context;

        public DatingRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        // adding user 
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int userId, bool likers)
        {
            var user = await _context.Users
                .Include(u => u.Likees)
                .Include(u => u.Likers)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (likers)
            {
                return user.Likers.Where(u => u.LikeeId == userId).Select(u => u.LikerId);
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == userId).Select(u => u.LikeeId);

            }
        }


        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            //filter users with oppsite gender and not current user
            var users = _context.Users.Include(x => x.Photos).OrderByDescending(x => x.LastActive).AsQueryable();
            users = users.Where(u => u.Id != userParams.UserId && u.Gender == userParams.Gender);

            // age filter
            if (userParams.MaxAge != 99 || userParams.MinAge != 18)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
                users = users.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);
            }
            //find records that user as the liker
            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }
            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }

            //sorting filter
            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(x => x.CreatedAt);
                        break;

                    default:
                        users = users.OrderByDescending(x => x.LastActive);
                        break;
                }

            }


            return await PagedList<User>.creatAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Id == id);

            return user;
        }



        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
        }


        //message methods
        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages
                .Include(m => m.Sender).ThenInclude(m => m.Photos)
                .Include(m => m.Recipient).ThenInclude(m => m.Photos)
                .AsQueryable();

            switch (messageParams.MessageContainer)
            {
                case "inbox":
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDelete == false);
                    break;
                case "outbox":
                    messages = messages.Where(u => u.SenderId == messageParams.UserId && u.SenderDelete == false);
                    break;

                default:
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId
                    && u.RecipientDelete == false && u.IsRead == false);

                    break;
            }

            messages = messages.OrderByDescending(m => m.MessageSent);
            return await PagedList<Message>.creatAsync(messages, messageParams.PageNumber, messageParams.PageSize);

        }

        // messages between two users
        public async Task<IEnumerable<Message>> GetMessagesThread(int userId, int recipientId)
        {
            var messages = await _context.Messages
                .Include(m => m.Sender).ThenInclude(m => m.Photos)
                .Include(m => m.Recipient).ThenInclude(m => m.Photos)
                .Where(
                m => m.RecipientId == userId && m.RecipientDelete == false && m.SenderId == recipientId
                || m.RecipientId == recipientId && m.SenderId == userId && m.SenderDelete == false)
                .OrderByDescending(m => m.MessageSent).ToListAsync();

            return messages;


        }

    }
}
