using Microsoft.AspNetCore.SignalR;
using Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubs {
    public class ChatHub : Hub {
        public async Task SendMessage(long chatId, long msgId) {
            await Clients.All.SendAsync($"ReceiveMessage_{chatId}", msgId);
        }

        public async Task SendDeleteMessage(long chatId, long msgId) {
            await Clients.All.SendAsync($"ReceiveDeleteMessage_{chatId}", msgId);
        }

        public async Task SendStatus(long chatId) {
            await Clients.All.SendAsync($"ReceiveStatus_{chatId}");
        }
    }
}
