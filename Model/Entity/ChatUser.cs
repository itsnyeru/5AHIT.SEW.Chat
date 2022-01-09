namespace Model.Entity;

[Table("CHAT_HAS_USERS_JT")]
public class ChatUser {
    [PrimaryKey]
    [ForeignColumn(ForeignType.MANY_TO_ONE, "CHAT_ID")]
    public Chat Chat { get; set; }

    [PrimaryKey]
    [ForeignColumn(ForeignType.MANY_TO_ONE, "USER_ID")]
    public User User { get; set; }

    [ForeignColumn(ForeignType.MANY_TO_ONE, "MESSAGE_ID")]
    public Message? Message { get; set; }
}