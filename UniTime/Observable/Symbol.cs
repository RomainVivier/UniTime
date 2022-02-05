using System;
using System.Collections.Generic;

namespace Kratorg.Internal
{
    [Serializable]
    public class Symbol<T> : IObservable<T>, IEquatable<T>, IDisposable
    {
        protected static readonly IEqualityComparer<T> Comparer = EqualityComparer<T>.Default;

        event Action<T> valueChanged;

        protected bool _disposed;
        protected T _value;

        public virtual T Value
        {
            get => _value;
            set
            {
                if (_disposed || Comparer.Equals(_value, value))
                {
                    return;
                }

                _value = value;
                valueChanged?.Invoke(value);
            }
        }

        public Symbol(T baseValue)
        {
            _disposed = false;
            Value = baseValue;
        }

        protected void Notify()
        {
            valueChanged?.Invoke(Value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_disposed)
            {
                return null;
            }

            valueChanged += observer.OnNext;
            return new Unsubscriber(() => Unsubscribe(observer));
        }

        public void Unsubscribe(IObserver<T> observer)
        {
            if (_disposed || observer == null)
            {
                return;
            }

            observer.OnCompleted();

            valueChanged -= observer.OnNext;
        }

        public bool Equals(T other)
        {
            return Comparer.Equals(Value, other);
        }

        public static bool operator ==(Symbol<T> first, Symbol<T> second)
        {
            return first.Equals(second.Value);
        }

        public static bool operator !=(Symbol<T> first, Symbol<T> second)
        {
            return !first.Equals(second.Value);
        }

        public static bool operator ==(Symbol<T> first, T second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(Symbol<T> first, T second)
        {
            return !first.Equals(second);
        }

        public override bool Equals(object obj)
        {
            if (obj is Symbol<T> other)
            {
                return Equals(other.Value);
            }
            else if (obj is T other2)
            {
                return Equals(other2);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Value == null ? 42 : Value.GetHashCode();
        }

        public virtual void Clear()
        {
            valueChanged = null;
            Value = default;
        }

        public virtual void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Clear();

            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }

    public sealed class Unsubscriber : IDisposable
    {
        bool _disposed = false;

        Action  _unsubscribe;

        public Unsubscriber(Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public void Dispose()
        {
            if (_disposed || _unsubscribe != null)
            {
                return;
            }

            _disposed = true;

            _unsubscribe();
            
            GC.SuppressFinalize(this);
        }
    }
}