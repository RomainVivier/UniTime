using System;
using UnityEngine;

namespace Kratorg.Internal.Times
{
    public class Stopwatch : Clock, IObserver<double>
    {
        protected IObservable<double> _source;
        protected IDisposable _unsubscriber;

        public Stopwatch(IObservable<double> source) => _source = source;

        public virtual void Start()
        {
            _span = TimeSpan.Zero;
            isPaused.Value  = false;
            _unsubscriber   = _source.Subscribe(this);
        }

        public void OnNext(double value) => Update(value);

        public virtual void OnCompleted()
        {
            isPaused.Value = true;
            _unsubscriber?.Dispose();
        }

        public void OnError(Exception error) => Debug.Log(error.Message);

        public override void OnDispose()
        {
            _unsubscriber?.Dispose();
            base.OnDispose();
        }
    }
}