using System;

namespace Xxx.Markets.Interview.OrderBook.Consumer;

public readonly struct OrderBookLevel : IEquatable<OrderBookLevel>
{
    public static bool operator ==(OrderBookLevel left, OrderBookLevel right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(OrderBookLevel left, OrderBookLevel right)
    {
        return !left.Equals(right);
    }

    public bool Equals(OrderBookLevel other)
    {
        return IsBuy == other.IsBuy && Price == other.Price && Quantity == other.Quantity && Count == other.Count;
    }

    public override bool Equals(object obj)
    {
        return obj is OrderBookLevel other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsBuy, Price, Quantity, Count);
    }

    public OrderBookLevel(bool isBuy, decimal price, int quantity, int count)
    {
        IsBuy = isBuy;
        Price = price;
        Quantity = quantity;
        Count = count;
    }

    public bool IsBuy { get; }

    public decimal Price { get; }

    public int Quantity { get; }

    public int Count { get; }
}