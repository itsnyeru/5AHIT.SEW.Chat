namespace Model.Entity;

[Table("FRIEND_REQUESTS_JT")]
public class FriendRequest {
    [PrimaryKey]
    [ForeignColumn(ForeignType.MANY_TO_ONE, "SENDER_ID", DeleteBehavior.ClientCascade)]
    public User Sender { get; set; }

    [PrimaryKey]
    [ForeignColumn(ForeignType.MANY_TO_ONE, "RECEIVER_ID", DeleteBehavior.ClientCascade)]
    public User Receiver { get; set; }
}