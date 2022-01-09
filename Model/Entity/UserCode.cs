namespace Model.Entity;

[Table("USER_CODES")]
public class UserCode {
    [PrimaryKey]
    [ForeignColumn(ForeignType.MANY_TO_ONE, "USER_ID")]
    public User User { get; set; }

    [PrimaryKey]
    public CodeType Type { get; set; }

    [Column("CODE")]
    public string Code { get; set; }
}

public enum CodeType { EMAIL, PASSWORD }
