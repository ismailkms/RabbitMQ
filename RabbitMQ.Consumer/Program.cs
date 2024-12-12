using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = "localhost";

            var connection = await connectionFactory.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            //Kuyruk tanımlıyoruz.
            await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
            //queue => Oluşacak kuyruğun ismidir.
            //durable => RabbitMQ'nun restart durumunda kuyruk silinsin mi silinmesin mi durumudur. false olursa silinecektir, true olursa silinmeyecektir.
            //exclusive => Bu channel haricinde başka channel'ları kullanıp kullanamayacağınızı belirttiğimiz yerdir. false olursa başkaları kullanabilir, true olursa kullanamaz.
            //autoDelete => Son Consumer unsubscribe olduğunda ben buradaki kuyruğu siliyim mi silmeyim mi der. false silme, true sil demektir.

            //Queue'yu exchange'e bağlıyorz/bind ediyoruz.
            await channel.QueueBindAsync(queue: "hello", exchange: "hello1", routingKey: string.Empty);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                //Thread.Sleep(2000);
                var byteMessage = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(byteMessage);

                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                //Mesajları tet tek işlememizi sağlar ve işlediği mesajı kuyruktan çıkarır. autoAck true yaptığımızda her hangi bir sıkıntı olunca kuyruğun devamındaki mesajları kaybetme durumu burada yoktur. Kuyruk okunurken bir sıkıntı olsa dahi okunmayan mesajlar kuyrukta kalır okunanlar silinir.

                Console.WriteLine("Okunan mesaj: " + message);
            };

            //Kuyruk okuyoruz.
            await channel.BasicConsumeAsync(queue: "hello", autoAck: false, consumer: consumer);
            //autoAck => true olursa mesajlar okunduktan sonra kuyruktan silinir. Ama mesaj okuma esnasında her hangi bir problem olursa ve bazı mesajlar okunmasa bile tüm kuyruktaki mesajları siler.

            Console.ReadKey();
            //Console klavyeden bir tuşa basılana kadar açık kalır.
        }
    }
}
