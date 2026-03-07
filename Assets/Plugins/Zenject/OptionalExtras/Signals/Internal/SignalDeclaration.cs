using System;
using System.Collections.Generic;
using ModestTree;
#if ZEN_SIGNALS_ADD_UNIRX
using UniRx;
#endif

namespace Zenject
{
    public class SignalDeclaration : ITickable, IDisposable
    {
        readonly List<SignalSubscription> _subscriptions = new List<SignalSubscription>();
        readonly List<object> _asyncQueue = new List<object>();
        readonly BindingId _bindingId;
        readonly SignalMissingHandlerResponses _missingHandlerResponses;
        readonly bool _isAsync;
        readonly ZenjectSettings.SignalSettings _settings;

#if ZEN_SIGNALS_ADD_UNIRX
        readonly Subject<object> _stream = new Subject<object>();
#endif

        public SignalDeclaration(
            SignalDeclarationBindInfo bindInfo,
            [InjectOptional]
            ZenjectSettings zenjectSettings)
        {
            zenjectSettings = zenjectSettings ?? ZenjectSettings.Default;
            _settings = zenjectSettings.Signals ?? ZenjectSettings.SignalSettings.Default;

            _bindingId = new BindingId(bindInfo.SignalType, bindInfo.Identifier);
            _missingHandlerResponses = bindInfo.MissingHandlerResponse;
            _isAsync = bindInfo.RunAsync;
            TickPriority = bindInfo.TickPriority;
        }

#if ZEN_SIGNALS_ADD_UNIRX
        public IObservable<object> Stream
        {
            get { return _stream; }
        }
#endif

		public List<SignalSubscription> Subscriptions => _subscriptions;

        public int TickPriority
        {
            get; private set;
        }

        public bool IsAsync
        {
            get { return _isAsync; }
        }

        public BindingId BindingId
        {
            get { return _bindingId; }
        }

        public void Dispose()
        {
            if (_settings.RequireStrictUnsubscribe)
            {
                Assert.That(_subscriptions.IsEmpty(),
                    "Found {0} signal handlers still added to declaration {1}", _subscriptions.Count, _bindingId);
            }
            else
            {
                for (int i = 0; i < _subscriptions.Count; i++)
                {
                    _subscriptions[i].OnDeclarationDespawned();
                }
            }
        }

        public void Fire(object signal)
        {
            Assert.That(signal.GetType().DerivesFromOrEqual(_bindingId.Type));

            if (_isAsync)
            {
                _asyncQueue.Add(signal);
            }
            else
            {
                using (var block = DisposeBlock.Spawn())
                {
                    var subscriptions = block.SpawnList<SignalSubscription>();
                    subscriptions.AddRange(_subscriptions);
                    FireInternal(subscriptions, signal);
                }
            }
        }

        void FireInternal(List<SignalSubscription> subscriptions, object signal)
        {
            if (subscriptions.IsEmpty()
#if ZEN_SIGNALS_ADD_UNIRX
                && !_stream.HasObservers
#endif
                )
            {
                if (_missingHandlerResponses == SignalMissingHandlerResponses.Warn)
                {
                    Log.Warn("Fired signal '{0}' but no subscriptions found!  If this is intentional then either add OptionalSubscriber() to the binding or change the default in ZenjectSettings", signal.GetType());
                }
                else if (_missingHandlerResponses == SignalMissingHandlerResponses.Throw)
                {
                    throw Assert.CreateException(
                        "Fired signal '{0}' but no subscriptions found!  If this is intentional then either add OptionalSubscriber() to the binding or change the default in ZenjectSettings", signal.GetType());
                }
            }

            for (int i = 0; i < subscriptions.Count; i++)
            {
                var subscription = subscriptions[i];

                if (_subscriptions.Contains(subscription))
                {
                    subscription.Invoke(signal);
                }
            }

#if ZEN_SIGNALS_ADD_UNIRX
            _stream.OnNext(signal);
#endif
        }

        public void Tick()
        {
            Assert.That(_isAsync);

            if (!_asyncQueue.IsEmpty())
            {
                using (var block = DisposeBlock.Spawn())
                {
                    var subscriptions = block.SpawnList<SignalSubscription>();
                    subscriptions.AddRange(_subscriptions);

                    var signals = block.SpawnList<object>();
                    signals.AddRange(_asyncQueue);

                    _asyncQueue.Clear();

                    for (int i = 0; i < signals.Count; i++)
                    {
                        FireInternal(subscriptions, signals[i]);
                    }
                }
            }
        }

        public void Add(SignalSubscription subscription)
        {
            Assert.That(!_subscriptions.Contains(subscription));
            _subscriptions.Add(subscription);
        }

        public void Remove(SignalSubscription subscription)
        {
            _subscriptions.RemoveWithConfirm(subscription);
        }

        public class Factory : PlaceholderFactory<SignalDeclarationBindInfo, SignalDeclaration>
        {
        }
    }
}
