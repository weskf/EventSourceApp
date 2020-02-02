using KafkaNet;
using KafkaNet.Model;
using KafkaNet.Protocol;
using System;

namespace App.Producer
{
    public class Producer
    {
        private KafkaNet.Producer client { get; set; }

        public Producer()
        {
            var options = new KafkaOptions(new Uri("http://localhost:9092"));
            var router = new BrokerRouter(options);
            client = new KafkaNet.Producer(router);
        }

        private void SendProducerMessage(string msg)
        {
            client.SendMessageAsync("desk-msg", new[] { new Message(msg) }).Wait();
        }

        public void SendMsgKafka(string msg)
        {
            var LogMsg = "Date: " + DateTime.Now + " / " + msg;
            SendProducerMessage(LogMsg);
        }
    }
}
