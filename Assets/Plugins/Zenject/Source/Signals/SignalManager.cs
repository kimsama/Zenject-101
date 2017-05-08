using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;
using ModestTree.Util;

namespace Zenject
{
    public class SignalManager : ILateDisposable
    {
        readonly Dictionary<BindingId, List<ISignalHandler>> _signalHandlers = new Dictionary<BindingId, List<ISignalHandler>>();

        public int GetNumHandlers(BindingId signalType)
        {
            return GetList(signalType).Count();
        }

        public bool IsHandlerRegistered(BindingId signalType)
        {
            return !GetList(signalType).IsEmpty();
        }

        List<ISignalHandler> GetList(BindingId signalType)
        {
            List<ISignalHandler> handlers;

            if (!_signalHandlers.TryGetValue(signalType, out handlers))
            {
                handlers = new List<ISignalHandler>();
                _signalHandlers.Add(signalType, handlers);
            }

            return handlers;
        }

        public void Register(BindingId signalType, ISignalHandler handler)
        {
            GetList(signalType).Add(handler);
        }

        public void Unregister(BindingId signalType, ISignalHandler handler)
        {
            GetList(signalType).RemoveWithConfirm(handler);
        }

        public void LateDispose()
        {
            Assert.Warn(_signalHandlers.Values.SelectMany(x => x).IsEmpty(),
                "Found signals still registered on SignalManager");
        }

        // Returns true the signal was handled
        public bool Trigger(BindingId signalType, object[] args)
        {
            var handlers = GetList(signalType);

            if (handlers.IsEmpty())
            {
                return false;
            }

#if UNITY_EDITOR
            using (ProfileBlock.Start("Signal '{0}'", signalType))
#endif
            {
                foreach (var handler in handlers)
                {
                    handler.Execute(args);
                }
            }

            return true;
        }
    }
}
