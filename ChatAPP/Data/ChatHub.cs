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
            // Validate inputs
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new HubException("Username cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new HubException("Message cannot be empty.");
            }

            if (chatRoomId <= 0)
            {
                throw new HubException("Invalid chat room ID.");
            }

            try
            {
                // Fetch the user from the database using the repository
                var user = _repository.GetUserByUsername(username);
                if (user == null)
                {
                    throw new HubException("User not found.");
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

                // Notify all connected clients in the chat room about the new message
                await Clients.Group(chatRoomId.ToString()).SendAsync("ReceiveMessage", user.Username, message);

                // Log successful message delivery
                Console.WriteLine($"Message sent to room {chatRoomId} by {username}: {message}");
            }
            catch (HubException hex)
            {
                // Handle known Hub exceptions
                Console.WriteLine($"HubException: {hex.Message}");
                throw; // Re-throw to notify the client
            }
            catch (System.Exception ex)
            {
                // Log the error
                Console.WriteLine($"Unexpected error while sending message: {ex.Message}");
                throw new HubException("An unexpected error occurred while sending the message.", ex);
            }
        }

        // Method to retrieve messages for a specific chat room
        public IEnumerable<ChatMessage> GetMessages(int chatRoomId)
        {
            if (chatRoomId <= 0)
            {
                throw new HubException("Invalid chat room ID.");
            }

            try
            {
                // Return messages from the database for the specified chat room
                return _repository.GetMessages(chatRoomId);
            }
            catch (System.Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error while retrieving messages for room {chatRoomId}: {ex.Message}");
                throw new HubException("An error occurred while retrieving messages.", ex);
            }
        }

        // Method to join a specific chat room
        public async Task JoinChatRoom(int chatRoomId)
        {
            if (chatRoomId <= 0)
            {
                throw new HubException("Invalid chat room ID.");
            }

            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId.ToString());
                Console.WriteLine($"User {Context.ConnectionId} joined chat room {chatRoomId}");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error joining chat room {chatRoomId}: {ex.Message}");
                // Consider logging more details like the user or connection ID for troubleshooting
                throw new HubException("An error occurred while joining the chat room.", ex);
            }
        }

        // Method to leave a specific chat room
        public async Task LeaveChatRoom(int chatRoomId)
        {
            if (chatRoomId <= 0)
            {
                throw new HubException("Invalid chat room ID.");
            }

            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId.ToString());
                Console.WriteLine($"User {Context.ConnectionId} left chat room {chatRoomId}");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error while leaving chat room {chatRoomId}: {ex.Message}");
                throw new HubException("An error occurred while leaving the chat room.", ex);
            }
        }
    }
}
