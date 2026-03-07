using System;
using ModestTree;

namespace Zenject
{
    public class BindSignalFromBinder<TObject, TSignal>
    {
        readonly BindStatement _bindStatement;
        readonly Func<TObject, Action<TSignal>> _methodGetter;
        readonly DiContainer _container;
        readonly SignalBindingBindInfo _signalBindInfo;

        public BindSignalFromBinder(
            SignalBindingBindInfo signalBindInfo, BindStatement bindStatement, Func<TObject, Action<TSignal>> methodGetter,
            DiContainer container)
        {
            _signalBindInfo = signalBindInfo;
            _bindStatement = bindStatement;
            _methodGetter = methodGetter;
            _container = container;
        }

        public SignalCopyBinder FromResolve()
        {
            return From(x => x.FromResolve().AsCached());
        }

        public SignalCopyBinder FromResolveAll()
        {
            return From(x => x.FromResolveAll().AsCached());
        }

        public SignalCopyBinder FromNew()
        {
            return From(x => x.FromNew().AsCached());
        }

        public SignalCopyBinder From(Action<ConcreteBinderGeneric<TObject>> objectBindCallback)
        {
            Assert.That(!_bindStatement.HasFinalizer);
            _bindStatement.SetFinalizer(new NullBindingFinalizer());

            var objectLookupId = Guid.NewGuid();

            var objectBinder = _container.BindNoFlush<TObject>().WithId(objectLookupId);

            objectBindCallback(objectBinder);

            Func<object, Action<object>> methodGetterMapper =
                obj => s => _methodGetter((TObject)obj)((TSignal)s);

            var wrapperBinder = _container.Bind<IDisposable>()
                .To<SignalCallbackWithLookupWrapper>()
                .AsCached()
                .WithArguments(_signalBindInfo, typeof(TObject), objectLookupId, methodGetterMapper)
                .NonLazy();

            var copyBinder = new SignalCopyBinder( wrapperBinder.BindInfo);
            copyBinder.AddCopyBindInfo(objectBinder.BindInfo);
            return copyBinder;
        }
    }
}
