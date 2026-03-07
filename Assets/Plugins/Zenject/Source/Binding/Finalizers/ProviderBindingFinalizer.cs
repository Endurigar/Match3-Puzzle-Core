using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Zenject.Internal;

namespace Zenject
{
    [NoReflectionBaking]
    public abstract class ProviderBindingFinalizer : IBindingFinalizer
    {
        public ProviderBindingFinalizer(BindInfo bindInfo)
        {
            BindInfo = bindInfo;
        }

        public BindingInheritanceMethods BindingInheritanceMethod
        {
            get { return BindInfo.BindingInheritanceMethod; }
        }

        protected BindInfo BindInfo
        {
            get;
            private set;
        }

        protected ScopeTypes GetScope()
        {
            if (BindInfo.Scope == ScopeTypes.Unset)
            {
                Assert.That(!BindInfo.RequireExplicitScope || BindInfo.Condition != null,
                    "Scope must be set for the previous binding!  Please either specify AsTransient, AsCached, or AsSingle. Last binding: Contract: {0}, Identifier: {1} {2}",
                    BindInfo.ContractTypes.Select(x => x.PrettyName()).Join(", "), BindInfo.Identifier,
                    BindInfo.ContextInfo != null ? "Context: '{0}'".Fmt(BindInfo.ContextInfo) : "");
                return ScopeTypes.Transient;
            }

            return BindInfo.Scope;
        }

        public void FinalizeBinding(DiContainer container)
        {
            if (BindInfo.ContractTypes.Count == 0)
            {
                return;
            }

            try
            {
                OnFinalizeBinding(container);
            }
            catch (Exception e)
            {
                throw Assert.CreateException(
                    e, "Error while finalizing previous binding! Contract: {0}, Identifier: {1} {2}",
                    BindInfo.ContractTypes.Select(x => x.PrettyName()).Join(", "), BindInfo.Identifier,
                    BindInfo.ContextInfo != null ? "Context: '{0}'".Fmt(BindInfo.ContextInfo) : "");
            }
        }

        protected abstract void OnFinalizeBinding(DiContainer container);

        protected void RegisterProvider<TContract>(
            DiContainer container, IProvider provider)
        {
            RegisterProvider(container, typeof(TContract), provider);
        }

        protected void RegisterProvider(
            DiContainer container, Type contractType, IProvider provider)
        {
            if (BindInfo.OnlyBindIfNotBound && container.HasBindingId(contractType, BindInfo.Identifier))
            {
                return;
            }

            container.RegisterProvider(
                new BindingId(contractType, BindInfo.Identifier),
                BindInfo.Condition,
                provider, BindInfo.NonLazy);

            if (contractType.IsValueType() && !(contractType.IsGenericType() && contractType.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(contractType);

                container.RegisterProvider(
                    new BindingId(nullableType, BindInfo.Identifier),
                    BindInfo.Condition,
                    provider, BindInfo.NonLazy);
            }
        }

        protected void RegisterProviderPerContract(
            DiContainer container, Func<DiContainer, Type, IProvider> providerFunc)
        {
            foreach (var contractType in BindInfo.ContractTypes)
            {
                var provider = providerFunc(container, contractType);

                if (BindInfo.MarkAsUniqueSingleton)
                {
                    container.SingletonMarkRegistry.MarkSingleton(contractType);
                }
                else if (BindInfo.MarkAsCreationBinding)
                {
                    container.SingletonMarkRegistry.MarkNonSingleton(contractType);
                }

                RegisterProvider(container, contractType, provider);
            }
        }

        protected void RegisterProviderForAllContracts(
            DiContainer container, IProvider provider)
        {
            foreach (var contractType in BindInfo.ContractTypes)
            {
                if (BindInfo.MarkAsUniqueSingleton)
                {
                    container.SingletonMarkRegistry.MarkSingleton(contractType);
                }
                else if (BindInfo.MarkAsCreationBinding)
                {
                    container.SingletonMarkRegistry.MarkNonSingleton(contractType);
                }

                RegisterProvider(container, contractType, provider);
            }
        }

        protected void RegisterProvidersPerContractAndConcreteType(
            DiContainer container,
            List<Type> concreteTypes,
            Func<Type, Type, IProvider> providerFunc)
        {
            Assert.That(!BindInfo.ContractTypes.IsEmpty());
            Assert.That(!concreteTypes.IsEmpty());

            foreach (var contractType in BindInfo.ContractTypes)
            {
                foreach (var concreteType in concreteTypes)
                {
                    if (ValidateBindTypes(concreteType, contractType))
                    {
                        RegisterProvider(container, contractType, providerFunc(contractType, concreteType));
                    }
                }
            }
        }

        bool ValidateBindTypes(Type concreteType, Type contractType)
        {
            bool isConcreteOpenGenericType = concreteType.IsOpenGenericType();
            bool isContractOpenGenericType = contractType.IsOpenGenericType();
            if (isConcreteOpenGenericType != isContractOpenGenericType)
            {
                return false;
            }

#if !(UNITY_WSA && ENABLE_DOTNET)

            if (isContractOpenGenericType)
            {
                Assert.That(isConcreteOpenGenericType);

                if (TypeExtensions.IsAssignableToGenericType(concreteType, contractType))
                {
                    return true;
                }
            }
            else if (concreteType.DerivesFromOrEqual(contractType))
            {
                return true;
            }
#else
            if (concreteType.DerivesFromOrEqual(contractType))
            {
                return true;
            }
#endif

            if (BindInfo.InvalidBindResponse == InvalidBindResponses.Assert)
            {
                throw Assert.CreateException(
                    "Expected type '{0}' to derive from or be equal to '{1}'", concreteType, contractType);
            }

            Assert.IsEqual(BindInfo.InvalidBindResponse, InvalidBindResponses.Skip);
            return false;
        }

        protected void RegisterProvidersForAllContractsPerConcreteType(
            DiContainer container,
            List<Type> concreteTypes,
            Func<DiContainer, Type, IProvider> providerFunc)
        {
            Assert.That(!BindInfo.ContractTypes.IsEmpty());
            Assert.That(!concreteTypes.IsEmpty());

            var providerMap = ZenPools.SpawnDictionary<Type, IProvider>();
            try
            {
                foreach (var concreteType in concreteTypes)
                {
                    var provider = providerFunc(container, concreteType);

                    providerMap[concreteType] = provider;

                    if (BindInfo.MarkAsUniqueSingleton)
                    {
                        container.SingletonMarkRegistry.MarkSingleton(concreteType);
                    }
                    else if (BindInfo.MarkAsCreationBinding)
                    {
                        container.SingletonMarkRegistry.MarkNonSingleton(concreteType);
                    }
                }

                foreach (var contractType in BindInfo.ContractTypes)
                {
                    foreach (var concreteType in concreteTypes)
                    {
                        if (ValidateBindTypes(concreteType, contractType))
                        {
                            RegisterProvider(container, contractType, providerMap[concreteType]);
                        }
                    }
                }
            }
            finally
            {
                ZenPools.DespawnDictionary(providerMap);
            }
        }
    }
}
