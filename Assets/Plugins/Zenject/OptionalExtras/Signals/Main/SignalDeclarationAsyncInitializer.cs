using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public class SignalDeclarationAsyncInitializer : IInitializable
    {
        readonly LazyInject<TickableManager> _tickManager;
        readonly List<SignalDeclaration> _declarations;

        public SignalDeclarationAsyncInitializer(
            [Inject(Source = InjectSources.Local)]
            List<SignalDeclaration> declarations,
            [Inject(Optional = true, Source = InjectSources.Local)]
            LazyInject<TickableManager> tickManager)
        {
            _declarations = declarations;
            _tickManager = tickManager;
        }

        public void Initialize()
        {
            for (int i = 0; i < _declarations.Count; i++)
            {
                var declaration = _declarations[i];

                if (declaration.IsAsync)
                {
                    Assert.IsNotNull(_tickManager.Value, "TickableManager is required when using asynchronous signals");
                    _tickManager.Value.Add(declaration, declaration.TickPriority);
                }
            }
        }
    }
}

