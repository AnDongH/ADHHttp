using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using System.Diagnostics;
using System.Security.Cryptography;

namespace StressClient {
    public class Program {

        static string ServerURL { get; set; } = "http://127.0.0.1:7777";
        static string Version { get; set; } = "0.0.1";

        static List<Client> clients = new List<Client>();
        static List<CancellationTokenSource> cancels = new List<CancellationTokenSource>();


        static async Task Main(string[] args) {
            
            UnionProtocolReqFormatterInitializer.RegisterFormatter();
            UnionProtocolResFormatterInitializer.RegisterFormatter();
            UnionItemInfoFormatterInitializer.RegisterFormatter();
            UnionRewardInfoFormatterInitializer.RegisterFormatter();

            bool isRunning = true;

            Console.WriteLine($"start client - {Version}");

            while (isRunning) {

                string input = Console.ReadLine();

                switch (input) {
                    case "s":
                        isRunning = false;
                        break;
                    case "a":

                        for (int i = 0; i < 10; i++) {

                            (string id, string pw) = MakeImpelementIDAndPW();
                            CancellationTokenSource c = new CancellationTokenSource();
                            cancels.Add(c);
                            _ = Test(id, pw, c.Token).WithCancellation(c.Token);

                        }
                        
                        break;
                    case "b":

                        for (int i = 0; i < 10; i++) {

                            (string id, string pw) = MakeImpelementIDAndPW();
                            CancellationTokenSource c = new CancellationTokenSource();
                            cancels.Add(c);
                            _ = ItemDBTest(id, pw, c.Token).WithCancellation(c.Token);

                        }
                        break;
                    case "c":

                        for (int i = 0; i < 10; i++) {

                            (string id, string pw) = MakeImpelementIDAndPW();
                            CancellationTokenSource c = new CancellationTokenSource();
                            cancels.Add(c);
                            _ = ItemDBUpdateTest(id, pw, c.Token).WithCancellation(c.Token);

                        }

                        break;
                    case "d": 
                        
                        {
                            (string id, string pw) = MakeImpelementIDAndPW();
                            _ = GetGatchaTest(id, pw);
                        }

                        break;

                }

            }

            foreach (CancellationTokenSource cancel in cancels) {

                cancel.Cancel();
                cancel.Dispose();

            }

            foreach (Client client in clients) {

                await client.DeleteAccountRequest();
                
            }

        }

        static async Task Test(string id, string pw, CancellationToken cancellationToken) {

            Client client = await InstantiateClient(id, pw);                        

            Stopwatch stopwatch = new Stopwatch();

            while (true) {

                stopwatch.Start();
                var res = await client.AuthPostTest(client.UID.ToString()).WithCancellation(cancellationToken);

                if (res.Result != ErrorCode.None) return;
                
                stopwatch.Stop();
                Console.WriteLine($"{res.responseMSG} - {stopwatch.Elapsed.TotalMilliseconds}ms");
                stopwatch.Reset();
            }

        }

        static async Task ItemDBTest(string id, string pw, CancellationToken cancellationToken) {

            Client client = await InstantiateClient(id, pw);

            Stopwatch stopwatch = new Stopwatch();

            while (true) {

                stopwatch.Start();
                var res = await client.ItemDBTest(true).WithCancellation(cancellationToken);
                stopwatch.Stop();
                Console.WriteLine($"[ItemDB]: {res.Result} - {stopwatch.Elapsed.TotalMilliseconds}ms");
                stopwatch.Reset();
                
            }

        }

        static async Task ItemDBUpdateTest(string id, string pw, CancellationToken cancellationToken) {

            Client client = await InstantiateClient(id, pw);

            Stopwatch stopwatch = new Stopwatch();

            while (true) {
                
                stopwatch.Start();
                var res = await client.ItemDBUpdateTest().WithCancellation(cancellationToken);
                stopwatch.Stop();
                Console.WriteLine($"[ItemDB Update Test]: {res.Result} - {stopwatch.Elapsed.TotalMilliseconds}ms");
                stopwatch.Reset();

            }

        }

        static async Task GetGatchaTest(string id, string pw) {

            Client client = await InstantiateClient(id, pw);

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            var res = await client.GetGatchaList();
            stopwatch.Stop();
            
            Console.WriteLine($"[Get Gatcha Test]: {res.Result} - {stopwatch.Elapsed.TotalMilliseconds}ms");

            foreach (var item in res.rewardInfos.rewardInfo) {

                Console.WriteLine(item.ToString());
                
            }

            Console.WriteLine("===================================================");

            stopwatch.Reset();

        }

        static async Task<Client> InstantiateClient(string id, string pw) {

            Client client = new Client(ServerURL, Version, pw);

            await client.HandShake();

            await client.CreateAccountRequest(id, pw);

            await client.LoginRequest(id, pw);

            lock (clients) {
                clients.Add(client);
            }
            
            return client;

        }

        
        static (string, string) MakeImpelementIDAndPW() {

            var bytes = new byte[8];

            using (var random = RandomNumberGenerator.Create()) {

                random.GetBytes(bytes);
                bytes[7] &= 0x7F;

            }

            long result = BitConverter.ToInt64(bytes, 0);

            return ("ID-" + result.ToString(), "PW-" + result.ToString());

        }

    }
}
