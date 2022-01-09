namespace Forms;

public class DisplayChat {
    public long Id { get; set; }
    public bool Singlechat { get; set; } = true;
    public string Name { get; set; } = "";
    public Image? Image { get; set; }
    public Chat? Chat { get; set; }
}