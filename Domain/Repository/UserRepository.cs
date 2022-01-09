using EFCAT.Model.Annotation;
using Forms;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

public interface IUserRepository : IRepositoryAsync<User, long> {
    Task<User?> GetAccount(long id);
    Task<User?> GetAccount(string identifier, string password);

    Task<List<UserCode>> GetUserCodes(User user);
    Task<UserCode> CreatePasswordResetCode(User user);
    Task<UserCode> CreateEmailVerificationCode(User user);

    Task<bool> GetDarkMode(User user);
    Task<List<User>> SearchUser(User user, string name);
    Task AddFriend(User user, User friend);
    Task<List<User>> GetFriends(User user);
    Task<List<User>> GetFriendRequests(User user);
}

public class UserRepository : ARepositoryAsync<User, long>, IUserRepository {
    public UserRepository(ChatDbContext context) : base(context) { }

    public async Task<User?> GetAccount(long id) => await _entitySet.FindAsync(id);

    /* Returns a User after  */
    public async Task<User?> GetAccount(string identifier, string password) {
        var user = _entitySet.Include(u => u.Codes).SingleOrDefault(u => u.Email == identifier || u.Username == identifier);
        if(user == null) return await Task.FromResult(user);
        return await Task.FromResult(user.Password.Verify(password) ? user : null);
    }

    /* Returns all Codes a user got */
    public async Task<List<UserCode>> GetUserCodes(User user) => await _context.Set<UserCode>().Where(c => c.User.Id == user.Id).ToListAsync();

    /* Creates a usercode for a password reset */
    public async Task<UserCode> CreatePasswordResetCode(User user) => await CreateCode(user, CodeType.PASSWORD);
    /* Creates a usercode for email verification */
    public async Task<UserCode> CreateEmailVerificationCode(User user) => await CreateCode(user, CodeType.EMAIL);

    /* Creates a usercode for the specified type */
    private async Task<UserCode> CreateCode(User user, CodeType type) {
        var code = await _context.Set<UserCode>().FirstOrDefaultAsync(c => c.User.Id == user.Id && c.Type == type);
        if(code != null) return code;
        code = new UserCode() {
            User = user,
            Type = type,
            Code = new Random().Next(0, 1000000).ToString("D6")
        };
        await _context.Set<UserCode>().AddAsync(code);
        await _context.SaveChangesAsync();
        return code;
    }

    /* Returns the dark mode status of a user */
    public async Task<bool> GetDarkMode(User user) => (await _entitySet.FirstOrDefaultAsync(u => u.Id == user.Id)).DarkMode;

    /* Gets all Friend Request a user got */
    public async Task<List<User>> GetFriendRequests(User user) => await _context.Set<FriendRequest>().Where(fr => fr.Receiver == user).Select(fr => fr.Sender).ToListAsync();

    /* Searches for users which are not befriended with the user*/
    public async Task<List<User>> SearchUser(User user, string name) =>
        await Task.FromResult(
            _entitySet
            .Where(u => u != user && u.Username.ToUpper().StartsWith(name.ToUpper()))
            .Include(u => u.Friends)
            .Include(u => u.FriendRequestsReceived)
            .Where(u => !u.Friends.Select(f => f.FriendUser).Contains(user) && !u.FriendRequestsReceived.Select(f => f.Sender).Contains(user))
            .OrderBy(u => u.Username)
            .Skip(0)
            .Take(10)
            .ToList());

    /* Gets all friends of a user */
    public async Task<List<User>> GetFriends(User user) => await _context.Set<Friend>().Where(f => f.User.Id == user.Id).Select(f => f.FriendUser).ToListAsync();

    /* Sends or accepts the request of a user */
    public async Task AddFriend(User user, User friend) {
        if(await _context.Set<Friend>().AnyAsync(f => f.User == user && f.FriendUser == friend)) return;
        var fr = await _context.Set<FriendRequest>().FirstOrDefaultAsync(fr => fr.Sender == friend && fr.Receiver == user);
        if(fr != null) {
            _context.Set<FriendRequest>().Remove(fr);
            await _context.Set<Friend>().AddRangeAsync(new Friend() { User = user, FriendUser = friend }, new Friend() { User = friend, FriendUser = user });
            await _context.SaveChangesAsync();
        } else {
            await _context.Set<FriendRequest>().AddAsync(new FriendRequest { Sender = user, Receiver = friend });
            await _context.SaveChangesAsync();
        }
    }
}

