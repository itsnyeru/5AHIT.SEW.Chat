using Forms;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

public interface IChatRepository : IRepositoryAsync<Chat, long> {
    Task<Singlechat> CreateSinglechat(User user, User partner);
    Task<Groupchat> CreateGroupchat(Groupchat chat);
    Task<List<DisplayChat>> GetChats(User user);
    Task<DisplayChat> GetChat(User user, long id);
}

public class ChatRepository : ARepositoryAsync<Chat, long>, IChatRepository {
    public ChatRepository(ChatDbContext context) : base(context) { }

    /* Returns or creates a Singlechat for the specified users */
    public async Task<Singlechat> CreateSinglechat(User user, User partner) =>
            await _context.Set<Singlechat>().Include(c => c.Users).FirstOrDefaultAsync(c => c.Users.Select(u => u.User).Contains(user) && c.Users.Select(u => u.User).Contains(partner)) ??
            await AddChat(new Singlechat() { Users = new[] { new ChatUser() { User = user },  new ChatUser() { User = partner } } });

    /* Creates a Groupchat from the input */
    public async Task<Groupchat> CreateGroupchat(Groupchat chat) => await AddChat(chat);

    /* Creates a chat */
    public async Task<T> AddChat<T>(T chat) where T : Chat {
        await _context.Set<T>().AddAsync(chat);
        await _context.SaveChangesAsync();
        return chat;
    }

    

    /* Gets information about all chats where the user is a member  */
    public async Task<List<DisplayChat>> GetChats(User user) {
        List<DisplayChat> displayChats = new List<DisplayChat>();
        foreach(Chat c in await GetAllChats(user)) {
            DisplayChat displayChat = await GetChat(user, c.Id);
            displayChats.Add(displayChat);
        }
        return displayChats;
    }

    /* Returns all chats where the user is a member */
    public async Task<List<Chat>> GetAllChats(User user) => await _context.Set<ChatUser>().Include(cu => cu.Chat.Users).Where(cu => cu.User.Id == user.Id).Select(cu => cu.Chat).ToListAsync();

    /* Gets all members from a chat */
    public async Task<List<ChatUser>> GetChatUsers(Chat chat) => await _context.Set<ChatUser>().Include(cu => cu.User).Where(cu => cu.Chat.Id == chat.Id).ToListAsync();

    /* Gets information about a chat */
    public async Task<DisplayChat> GetChat(User user, long chatId) {
        DisplayChat displayChat = new DisplayChat() { Name = "Unknown", Id = chatId };
        Chat? chat = (await GetAllChats(user)).FirstOrDefault(c => c.Id == chatId);
        if(chat == null) return displayChat;
        displayChat.Chat = chat;
        if(await _context.Set<Singlechat>().AnyAsync(s => s.Id == chatId)) {
            User other = (await GetChatUsers(chat)).Select(cu => cu.User).First(u => u.Id != user.Id);
            displayChat.Name = other.Username;
            displayChat.Image = other.Image;
            displayChat.Singlechat = true;
        } else {
            Groupchat groupChat = await _context.Set<Groupchat>().FirstAsync(c => c.Id == chatId);
            displayChat.Name = groupChat.Name;
            displayChat.Singlechat = false;
        }
        return displayChat;
    }
}