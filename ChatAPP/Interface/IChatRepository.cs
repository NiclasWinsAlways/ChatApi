using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Repositories
{
    public interface IChatRepository
    {
        IEnumerable<ChatRoom> GetChatRooms();
        ChatRoom GetChatRoom(int id);
        ChatRoom AddChatRoom(ChatRoom chatRoom);
        void AddUserToRoom(int chatRoomId, User user);
        IEnumerable<ChatMessage> GetMessages(int chatRoomId);
        IEnumerable<ChatMessage> GetAllMessages();
        ChatMessage AddMessage(ChatMessage message);
        User GetUser(int id);
        User GetUserByEmail(string email);
        User GetUserByUsername(string username); 

        User AddUser(User user);
        IEnumerable<User> GetAllUsers();
        void UpdateUser(User user);
        void DeleteUser(int id);
        ChatMessage GetMessageById(int id); // Method to get a message by ID
        void UpdateMessage(ChatMessage message); // Method to update a message
        void DeleteMessage(int id); // Method to delete a message
    }
}
