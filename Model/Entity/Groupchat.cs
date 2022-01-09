namespace Model.Entity;

[Table("GROUPCHATS")]
public class Groupchat : Chat {
    [Varchar(32, Min = 3)]
    public string Name { get; set; }
}
