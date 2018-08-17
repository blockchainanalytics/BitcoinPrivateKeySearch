using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using System.Threading;

namespace BitcoinAddressSearch
{
    class Program
    {
        static int ThreadDelay = 0;
        static string TargetAddressLocation;
        static string PrivateKeyFoundSaveLocation;


        static void Main(string[] args)
        {
            if(args.Length < 3)
            {
                Console.WriteLine("3 parameters required : {Thread Delay}  {Target Address Location}  {Private Key Found Save Location}");
                return;
            }

            ThreadDelay = int.Parse(args[0]);
            TargetAddressLocation = args[1];
            PrivateKeyFoundSaveLocation = args[2];


            Thread T1 = new Thread(new ThreadStart(SearchBitcoinAddress));
            T1.Start();

            Console.WriteLine("You can now press any key to stop this thread");
            Console.ReadLine();
            T1.Abort();


            Console.ReadLine();
        }





        public static void SearchBitcoinAddress()
        {
            HashSet<string> bitcoinAddressesHash = new HashSet<string>();

            string line;
            string RandomTestAddress = "";
            Random rnd = new Random();
            System.IO.StreamReader file = new System.IO.StreamReader(TargetAddressLocation);
            while ((line = file.ReadLine()) != null)
            {
                bitcoinAddressesHash.Add(line);
                if (rnd.Next(100) < 10)           //select a random address for hashtable test
                    RandomTestAddress = line;
            }
            file.Close();


            Console.WriteLine("HashTable Initialized. It contains " + bitcoinAddressesHash.Count + " target addresses");

            //if we have a random address selected for testing then conduct a test on hashtable 
            if (RandomTestAddress != "")
            {
                Console.WriteLine("Conducting a test on hashtable for address " + RandomTestAddress);
                if (bitcoinAddressesHash.Contains(RandomTestAddress))
                    Console.WriteLine("Test successful");
            }

            Console.WriteLine("Thread delay is set to " + ThreadDelay);

            double total = 0;
            int count = 0;

            while (true)
            {
                Key privateKey = new Key();

                Key k = new Key();
                BitcoinSecret secretKey = k.GetBitcoinSecret(Network.Main);
                BitcoinAddress addressKey = secretKey.PubKey.GetAddress(Network.Main);

                //Console.WriteLine(total + "  " + secretKey.ToString() + "  " + addressKey.ToString());

                if (bitcoinAddressesHash.Contains(addressKey.ToString()))
                {
                    Console.WriteLine("*************  Check this out     Secret :" + secretKey + "         Key :" + addressKey.ToString());
                    string s = secretKey + " ----- " + addressKey.ToString();
                    System.IO.File.WriteAllText(PrivateKeyFoundSaveLocation + "BitcoinAddres-" + addressKey.ToString() + ".txt", s);
                }

                count++;
                if (count > 999)
                {
                    total = total + count;
                    Console.WriteLine("Total Search " + total);
                    count = 0;
                }

                Thread.Sleep(ThreadDelay);
            }
        }

    }
}