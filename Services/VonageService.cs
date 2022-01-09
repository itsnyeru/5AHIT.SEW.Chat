using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vonage;
using Vonage.Request;

namespace Services {
    public interface ISmsService {
        Task SendSmsAsync(string from, string to, string message);
    }

    public class SmsSettings {
        public string Key { get; set; }
        public string Secret { get; set; }
    }

    public class VonageService : ISmsService {
        readonly SmsSettings _settings;
        private VonageClient _client;

        public VonageService(SmsSettings settings) {
            _settings = settings;
            _client = new VonageClient(Credentials.FromApiKeyAndSecret(_settings.Key, _settings.Secret));
        }

        public async Task SendSmsAsync(string from, string to, string message) => await _client.SmsClient.SendAnSmsAsync(new Vonage.Messaging.SendSmsRequest() {
            From = from,
            To = to,
            Text = message
        });
    }
}
