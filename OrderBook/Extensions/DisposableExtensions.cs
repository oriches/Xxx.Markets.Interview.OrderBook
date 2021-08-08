using System;
using System.Reactive.Disposables;

namespace Xxx.Markets.Interview.OrderBook.Extensions
{
    public static class DisposableExtensions
    {
        public static T DisposeWith<T>(this T disposable, CompositeDisposable compositeDisposable) where T : IDisposable
        {
            compositeDisposable.Add(disposable);
            return disposable;
        }
    }
}