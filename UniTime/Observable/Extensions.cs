using System;

namespace Kratorg.Internal
{
    public static class ActionExtensions
    {
        public static IObservable<T> AsObservable<T>(this Action<T> action) where T : MulticastDelegate
        {
            return new Bridge<T>(action);
        }
    }

    public static class ObservableExtensions
    {
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
        {
            return source.Subscribe(new Subscribe<T>(onNext));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
        {
            return source.Subscribe(new Subscribe<T>(onNext, null, onError));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action onCompleted)
        {
            return source.Subscribe(new Subscribe<T>(onNext, onCompleted));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return source.Subscribe(new Subscribe<T>(onNext, onCompleted, onError));
        }
    }
}