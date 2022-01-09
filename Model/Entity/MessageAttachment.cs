namespace Model.Entity;

[Table("MESSAGE_HAS_ATTACHMENTS")]
public class MessageAttachment {
    [PrimaryKey]
    [AutoIncrement]
    [Column("ATTACHMENT_ID")]
    public long Id { get; set; }

    [Required]
    [ForeignColumn(ForeignType.MANY_TO_ONE, "MESSAGE_ID")]
    public Message Message { get; set; }

    [Varchar(64)]
    public string Name { get; set; }

    [Required]
    public Document Attachment { get; set; }
}