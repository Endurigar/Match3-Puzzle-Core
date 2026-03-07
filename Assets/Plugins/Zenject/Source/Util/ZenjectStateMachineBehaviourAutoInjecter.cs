using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class ZenjectStateMachineBehaviourAutoInjecter : MonoBehaviour
    {
        DiContainer _container;
        Animator _animator;

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
            _animator = GetComponent<Animator>();
            Assert.IsNotNull(_animator);
        }

        public void Start()
        {
            if (_animator != null)
            {
                var behaviours = _animator.GetBehaviours<StateMachineBehaviour>();

                if (behaviours != null)
                {
                    foreach (var behaviour in behaviours)
                    {
                        _container.Inject(behaviour);
                    }
                }
            }
        }
    }
}
