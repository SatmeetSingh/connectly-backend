using dating_app_backend.src.DB;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class MessageHub : Hub
{
    private readonly AppDbContext _context;

    public MessageHub(AppDbContext context)
    {
        _context = context;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"];
        if (Guid.TryParse(userId, out var parsedUserId))
        {
            var user = await _context.Users.FindAsync(parsedUserId);
            if (user != null)
            {
                user.ConnectionId = Context.ConnectionId;
                await _context.SaveChangesAsync();
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.ConnectionId == Context.ConnectionId);
        if (user != null)
        {
            user.ConnectionId = null; // Clear connection when user disconnects
            await _context.SaveChangesAsync();
        }

        await base.OnDisconnectedAsync(exception);
    } 
}
