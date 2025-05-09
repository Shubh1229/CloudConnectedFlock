using MQTTnet;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;

namespace HubService.Services
{
    public class MqttClientService
    {
        private IMqttClient? client;
        private readonly ILogger<MqttClientService> logger;

        public MqttClientService(ILogger<MqttClientService> logger)
        {
            this.logger = logger;
        }

        public async Task ConnectAsync()
        {
            var factory = new MqttFactory();
            client = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder().WithClientId("hubservice").WithTcpServer("mqtt-broker", 1883).WithCleanSession().Build();

            client.ApplicationMessageReceivedAsync += async e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = e.ApplicationMessage.Payload;

                logger.LogInformation($"[MQTT] Topic: {topic}, Payload: {payload}");

                await Task.CompletedTask;
            };

            await client.ConnectAsync(options);
            await client.SubscribeAsync("heartbeat/online");
        }
    }
}
