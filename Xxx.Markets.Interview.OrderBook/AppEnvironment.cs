using System;
using Xxx.Markets.Interview.OrderBook.Consumer;
using Xxx.Markets.Interview.OrderBook.Interface;
using Xxx.Markets.Interview.OrderBook.Model;
using Action = Xxx.Markets.Interview.OrderBook.Model.Action;

namespace Xxx.Markets.Interview.OrderBook;

public class AppEnvironment : IAppEnvironment
{
    private readonly ILog _log = new ConsoleLogger();

    public void Run()
    {
        OnProcessingStart(new ProcessingStartEventArgs(_log));
        FeedOrders();
        OnProcessingFinish();
    }

    public event EventHandler<OrderActionEventArgs> OrderActionEvent;
    public event EventHandler<ProcessingStartEventArgs> ProcessingStartEvent;
    public event EventHandler ProcessingFinishEvent;

    private void FeedOrders()
    {
        Command[] commands =
        {
            new(Action.Add, 1L, "MSFT.L", true, 5, 200),
            new(Action.Add, 2L, "VOD.L", true, 15, 100),
            new(Action.Add, 3L, "MSFT.L", false, 5, 300),
            new(Action.Add, 4L, "MSFT.L", true, 7, 150),
            new(Action.Remove, 1L, null, true, -1, -1),
            new(Action.Add, 5L, "VOD.L", false, 17, 300),
            new(Action.Add, 6L, "VOD.L", true, 12, 150),
            new(Action.Edit, 3L, null, true, 7, 200),
            new(Action.Add, 7L, "VOD.L", false, 16, 100),
            new(Action.Add, 8L, "VOD.L", false, 19, 100),
            new(Action.Add, 9L, "VOD.L", false, 21, 112),
            new(Action.Remove, 5L, null, false, -1, -1)
        };

        foreach (var command in commands) OnOrderAction(new OrderActionEventArgs(command.Action, command.Order));
    }

    private void OnProcessingStart(ProcessingStartEventArgs args)
    {
        ProcessingStartEvent?.Invoke(this, args);
    }

    private void OnProcessingFinish()
    {
        ProcessingFinishEvent?.Invoke(this, EventArgs.Empty);
    }

    private void OnOrderAction(OrderActionEventArgs args)
    {
        OrderActionEvent?.Invoke(this, args);
    }

    public class Command
    {
        public Command(Action action, long orderId, string symbol, bool isBuy, decimal price, int quantity)
        {
            Action = action;
            Order = new Order(orderId, symbol, isBuy, price, quantity);
        }

        public Action Action { get; }
        public Order Order { get; }
    }
}