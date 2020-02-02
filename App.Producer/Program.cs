using System;
using System.Threading;

namespace App.Producer
{
    class Program
    {
        private static bool Terminate = false;
        private static int i = 0;
        private static Thread CounterThread = new Thread(new ThreadStart(StartCounter));
        private static Producer producer = new Producer();

        static void Main(string[] args)
        {
            try
            {

                Console.ForegroundColor = ConsoleColor.Green;

                Thread MainThread = new Thread(new ThreadStart(startProcess));
                Thread ConsoleKeyListener = new Thread(new ThreadStart(ListeningKeyBoardEvent));

                MainThread.Name = "Processamento";
                ConsoleKeyListener.Name = "KeyReading";

                MainThread.Start();
                ConsoleKeyListener.Start();

                while(true)
                {
                    if(Terminate)
                    {
                        Console.WriteLine("Finalizado aplicação...");
                        Environment.Exit(0);
                        return;
                    }
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine("Atenção! Houve um erro inesperado: " + ex.Message);
            }
        }

        #region .: Key Events :.

        static void KeyStartPress(object sender, KeyPressEventArgs args)
        {
            var msg = "Operação iniciada";

            if(args.Key == ConsoleKey.F1)
            {
                if(!CounterThread.IsAlive)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(msg);
                    Console.WriteLine("--------------------------------------");

                    producer.SendMsgKafka("Tecla " + args.Key + " pressionada");
                    producer.SendMsgKafka(msg);

                    CounterThread.Start();
                }
            }
        }

        static void KeyEndPress(object sender, KeyPressEventArgs args)
        {
            if(args.Key == ConsoleKey.F2)
            {
                CounterThread.Abort();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nTecla F2 clicada.");
                Console.WriteLine("Operação será encerrada");

                producer.SendMsgKafka("Tecla " + args.Key + " pressionada");

                Thread.Sleep(4000);

                Terminate = true;
            }
        }

        static void AnyKeyPress(object sender, KeyPressEventArgs args)
        {
            if(args.Key != ConsoleKey.F1 && args.Key != ConsoleKey.F2)
            {
                if(CounterThread.IsAlive)
                {
                    CounterThread.Abort();
                }

                producer.SendMsgKafka("Tecla " + args.Key + " pressionada");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("----=== Contagem reiciada ===----");
                Console.ForegroundColor = ConsoleColor.White;
                CounterThread = new Thread(new ThreadStart(StartCounter));
                CounterThread.Start();
            }
        }

        #endregion

        static void StartCounter()
        {
            int count = 0;

            while(count <= 10)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Aplicação em espera a : " + count + " segundos");
                count++;
            }
            SendAlertAndCloseApplication();
        }

        private static void startProcess()
        {

            Events evento = new Events();
            evento.OnKeyClick += KeyStartPress;
            evento.OnKeyClick += KeyEndPress;
            evento.OnKeyClick += AnyKeyPress;

            Console.WriteLine("======================================================");
            Console.WriteLine("Aperte F1 para inciar a aplicação");
            Console.WriteLine("Aperte F2 para encerrar a aplicação");
            Console.WriteLine("======================================================");

        }

        private static void ListeningKeyBoardEvent()
        {
            Events evento = new Events();
            evento.OnKeyClick += KeyStartPress;
            evento.OnKeyClick += KeyEndPress;
            evento.OnKeyClick += AnyKeyPress;

            ConsoleKeyInfo cki;

            do
            {
                while(Console.KeyAvailable == false)
                    Thread.Sleep(100);

                cki = Console.ReadKey(true);

                evento.OnPress(cki.Key);

            } while(cki.Key != ConsoleKey.Escape);
        }

        private static void SendAlertAndCloseApplication()
        {
            string msgAlerta = "Identificado ausência de ações na operação e por segurança essa será encerrada. ";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n--------------------------------------");
            Console.WriteLine(msgAlerta);

            producer.SendMsgKafka(msgAlerta);

            Thread.Sleep(2000);
            Environment.Exit(0);
        }
    }
}
