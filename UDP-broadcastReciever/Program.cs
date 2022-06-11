using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace UDP_broadcastReciever
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("UDP-broadcast receiver(Server)!");

            //Lav et program der kan modtage de udsendte UDP-broadcast.
            //Vind-retning og vind-hastighed skal udskrives i hver sin
            //linie.

            //Forklar ligheder og forskelle mellem UDP-broadcast og
            //almindelig UDP.
            //note med dine stik - ord: 
            // - UDP is a connectionless protocol.the Internet Protocol (IP) suite.
            // - UDP broadcasting insulates the client and Unified Broker
            // from having to know the exact host location of the NameServer.
            // - the Broadcast term, it describes the process of
            // broadcasting packets to an entire subnet.
            // - UDP does not provide the reliability of TCP

            //Opg - 4
            //REST er lavet til at kunne modtage et objekt der ligner dit Wind object:

            //fordi jeg deserializer i min receiver, så er det nemmere at sende på den måde
            //du gør ved at anvende (PostAsJson)
            //men hvis jeg ikke deserialized, kunne jeg bruge metoden fra proxy, med StringContent



            using (UdpClient socket = new UdpClient())
            {
                //initialisering af object binding:
                //bindes kun en gang
                socket.Client.Bind(new IPEndPoint(IPAddress.Any, 5005));

                //Opg 4 - HttpClient
                //baseclass der sender Http request, og modtager http responses
                using (HttpClient httpClient = new HttpClient())
                {
                    while (true)
                    { 
                        //IPEndPoint - indeholder en host og en local eller remote port,
                        //som er den information der behøves for at få en application til
                        //at connecte til en service på en host. ved at kombinere
                        // hosten's IP address og port-nummer på en service, tager
                        //IPEndPoint class og opretter en connection til den service.
                        IPEndPoint clientEndpoint = null;
                        //Her “listens” der på port 5005, på alle netværks
                        //adapters på den PC det køres.
                        //modtager fra en client
                        //byte array
                        byte[] data = socket.Receive(ref clientEndpoint);
                        //Encoding - konvertere det til en string
                        //UTF8 - beskederne sendes i UTF8 format
                        string objectRecieved = Encoding.UTF8.GetString(data);
                       
                        //modtager som byte array
                        //derfor deserialize for at konvertere det om til et almindeligt
                        //objekt/læseligt - modsatte
                        //nu er der et obj. der hedder Wind
                        Wind wind = JsonConvert.DeserializeObject<Wind>(objectRecieved);

                        //message fra client i konsollen
                        Console.WriteLine("Message received Speed: " + wind.Speed);
                        Console.WriteLine("Message received Direction: " + wind.Direction);

                        //Opg 4 - Den UDP-broadcast receiver du lavede i en tidligere opgave skal
                        //udbygges: Hver gang du modtager 
                        //et UDP-broadcast, skal du kalde Add i din REST controller
                        //tilføjede url
                        httpClient.PostAsJsonAsync("https://localhost:44358/api/WindGenerators", wind);
                    }
                }
            }
        }
    }
}
