using System;

namespace Xxx.Markets.Interview.OrderBook.Model;

public sealed class OrderActionEventArgs : EventArgs
{
    public OrderActionEventArgs(Action action, Order order)
    {
        Action = action;
        Order = order;
    }

    public Action Action { get; }

    public Order Order { get; }
}