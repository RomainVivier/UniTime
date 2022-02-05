using System;
using System.Collections.Generic;

namespace Kratorg.Internal
{
    public static class Broadcaster<T>
    {
        private static Dictionary<int, IObservable<T>> _domainName  = new Dictionary<int, IObservable<T>>();

        public static void Add(int key, IObservable<T> observable)
        {
            if (_domainName.ContainsKey(key))
            {
                return;
            }

            _domainName.Add(key, observable);
        }

        public static void Remove(int key)
        {
            if (_domainName.ContainsKey(key) == false)
            {
                return;
            }

            _domainName.Remove(key);
        }

        public static IDisposable Subscribe(int key, IObserver<T> observer)
        {
            if (_domainName.ContainsKey(key) == false)
            {
                return new EmptyDisposable();
            }

            return _domainName[key].Subscribe(observer);
        }
    }

    public class EmptyDisposable : IDisposable
    {
        public void Dispose() { }
    }

}