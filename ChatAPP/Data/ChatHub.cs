using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using ChatApp.Models;
using ChatApp.Repositories;
using System.Collections.Generic;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatRepository _repository;

        public ChatHub(IChatRepository repository)
        {
            _repository = repository;
        }

        // Method to send a message to a specific chat room
        public async Task SendMessage(int chatRoomId, string username, string message)
        {
            // Fetch the user from the database using the repository
            var user = _repository.GetUserByUsername(username);

            if (user == null)
            {
                // Handle the case where the user is not found
                throw new System.Exception("User not found.");
            }

            // Create a new ChatMessage instance
            var chatMessage = new ChatMessage
            {
                User = user, // Assign the User object
                Message = message,
                Timestamp = System.DateTime.Now,
                ChatRoomId = chatRoomId // Associate the message with the chat room
            };

            // Add the message to the database
            _repository.AddMessage(chatMessage);

            // Notify all connected clients about the new message
            await Clients.Group(chatRoomId.ToString()).SendAsync("ReceiveMessage", user.Username, message);
        }

        // Method to retrieve messages for a specific chat room
        public IEnumerable<ChatMessage> GetMessages(int chatRoomId)
        {
            // Return messages from the database for the specified chat room
            return _repository.GetMessages(chatRoomId);
        }

        // Method to join a specific chat room
        public async Task JoinChatRoom(int chatRoomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId.ToString());
        }

        // Method to leave a specific chat room
        public async Task LeaveChatRoom(int chatRoomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId.ToString());
        }
    }
}
