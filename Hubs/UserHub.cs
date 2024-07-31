using Fuchibol.ChatService.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Fuchibol.ChatService.Hubs
{
	public class UserHub : Hub
	{
		UserRepository userRepository;

		public UserHub(IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("DefaultConnection");
			userRepository = new UserRepository(connectionString);
		}

		public async Task SendUsers()
		{
			try
				{
					var users = userRepository.GetUsers();
					await Clients.All.SendAsync("ReceivedUsers", users);
				}
				catch (Exception ex)
				{
			throw new HubException("Error retrieving users", ex);
			}
		}
	}
}
