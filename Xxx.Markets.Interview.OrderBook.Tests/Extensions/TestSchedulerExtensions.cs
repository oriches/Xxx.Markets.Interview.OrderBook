using System;
using Microsoft.Reactive.Testing;

namespace Xxx.Markets.Interview.OrderBook.Tests.Extensions;

public static class TestSchedulerExtensions
{
    public static void AdvanceBy(this TestScheduler scheduler, TimeSpan duration) =>
        scheduler.AdvanceBy(duration.Ticks);
}