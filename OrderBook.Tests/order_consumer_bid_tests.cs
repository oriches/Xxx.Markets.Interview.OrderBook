using System;
using System.Linq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Xxx.Markets.Interview.OrderBook.Consumer;
using Xxx.Markets.Interview.OrderBook.Model;
using Xxx.Markets.Interview.OrderBook.Tests.Extensions;
using Action = Xxx.Markets.Interview.OrderBook.Model.Action;

// ReSharper disable InconsistentNaming

namespace Xxx.Markets.Interview.OrderBook.Tests
{
    [TestFixture]
    public sealed class order_consumer_bid_tests
    {
        [SetUp]
        public void SetUp()
        {
            _scheduler = new TestScheduler();

            _log = new ConsoleLogger();
            _startEventArgs = new ProcessingStartEventArgs(_log);
            _finishEventArgs = EventArgs.Empty;
        }

        private ConsoleLogger _log;
        private ProcessingStartEventArgs _startEventArgs;
        private EventArgs _finishEventArgs;
        private TestScheduler _scheduler;

        [Test]
        public void bid_is_amended()
        {
            // ARRANGE
            var consumer = new OrderConsumer(_scheduler);

            var bid1 = new OrderActionEventArgs(Action.Add, new Order(1, "SYMBOL_1", true, 100, 1000));
            var bid2 = new OrderActionEventArgs(Action.Edit, new Order(1, "", true, 110, 2500));

            consumer.StartProcessing(this, _startEventArgs);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ACT
            consumer.HandleOrderAction(this, bid1);
            consumer.HandleOrderAction(this, bid2);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(consumer.OrderBooks.Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Symbol, Is.EqualTo("SYMBOL_1"));
            Assert.That(consumer.OrderBooks.First()
                .Buys, Is.Not.Empty);
            Assert.That(consumer.OrderBooks.First()
                .Buys.Count(), Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .IsBuy, Is.True);
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Price, Is.EqualTo(110));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Quantity, Is.EqualTo(2500));
            Assert.That(consumer.OrderBooks.First()
                .Sells, Is.Empty);
        }

        [Test]
        public void bid_is_amended_to_ask()
        {
            // ARRANGE
            var consumer = new OrderConsumer(_scheduler);

            var bid1 = new OrderActionEventArgs(Action.Add, new Order(1, "SYMBOL_1", true, 100, 1000));
            var bid2 = new OrderActionEventArgs(Action.Edit, new Order(1, null, false, 110, 2500));

            consumer.StartProcessing(this, _startEventArgs);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ACT
            consumer.HandleOrderAction(this, bid1);
            consumer.HandleOrderAction(this, bid2);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(consumer.OrderBooks.Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Symbol, Is.EqualTo("SYMBOL_1"));
            Assert.That(consumer.OrderBooks.First()
                .Buys, Is.Empty);
            Assert.That(consumer.OrderBooks.First()
                .Sells, Is.Not.Empty);
            Assert.That(consumer.OrderBooks.First()
                .Sells.Count(), Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Sells.First()
                .IsBuy, Is.False);
            Assert.That(consumer.OrderBooks.First()
                .Sells.First()
                .Price, Is.EqualTo(110));
            Assert.That(consumer.OrderBooks.First()
                .Sells.First()
                .Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Sells.First()
                .Quantity, Is.EqualTo(2500));
        }

        [Test]
        public void bid_is_withdrawn()
        {
            // ARRANGE
            var consumer = new OrderConsumer(_scheduler);

            var bid1 = new OrderActionEventArgs(Action.Add, new Order(1, "SYMBOL_1", true, 100, 1000));
            var bid2 = new OrderActionEventArgs(Action.Remove, new Order(1, null, false, 0, 0));

            consumer.StartProcessing(this, _startEventArgs);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ACT
            consumer.HandleOrderAction(this, bid1);
            consumer.HandleOrderAction(this, bid2);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(consumer.OrderBooks, Is.Empty);
        }

        [Test]
        public void bids_order_by_price_descending()
        {
            // ARRANGE
            var consumer = new OrderConsumer(_scheduler);

            var bid1 = new OrderActionEventArgs(Action.Add, new Order(1, "SYMBOL_1", true, 100, 1000));
            var bid2 = new OrderActionEventArgs(Action.Add, new Order(2, "SYMBOL_1", true, 300, 1000));
            var bid3 = new OrderActionEventArgs(Action.Add, new Order(3, "SYMBOL_1", true, 200, 1000));

            consumer.StartProcessing(this, _startEventArgs);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ACT
            consumer.HandleOrderAction(this, bid1);
            consumer.HandleOrderAction(this, bid2);
            consumer.HandleOrderAction(this, bid3);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(consumer.OrderBooks.Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Symbol, Is.EqualTo("SYMBOL_1"));
            Assert.That(consumer.OrderBooks.First()
                .Buys, Is.Not.Empty);
            Assert.That(consumer.OrderBooks.First()
                .Buys.Count(), Is.EqualTo(3));
            Assert.That(consumer.OrderBooks.First()
                .Buys.Skip(0)
                .First()
                .Price, Is.EqualTo(300));
            Assert.That(consumer.OrderBooks.First()
                .Buys.Skip(1)
                .First()
                .Price, Is.EqualTo(200));
            Assert.That(consumer.OrderBooks.First()
                .Buys.Skip(2)
                .First()
                .Price, Is.EqualTo(100));
            Assert.That(consumer.OrderBooks.First()
                .Sells, Is.Empty);
        }

        [Test]
        public void no_bids()
        {
            // ARRANGE
            var consumer = new OrderConsumer(_scheduler);

            consumer.StartProcessing(this, _startEventArgs);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ACT
            consumer.FinishProcessing(this, _finishEventArgs);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(consumer.OrderBooks, Is.Empty);
        }

        [Test]
        public void no_bids_if_not_started()
        {
            // ARRANGE
            var bid1 = new OrderActionEventArgs(Action.Add, new Order(1, "SYMBOL_1", true, 100, 1000));
            var bid2 = new OrderActionEventArgs(Action.Add, new Order(2, "SYMBOL_1", true, 300, 1000));
            var bid3 = new OrderActionEventArgs(Action.Add, new Order(3, "SYMBOL_1", true, 200, 1000));

            var consumer = new OrderConsumer(_scheduler);

            consumer.HandleOrderAction(this, bid1);
            consumer.HandleOrderAction(this, bid2);
            consumer.HandleOrderAction(this, bid3);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ACT
            consumer.FinishProcessing(this, _finishEventArgs);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(consumer.OrderBooks, Is.Empty);
        }

        [Test]
        public void two_bids_for_same_symbol_at_different_prices()
        {
            // ARRANGE
            var consumer = new OrderConsumer(_scheduler);

            var bid1 = new OrderActionEventArgs(Action.Add, new Order(1, "SYMBOL_1", true, 100, 1000));
            var bid2 = new OrderActionEventArgs(Action.Add, new Order(2, "SYMBOL_1", true, 110, 2000));

            consumer.StartProcessing(this, _startEventArgs);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ACT
            consumer.HandleOrderAction(this, bid1);
            consumer.HandleOrderAction(this, bid2);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(consumer.OrderBooks.Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Symbol, Is.EqualTo("SYMBOL_1"));
            Assert.That(consumer.OrderBooks.First()
                .Buys, Is.Not.Empty);
            Assert.That(consumer.OrderBooks.First()
                .Buys.Count(), Is.EqualTo(2));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .IsBuy, Is.True);
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Price, Is.EqualTo(110));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Quantity, Is.EqualTo(2000));
            Assert.That(consumer.OrderBooks.First()
                .Buys.Last()
                .IsBuy, Is.True);
            Assert.That(consumer.OrderBooks.First()
                .Buys.Last()
                .Price, Is.EqualTo(100));
            Assert.That(consumer.OrderBooks.First()
                .Buys.Last()
                .Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Buys.Last()
                .Quantity, Is.EqualTo(1000));
            Assert.That(consumer.OrderBooks.First()
                .Sells, Is.Empty);
        }

        [Test]
        public void two_bids_for_same_symbol_at_different_prices_one_is_amended()
        {
            // ARRANGE
            var consumer = new OrderConsumer(_scheduler);

            var bid1 = new OrderActionEventArgs(Action.Add, new Order(1, "SYMBOL_1", true, 100, 1000));
            var bid2 = new OrderActionEventArgs(Action.Add, new Order(2, "SYMBOL_1", true, 110, 2000));
            var bid3 = new OrderActionEventArgs(Action.Edit, new Order(1, null, true, 110, 2000));

            consumer.StartProcessing(this, _startEventArgs);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ACT
            consumer.HandleOrderAction(this, bid1);
            consumer.HandleOrderAction(this, bid2);
            consumer.HandleOrderAction(this, bid3);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(consumer.OrderBooks.Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Symbol, Is.EqualTo("SYMBOL_1"));
            Assert.That(consumer.OrderBooks.First()
                .Buys, Is.Not.Empty);
            Assert.That(consumer.OrderBooks.First()
                .Buys.Count(), Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .IsBuy, Is.True);
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Price, Is.EqualTo(110));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Count, Is.EqualTo(2));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Quantity, Is.EqualTo(4000));
            Assert.That(consumer.OrderBooks.First()
                .Sells, Is.Empty);
        }

        [Test]
        public void two_bids_for_same_symbol_at_different_prices_one_is_withdrawn()
        {
            // ARRANGE
            var consumer = new OrderConsumer(_scheduler);

            var bid1 = new OrderActionEventArgs(Action.Add, new Order(1, "SYMBOL_1", true, 100, 1000));
            var bid2 = new OrderActionEventArgs(Action.Add, new Order(2, "SYMBOL_1", true, 110, 2000));
            var bid3 = new OrderActionEventArgs(Action.Remove, new Order(1, null, false, 0, 0));

            consumer.StartProcessing(this, _startEventArgs);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ACT
            consumer.HandleOrderAction(this, bid1);
            consumer.HandleOrderAction(this, bid2);
            consumer.HandleOrderAction(this, bid3);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(consumer.OrderBooks.Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Symbol, Is.EqualTo("SYMBOL_1"));
            Assert.That(consumer.OrderBooks.First()
                .Buys, Is.Not.Empty);
            Assert.That(consumer.OrderBooks.First()
                .Buys.Count(), Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .IsBuy, Is.True);
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Price, Is.EqualTo(110));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Quantity, Is.EqualTo(2000));
            Assert.That(consumer.OrderBooks.First()
                .Sells, Is.Empty);
        }

        [Test]
        public void two_bids_for_same_symbol_at_same_price()
        {
            // ARRANGE
            var consumer = new OrderConsumer(_scheduler);

            var bid1 = new OrderActionEventArgs(Action.Add, new Order(1, "SYMBOL_1", true, 100, 1000));
            var bid2 = new OrderActionEventArgs(Action.Add, new Order(2, "SYMBOL_1", true, 100, 2000));

            consumer.StartProcessing(this, _startEventArgs);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ACT
            consumer.HandleOrderAction(this, bid1);
            consumer.HandleOrderAction(this, bid2);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(consumer.OrderBooks.Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Symbol, Is.EqualTo("SYMBOL_1"));
            Assert.That(consumer.OrderBooks.First()
                .Buys, Is.Not.Empty);
            Assert.That(consumer.OrderBooks.First()
                .Buys.Count(), Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .IsBuy, Is.True);
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Price, Is.EqualTo(100));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Count, Is.EqualTo(2));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Quantity, Is.EqualTo(3000));
            Assert.That(consumer.OrderBooks.First()
                .Sells, Is.Empty);
        }

        [Test]
        public void two_bids_for_same_symbol_at_same_prices_one_is_amended()
        {
            // ARRANGE
            var consumer = new OrderConsumer(_scheduler);

            var bid1 = new OrderActionEventArgs(Action.Add, new Order(1, "SYMBOL_1", true, 100, 1000));
            var bid2 = new OrderActionEventArgs(Action.Add, new Order(2, "SYMBOL_1", true, 100, 2000));
            var bid3 = new OrderActionEventArgs(Action.Edit, new Order(2, null, true, 110, 3000));

            consumer.StartProcessing(this, _startEventArgs);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ACT
            consumer.HandleOrderAction(this, bid1);
            consumer.HandleOrderAction(this, bid2);
            consumer.HandleOrderAction(this, bid3);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(consumer.OrderBooks.Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Symbol, Is.EqualTo("SYMBOL_1"));
            Assert.That(consumer.OrderBooks.First()
                .Buys, Is.Not.Empty);
            Assert.That(consumer.OrderBooks.First()
                .Buys.Count(), Is.EqualTo(2));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .IsBuy, Is.True);
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Price, Is.EqualTo(110));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Quantity, Is.EqualTo(3000));
            Assert.That(consumer.OrderBooks.First()
                .Buys.Last()
                .IsBuy, Is.True);
            Assert.That(consumer.OrderBooks.First()
                .Buys.Last()
                .Price, Is.EqualTo(100));
            Assert.That(consumer.OrderBooks.First()
                .Buys.Last()
                .Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Buys.Last()
                .Quantity, Is.EqualTo(1000));
            Assert.That(consumer.OrderBooks.First()
                .Sells, Is.Empty);
        }

        [Test]
        public void two_bids_for_same_symbol_at_same_prices_one_is_withdrawn()
        {
            // ARRANGE
            var consumer = new OrderConsumer(_scheduler);

            var bid1 = new OrderActionEventArgs(Action.Add, new Order(1, "SYMBOL_1", true, 100, 1000));
            var bid2 = new OrderActionEventArgs(Action.Add, new Order(2, "SYMBOL_1", true, 100, 2000));
            var bid3 = new OrderActionEventArgs(Action.Remove, new Order(2, null, false, 0, 0));

            consumer.StartProcessing(this, _startEventArgs);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ACT
            consumer.HandleOrderAction(this, bid1);
            consumer.HandleOrderAction(this, bid2);
            consumer.HandleOrderAction(this, bid3);

            _scheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));

            // ASSERT
            Assert.That(consumer.OrderBooks.Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Symbol, Is.EqualTo("SYMBOL_1"));
            Assert.That(consumer.OrderBooks.First()
                .Buys, Is.Not.Empty);
            Assert.That(consumer.OrderBooks.First()
                .Buys.Count(), Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .IsBuy, Is.True);
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Price, Is.EqualTo(100));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Count, Is.EqualTo(1));
            Assert.That(consumer.OrderBooks.First()
                .Buys.First()
                .Quantity, Is.EqualTo(1000));
            Assert.That(consumer.OrderBooks.First()
                .Sells, Is.Empty);
        }
    }
}