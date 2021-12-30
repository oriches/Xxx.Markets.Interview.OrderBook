using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Xxx.Markets.Interview.OrderBook.Extensions;
using Xxx.Markets.Interview.OrderBook.Interface;
using Xxx.Markets.Interview.OrderBook.Model;
using Action = Xxx.Markets.Interview.OrderBook.Model.Action;

namespace Xxx.Markets.Interview.OrderBook.Consumer;

public sealed class OrderConsumer : DisposableObject, IOrderConsumer
{
    private static readonly OrderBook[] Empty = Array.Empty<OrderBook>();

    private readonly Subject<bool> _finish;
    private readonly Subject<OrderActionEventArgs> _orders;
    private readonly Subject<ILog> _start;

    public OrderConsumer(IScheduler scheduler)
    {
        _start = new Subject<ILog>().DisposeWith(this);
        _finish = new Subject<bool>().DisposeWith(this);
        _orders = new Subject<OrderActionEventArgs>().DisposeWith(this);

        OrderBooks = Empty;

        _start.Select(x =>
            {
                return _orders.ObserveOn(scheduler)
                    .Scan(new Dictionary<long, Order>(), (y, args) =>
                    {
                        switch (args.Action)
                        {
                            case Action.Add:
                                y[args.Order.OrderId] = args.Order;
                                break;
                            case Action.Edit:
                            {
                                var existingOrder = y[args.Order.OrderId];
                                var updatedOrder = new Order(args.Order.OrderId, existingOrder.Symbol,
                                    args.Order.IsBuy,
                                    args.Order.Price, args.Order.Quantity);
                                y[args.Order.OrderId] = updatedOrder;
                                break;
                            }
                            default:
                                y.Remove(args.Order.OrderId);
                                break;
                        }

                        return y;
                    })
                    .Select(y =>
                    {
                        var orderBooks = y.GroupBy(group => group.Value.Symbol)
                            .Select(group =>
                            {
                                var buys = group.Select(kvp => kvp.Value)
                                    .Where(order => order.IsBuy);
                                var sells = group.Select(kvp => kvp.Value)
                                    .Where(order => !order.IsBuy);

                                return new OrderBook(group.Key, buys, sells);
                            })
                            .ToArray();

                        OrderBooks = orderBooks;
                        return new Tuple<ILog, OrderBook[]>(x, orderBooks);
                    });
            })
            .Switch()
            .CombineLatest(_finish.Where(y => y), (x, y) => x)
            .Subscribe(x => { x.Item2?.ForEach(orderBook => orderBook.Dump(x.Item1)); })
            .DisposeWith(this);
    }

    public IReadOnlyCollection<OrderBook> OrderBooks { get; private set; }

    public void StartProcessing(object sender, ProcessingStartEventArgs args)
    {
        _start.OnNext(args.Log);
    }

    public void HandleOrderAction(object sender, OrderActionEventArgs args)
    {
        _orders.OnNext(args);
    }

    public void FinishProcessing(object sender, EventArgs args)
    {
        _finish.OnNext(true);
        _finish.OnNext(false);
    }
}