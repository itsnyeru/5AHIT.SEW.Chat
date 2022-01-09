namespace Forms;

public class RequestPasswordResetForm {
    public string Identifier { get; set; }
}

public class PasswordResetForm {
    public string Code { get; set; }
    public string Password { get; set; }
}