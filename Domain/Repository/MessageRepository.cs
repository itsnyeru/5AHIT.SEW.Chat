using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository;

public interface IMessageRepository : IRepositoryAsync<Message, long> {
    Task<List<Message>> GetMessages(Chat chat);
    Task<Message> SendMessage(Message message);
    Task DeleteMessage(Message message);
    Task<Message> ReceiveMessage(long id);
    Task SetMessageStatus(User user, Message message);
    Task<List<string>> CheckMessageStatus(Message message);
}

public class MessageRepository : ARepositoryAsync<Message, long>, IMessageRepository {
    public MessageRepository(ChatDbContext context) : base(context) { }

    /* Gets all messages from a chat */
    public async Task<List<Message>> GetMessages(Chat chat) => await _context.Set<Message>().Include(m => m.User).Include(m => m.MessageAttachments).Where(m => m.Chat.Id == chat.Id).ToListAsync();

    /* Sends a message */
    public async Task<Message> SendMessage(Message message) {
        var chat = await _context.Set<Chat>().FirstOrDefaultAsync(c => c.Id == message.Chat.Id);
        var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.Id == message.User.Id);
        Message msg = new Message() {
            Chat = chat,
            User = user,
            SendAt = message.SendAt,
            Content = message.Content,
            MessageAttachments = message.MessageAttachments
        };
        await _context.Set<Message>().AddAsync(msg);
        chat.LastEntry = message.SendAt;
        _context.Set<Chat>().Update(chat);
        await _context.SaveChangesAsync();
        return msg;
    }

    /* Delete a message */
    public async Task DeleteMessage(Message message) {
        var msg = await _entitySet.Include(m => m.MessageAttachments).FirstOrDefaultAsync(m => m.MessageId == message.MessageId);
        var chat = await _context.Set<Chat>().Include(c => c.Messages).FirstOrDefaultAsync(c => c.Id == message.Chat.Id);
        if(msg == null || chat == null) return;
        chat.Messages.Remove(msg);
        _context.Set<Chat>().Update(chat);
        await _context.SaveChangesAsync();
    }

    /* Receives a single message */
    public async Task<Message> ReceiveMessage(long id) => await _context.Set<Message>().Include(m => m.User).Include(m => m.MessageAttachments).FirstAsync(m => m.MessageId == id);

    public async Task SetMessageStatus(User user, Message message) {
        var chatUser = await _context.Set<ChatUser>().FirstOrDefaultAsync(cu => cu.User.Id == user.Id && cu.Chat.Id == message.Chat.Id);
        chatUser.Message = message;
        _context.Set<ChatUser>().Update(chatUser);
        await _context.SaveChangesAsync();
    }

    /* Checks the status from one message */
    public async Task<List<string>> CheckMessageStatus(Message message) =>
        await _context.Set<ChatUser>()
            .Include(cu => cu.Message)
            .Include(cu => cu.User)
            .Include(cu => cu.Chat)
            .Where(cu => cu.Chat.Id == message.Chat.Id && cu.Message.MessageId >= message.MessageId)
            .Select(cu => cu.User.Username)
            .OrderBy(name => name)
            .ToListAsync();
}

