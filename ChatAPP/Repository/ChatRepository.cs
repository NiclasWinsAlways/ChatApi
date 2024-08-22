using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ChatApp.Models;
using ChatApp.Data;

namespace ChatApp.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatAppContext _context;

        public ChatRepository(ChatAppContext context)
        {
            _context = context;
        }

        // ChatRoom related methods
        public IEnumerable<ChatRoom> GetChatRooms()
        {
            return _context.ChatRooms
                .Include(r => r.Users)
                .Include(r => r.Messages)
                .ToList();
        }

        public ChatRoom GetChatRoom(int id)
        {
            return _context.ChatRooms
                .Include(r => r.Users)
                .Include(r => r.Messages)
                .FirstOrDefault(r => r.Id == id);
        }

        public ChatRoom AddChatRoom(ChatRoom chatRoom)
        {
            _context.ChatRooms.Add(chatRoom);
            _context.SaveChanges();
            return chatRoom;
        }

        public void AddUserToRoom(int chatRoomId, User user)
        {
            var chatRoom = GetChatRoom(chatRoomId);
            if (chatRoom != null && !chatRoom.Users.Any(u => u.Id == user.Id))
            {
                chatRoom.Users.Add(user);
                _context.SaveChanges();
            }
        }

        // User related methods
        public User GetUser(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public User GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public User AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            var user = GetUser(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public bool VerifyUserPassword(User user, string password)
        {
            return user.Password == password;
        }

        
        // ChatMessage related methods
        public IEnumerable<ChatMessage> GetMessages(int chatRoomId)
        {
            return _context.ChatMessages
                .Where(m => m.ChatRoomId == chatRoomId)
                .Include(m => m.User) // Include the User to avoid lazy loading issues
                .OrderBy(m => m.Timestamp)
                .ToList();
        }

        public IEnumerable<ChatMessage> GetAllMessages()
        {
            return _context.ChatMessages
                .Include(m => m.User) // Include the User to avoid lazy loading issues
                .OrderBy(m => m.Timestamp)
                .ToList();
        }

        public ChatMessage GetMessageById(int id)
        {
            return _context.ChatMessages
                .Include(m => m.User) // Include User to avoid lazy loading
                .FirstOrDefault(m => m.Id == id);
        }

        public ChatMessage AddMessage(ChatMessage message)
        {
            _context.ChatMessages.Add(message);
            _context.SaveChanges();
            return message;
        }

        public void UpdateMessage(ChatMessage message)
        {
            _context.ChatMessages.Update(message);
            _context.SaveChanges();
        }

        public void DeleteMessage(int id)
        {
            var message = GetMessageById(id);
            if (message != null)
            {
                _context.ChatMessages.Remove(message);
                _context.SaveChanges();
            }
        }
    }
}
