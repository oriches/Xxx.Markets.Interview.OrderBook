using System;
using System.Reactive.Disposables;

namespace Xxx.Markets.Interview.OrderBook.Consumer;

public abstract class DisposableObject : IDisposable
{
    private readonly CompositeDisposable _disposable;

    protected DisposableObject() => _disposable = new CompositeDisposable();

    public void Dispose()
    {
        _disposable.Dispose();
    }

    public static implicit operator CompositeDisposable(DisposableObject disposableObject) =>
        disposableObject._disposable;
}