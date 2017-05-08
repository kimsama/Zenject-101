using System;
using System.Collections.Generic;
using ModestTree;
using ModestTree.Util;

#if ZEN_SIGNALS_ADD_UNIRX
using UniRx;
#endif

namespace Zenject
{
    // This is just used for generic constraints
    public interface ISignal<TParam1, TParam2> : ISignalBase
    {
        void Fire(TParam1 p1, TParam2 p2);

        void Unlisten(Action<TParam1, TParam2> listener);
        void Listen(Action<TParam1, TParam2> listener);
    }

    public abstract class Signal<TParam1, TParam2, TDerived> : SignalBase, ISignal<TParam1, TParam2>
        where TDerived : Signal<TParam1, TParam2, TDerived>
#if ENABLE_IL2CPP
        // See discussion here for why we do this: https://github.com/modesttree/Zenject/issues/219#issuecomment-284751679
        where TParam1 : class
        where TParam2 : class
#endif
    {
        readonly List<Action<TParam1, TParam2>> _listeners = new List<Action<TParam1, TParam2>>();

#if ZEN_SIGNALS_ADD_UNIRX
        readonly Subject<Tuple<TParam1, TParam2>> _observable = new Subject<Tuple<TParam1, TParam2>>();
#endif

#if ZEN_SIGNALS_ADD_UNIRX
        public IObservable<Tuple<TParam1, TParam2>> AsObservable
        {
            get
            {
                return _observable;
            }
        }
#endif

        public int NumListeners
        {
            get { return _listeners.Count; }
        }

        public void Listen(Action<TParam1, TParam2> listener)
        {
            Assert.That(!_listeners.Contains(listener),
                () => "Tried to add method '{0}' to signal '{1}' but it has already been added"
                .Fmt(listener.ToDebugString(), this.GetType()));
            _listeners.Add(listener);
        }

        public void Unlisten(Action<TParam1, TParam2> listener)
        {
            bool success = _listeners.Remove(listener);
            Assert.That(success,
                () => "Tried to remove method '{0}' from signal '{1}' without adding it first"
                .Fmt(listener.ToDebugString(), this.GetType()));
        }

        public static TDerived operator + (Signal<TParam1, TParam2, TDerived> signal, Action<TParam1, TParam2> listener)
        {
            signal.Listen(listener);
            return (TDerived)signal;
        }

        public static TDerived operator - (Signal<TParam1, TParam2, TDerived> signal, Action<TParam1, TParam2> listener)
        {
            signal.Unlisten(listener);
            return (TDerived)signal;
        }

        public void Fire(TParam1 p1, TParam2 p2)
        {
#if UNITY_EDITOR
            using (ProfileBlock.Start("Signal '{0}'", this.GetType().Name))
#endif
            {
                var wasHandled = Manager.Trigger(SignalId, new object[] { p1, p2 });

                wasHandled |= !_listeners.IsEmpty();

                // Use ToArray in case they remove in the handler
                foreach (var listener in _listeners.ToArray())
                {
#if UNITY_EDITOR
                    using (ProfileBlock.Start(listener.ToDebugString()))
#endif
                    {
                        listener(p1, p2);
                    }
                }

#if ZEN_SIGNALS_ADD_UNIRX
                wasHandled |= _observable.HasObservers;
#if UNITY_EDITOR
                using (ProfileBlock.Start("UniRx Stream"))
#endif
                {
                    _observable.OnNext(Tuple.Create(p1, p2));
                }
#endif

                if (Settings.RequiresHandler && !wasHandled)
                {
                    throw Assert.CreateException(
                        "Signal '{0}' was fired but no handlers were attached and the signal is marked to require a handler!", SignalId);
                }
            }
        }
    }
}

