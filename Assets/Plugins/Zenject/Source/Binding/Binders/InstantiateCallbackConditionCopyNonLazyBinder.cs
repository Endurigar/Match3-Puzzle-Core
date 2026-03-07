using System;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class InstantiateCallbackConditionCopyNonLazyBinder : ConditionCopyNonLazyBinder
    {
        public InstantiateCallbackConditionCopyNonLazyBinder(BindInfo bindInfo)
            : base(bindInfo)
        {
        }

        public ConditionCopyNonLazyBinder OnInstantiated(
            Action<InjectContext, object> callback)
        {
            BindInfo.InstantiatedCallback = callback;
            return this;
        }

        public ConditionCopyNonLazyBinder OnInstantiated<T>(
            Action<InjectContext, T> callback)
        {

            BindInfo.InstantiatedCallback = (ctx, obj) =>
            {
                Assert.That(obj == null || obj is T,
                    "Invalid generic argument to OnInstantiated! {0} must be type {1}", obj.GetType(), typeof(T));

                callback(ctx, (T)obj);
            };
            return this;
        }
    }
}
