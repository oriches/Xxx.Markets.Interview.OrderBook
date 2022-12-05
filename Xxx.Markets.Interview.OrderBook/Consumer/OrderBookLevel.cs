using System;

namespace Xxx.Markets.Interview.OrderBook.Consumer;

public readonly struct OrderBookLevel : IEquatable<OrderBookLevel>
{
    public static bool operator ==(OrderBookLevel left, OrderBookLevel right) => left.Equals(right);

    public static bool operator !=(OrderBookLevel left, OrderBookLevel right) => !left.Equals(right);

    public bool Equals(OrderBookLevel other) => IsBuy == other.IsBuy && Price == other.Price &&
                                                Quantity == other.Quantity && Count == other.Count;

    public override bool Equals(object obj) => obj is OrderBookLevel other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(IsBuy, Price, Quantity, Count);

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