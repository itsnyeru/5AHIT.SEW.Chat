
namespace Forms;

public class InputMessage {
    public string Content { get; set; } = "";
    public List<InputDocument> Attachment { get; set; } = new List<InputDocument>();
}