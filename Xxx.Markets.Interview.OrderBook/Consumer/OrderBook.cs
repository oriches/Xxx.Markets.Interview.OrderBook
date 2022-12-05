using System;
using System.Collections.Generic;
using System.Linq;
using Xxx.Markets.Interview.OrderBook.Extensions;
using Xxx.Markets.Interview.OrderBook.Interface;
using Xxx.Markets.Interview.OrderBook.Model;

namespace Xxx.Markets.Interview.OrderBook.Consumer;

public readonly struct OrderBook : IEquatable<OrderBook>
{
    public static bool operator ==(OrderBook left, OrderBook right) => left.Equals(right);

    public static bool operator !=(OrderBook left, OrderBook right) => !left.Equals(right);

    public OrderBook(string symbol, IEnumerable<Order> buys, IEnumerable<Order> sells)
    {
        Symbol = symbol;

        Buys = buys.GroupBy(x => x.Price)
            .Select(x => new OrderBookLevel(true, x.Key, x.Sum(y => y.Quantity), x.Count()))
            .OrderByDescending(x => x.Price)
            .ToArray();

        Sells = sells.GroupBy(x => x.Price)
            .Select(x => new OrderBookLevel(false, x.Key, x.Sum(y => y.Quantity), x.Count()))
            .OrderBy(x => x.Price)
            .ToArray();
    }

    public IEnumerable<OrderBookLevel> Buys { get; }

    public IEnumerable<OrderBookLevel> Sells { get; }

    public string Symbol { get; }

    public bool Equals(OrderBook other) => Symbol == other.Symbol;

    public override bool Equals(object obj) => obj is OrderBook other && Equals(other);

    public override int GetHashCode() => Symbol != null ? Symbol.GetHashCode() : 0;

    public void Dump(ILog log)
    {
        log.Log($"Symbol: {Symbol}");

        log.Log("Buy Price\tQuantity\tCount");
        Buys.ForEach(row => { log.Log($"{row.Price:N2}\t\t{row.Quantity}\t\t{row.Count}"); });

        log.Log("Ask Price\tQuantity\tCount");
        Sells.ForEach(row => { log.Log($"{row.Price:N2}\t\t{row.Quantity}\t\t{row.Count}"); });

        log.Log("");
    }
}