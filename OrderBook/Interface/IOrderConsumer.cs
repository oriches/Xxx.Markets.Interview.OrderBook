using System;
using Xxx.Markets.Interview.OrderBook.Model;

namespace Xxx.Markets.Interview.OrderBook.Interface
{
    public interface IOrderConsumer
    {
        /// <summary>
        ///     This is called by the environment before any events are processed.
        /// </summary>
        void StartProcessing(object sender, ProcessingStartEventArgs args);

        /// <summary>
        ///     This handles a specific order event. The properties given in the order
        ///     depend on the action:
        ///     For Remove: OrderId
        ///     For Edit: OrderId, Quantity and Price
        ///     For Add: OrderId, Symbol, IsBuy, Quantity and Price
        /// </summary>
        void HandleOrderAction(object sender, OrderActionEventArgs args);

        /// <summary>
        ///     This is called by the environment when no more events will be processed.
        /// </summary>
        void FinishProcessing(object sender, EventArgs args);
    }
}