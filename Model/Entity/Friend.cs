namespace Model.Entity;

[Table("FRIENDS_JT")]
public class Friend {
    [PrimaryKey]
    [ForeignColumn(ForeignType.MANY_TO_ONE, "USER_ID")]
    public User User { get; set; }

    [PrimaryKey]
    [ForeignColumn(ForeignType.MANY_TO_ONE, "FRIEND_ID")]
    public User FriendUser { get; set; }
}
