using System.Security.Cryptography;

namespace Model.Entity;

[Table("MESSAGES")]
public class Message {
    [PrimaryKey]
    [AutoIncrement]
    [Column("MESSAGE_ID")]
    public long MessageId { get; set; }

    [ForeignColumn(ForeignType.MANY_TO_ONE, "USER_ID", DeleteBehavior.SetNull)]
    public User? User { get; set; }
    
    [ForeignColumn(ForeignType.MANY_TO_ONE, "CHAT_ID", DeleteBehavior.SetNull)]
    public Chat? Chat { get; set; }

    [Required]
    public DateTime SendAt { get; set; } = DateTime.Now;

    [Required]
    public Crypt<MessageAlgorithm> Content { get; set; } = "";

    public ICollection<MessageAttachment>? MessageAttachments { get; set; }
}

public class MessageAlgorithm : CustomAlgorithm {
    public MessageAlgorithm() : base("customkeyjustwritesomethinghere1") { }
}