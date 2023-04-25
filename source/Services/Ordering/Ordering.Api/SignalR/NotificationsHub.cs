namespace Ordering.Api.SignalR
{
    [Authorize]
    public class NotificationsHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.User?.FindFirstValue(ClaimTypes.Name)!);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User?.FindFirstValue(ClaimTypes.Name)!);
            await base.OnDisconnectedAsync(ex);
        }
    }
}