namespace Model.Entity;

[Table("USERS")]
public class User {
    [PrimaryKey]
    [AutoIncrement]
    [Column("USER_ID")]
    public long Id { get; set; }

    [Unique]
    [Required]
    [Varchar(16, Min = 3)]
    public string Username { get; set; } = "";

    [Unique]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public Crypt<SHA256> Password { get; set; }

    public Image? Image { get; set; }

    [Required]
    public ChatMode ChatMode { get; set; } = ChatMode.ONLY_FRIENDS;

    [Required]
    [Boolean]
    public bool DarkMode { get; set; } = false;

    public ICollection<UserCode>? Codes { get; set; }

    public ICollection<ChatUser>? Chats { get; set; }

    [ReferenceColumn("User")]
    public ICollection<Friend>? Friends { get; set; }
    [ReferenceColumn("Sender")]
    public ICollection<FriendRequest>? FriendRequestsSend { get; set; }
    [ReferenceColumn("Receiver")]
    public ICollection<FriendRequest>? FriendRequestsReceived { get; set; }
}

public enum ChatMode { PUBLIC, ONLY_FRIENDS }
