﻿namespace Xxx.Markets.Interview.OrderBook.Model;

public readonly struct Order
{
    public bool Equals(Order other) => OrderId == other.OrderId;

    public override bool Equals(object obj) => obj is Order other && Equals(other);

    public override int GetHashCode() => OrderId.GetHashCode();

    public Order(long orderId, string symbol, bool isBuy, decimal price, int quantity)
    {
        OrderId = orderId;
        Symbol = symbol;
        IsBuy = isBuy;
        Price = price;
        Quantity = quantity;
    }

    public long OrderId { get; }

    public string Symbol { get; }

    public bool IsBuy { get; }

    public decimal Price { get; }

    public int Quantity { get; }
}