using KafkaNet;
using KafkaNet.Model;
using System;
using System.Text;

namespace App.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new KafkaOptions(new Uri("http://localhost:9092"), new Uri("http://localhost:9092"));

            var router = new BrokerRouter(options);
            var consumer = new KafkaNet.Consumer(new ConsumerOptions("desk-msg", new BrokerRouter(options)));

            foreach(var message in consumer.Consume())
            {
                Console.WriteLine("Response: P{0},O{1} : {2}",
                        message.Meta.PartitionId,
                        message.Meta.Offset,
                        Encoding.UTF8.GetString(message.Value));
            }
        }
    }
}
