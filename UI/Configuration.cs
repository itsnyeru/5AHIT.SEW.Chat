using Services;

namespace UI;

internal class Configuration {
    public string DefaultConnection { get; set; } = "server = localhost; port = 3306; database = chat_db; user = htl; password = insy; Persist Security Info = False; Connect Timeout = 300";
    public MailSettings DefaultMail { get; set; } = new MailSettings() {
        Displayname = "ChatMail",
        Username = "sew@nurfuerspam.de",
        Password = "5ahit@Test{SEW}",
        Port = 587,
        Host = "mail.gmx.net"
    };
    public SmsSettings DefaultSms { get; set; } = new SmsSettings() {
        Key = "7fa0e3c8",
        Secret = "XaevGFl1smAv4172"
    };
}
