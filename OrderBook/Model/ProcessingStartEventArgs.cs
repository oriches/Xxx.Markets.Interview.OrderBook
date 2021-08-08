using System;
using Xxx.Markets.Interview.OrderBook.Interface;

namespace Xxx.Markets.Interview.OrderBook.Model
{
    public class ProcessingStartEventArgs : EventArgs
    {
        public ProcessingStartEventArgs(ILog log)
        {
            Log = log;
        }

        public ILog Log { get; }
    }
}