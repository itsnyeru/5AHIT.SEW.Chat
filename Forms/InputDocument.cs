
namespace Forms;

public class InputDocument {
    [Varchar(32, Min = 3)]
    public string Name { get; set; } = "";
    public Document Document { get; set; } = new Document();
}