using System;
using System.Reactive.Concurrency;
using Xxx.Markets.Interview.OrderBook.Consumer;

namespace Xxx.Markets.Interview.OrderBook
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var environment = new AppEnvironment();
            using (var consumer = new OrderConsumer(Scheduler.CurrentThread))
            {
                environment.ProcessingStartEvent += consumer.StartProcessing;
                environment.ProcessingFinishEvent += consumer.FinishProcessing;
                environment.OrderActionEvent += consumer.HandleOrderAction;

                environment.Run();
            }

            Console.WriteLine();
            Console.WriteLine("Press ENTER to close...");
            Console.ReadLine();
        }
    }
}