using System;
using Xxx.Markets.Interview.OrderBook.Interface;

namespace Xxx.Markets.Interview.OrderBook.Consumer
{
    public sealed class ConsoleLogger : ILog
    {
        public void Log(string message)
        {
            Console.WriteLine(message?.Trim());
        }
    }
}