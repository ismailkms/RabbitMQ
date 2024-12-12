using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Producer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //RabbitMQ açık kaynak kodlu mesaj aracıdır.(messaging broker). RabbitMQ, AMQP(Advanced Message Queuing Protocol) adı verilen bir mesaj protokülü kullanır.

            //Broker: Mesajları yönlendiren RabbitMQ sunucusudur.
            //Producer: Mesajları üreten uygulama veya bileşendir.
            //Consumer: Mesajları alan ve işleyen uygulama veya bileşendir.
            //Exchange: Mesajların kuyruklara yönlendirilme şeklini belirleyen yapıdır.
            //Queue: Mesajların depolandığı ve tüketildiği yapıdır.
            //Routing Key: Mesajların belirli bir kuyruğa yönlendirilmesi için kullanılan bir etikettir.

            //RabbitMQ Exchange Türleri

            //Direct Exchange => Direct exchange, mesajları doğrudan bir kuyruğa yönlendirmek için kullanılır. Mesaj, exchange tarafından belirlenen routing key ile eşleştiği kuyruğa gönderilir.Örnek senaryo: Bir e-ticaret uygulaması, yeni bir sipariş oluşturulduğunda sipariş bilgilerini direkt exchange aracılığıyla sipariş kuyruğuna yönlendirir.

            //Topic Exchange => Topic exchange, mesajları routing key’e göre kuyruklara yönlendirmek için kullanılır. Routing key, “.” karakteri ile ayrılmış bir veya daha fazla kelime içeren bir dizedir.Mesaj, routing key’e eşleşen kuyruklara gönderilir. Örnek senaryo: Bir haber uygulaması, spor kategorisindeki haberleri “haber.spor” routing key’iyle, ekonomi kategorisindeki haberleri “haber.ekonomi” routing key’iyle topic exchange aracılığıyla ilgili kuyruklara gönderir.

            //Fanout Exchange => Fanout exchange, mesajları tüm kuyruklara eşit şekilde dağıtmak için kullanılır.Mesaj, exchange’e bağlı tüm kuyruklara gönderilir. Örnek senaryo: Bir bildirim uygulaması, bir kullanıcıya gönderilen mesajları fanout exchange aracılığıyla kullanıcının tüm cihazlarına gönderir.

            //Headers Exchange => Headers exchange, mesajları header bilgilerine göre kuyruklara yönlendirmek için kullanılır.Header bilgileri, özel bir şekilde tanımlanmış özelliklerdir. Mesaj, header bilgileri ile eşleşen kuyruklara gönderilir. Örnek senaryo: Bir reklam uygulaması, hedef kitlesi yaş aralığı 18–24 olan kullanıcılara gönderilen reklamları headers exchange aracılığıyla ilgili kuyruklara gönderir.

            //RabbitMQ Queue Türleri
            
            //Durable Queue => Kuyruk, RabbitMQ sunucusu yeniden başlatıldığında bile korunur. Durable değerini false olarak ayarlarsanız yeniden başlatıldığında silinecektir.
            //Exclusive Queue => Kuyruk, sadece oluşturulan bağlantıya özeldir. Bağlantı sonlandırıldığında, kuyruk da silinir.
            //Auto-Delete Queue => Kuyruk, en az bir consumer bağlı değilken, son mesajı aldıktan sonra otomatik olarak silinir.

            //channel => Eğer bir kuyruğa mesaj bırakmak veya bir kuyruğu tanımlamak istiyorsan channel kullanmalısın.
            //connection => Bir channel oluşturabilmek için connection kullanmalısın.
            //connectionFactory => Bir connection oluşturabilmek için connectionFactory kullanmalısın.

            var connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost" //Harfin büyük ya da küçük olamasına duyarlıdır.

                //localhost'e değilde uzaktaki bir RabbitMQ'ya bağlanmak istiyorsak alttaki şekilde bağlantı yapmalıyız.
                //Uri = new Uri("connection string"),
                //UserName ="userName",
                //Password = "password"
            };

            var connection = await connectionFactory.CreateConnectionAsync();
            //connectionFactory.CreateConnection();

            var channel = await connection.CreateChannelAsync();
            //connection.CreateModel();

            var message = "Hello RabbitMQ";
            var byteMessage = Encoding.UTF8.GetBytes(message);

            //Hangi exchange'i kullanacağımızı belirliyoruz. "hello" vermiş olduğumuz isim
            await channel.ExchangeDeclareAsync(exchange:"hello1", type: ExchangeType.Fanout);

            //Mesaj gönderme
            Thread.Sleep(4000); //Uygulama çalıştığında Consumer tarafında oluşturmuş olduğumuz kuyruğa burada oluşturduğumuz exchange ile bind ettikten sonra mesaj göndermek için 4 saniye beklettim. Yani console uygulamasında test amaçlı
            await channel.BasicPublishAsync(exchange: "hello1", routingKey: string.Empty, body: byteMessage);
            //Eğer bir exchange belirtmediysek routingKey'de queue name'i belirtmemiz gerekiyor. exchange boş bırakıldığında RabbitMQ default bir exchange ayağa kaldırır ve bu Direct tipinderdir.
            //RabbitMQ byte dizileriyle iş yapar.(byte[]) Byte dizileriyle RabbitMQ'dan mesaj gönderebiliriz ya da RabbitMQ'dan bize gelen mesaj byte dizisi olarak bize gelir. Bu nedenle body bizden byte dizisi bekler.

            Console.WriteLine("Mesaj gönderildi");

            Console.ReadKey();
            //Console klavyeden bir tuşa basılana kadar açık kalır.
        }
    }
}
