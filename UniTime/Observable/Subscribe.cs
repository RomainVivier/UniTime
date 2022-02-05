using System;

namespace Kratorg.Internal
{
    class Subscribe<T> : IObserver<T>
    {
        object lockObject = new object();

        readonly Action<T> onNext;
        readonly Action<Exception> onError;
        readonly Action onCompleted;

        public Subscribe(Action<T> onNext, Action onCompleted = null, Action<Exception> onError = null)
        {
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
        }

        public void OnNext(T value)
        {
            onNext?.Invoke(value);
        }

        public void OnError(Exception error)
        {
            onError?.Invoke(error);
        }

        public void OnCompleted()
        {
            onCompleted?.Invoke();
        }
    }
}