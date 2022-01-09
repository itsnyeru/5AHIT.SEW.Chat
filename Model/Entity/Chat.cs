namespace Model.Entity;

[Table("CHATS_BT")]
public class Chat {
    [PrimaryKey]
    [AutoIncrement]
    [Column("CHAT_ID")]
    public long Id { get; set; }

    public DateTime LastEntry { get; set; } = DateTime.Now;

    public ICollection<ChatUser>? Users { get; set; }
    public ICollection<Message>? Messages { get; set; }
}
