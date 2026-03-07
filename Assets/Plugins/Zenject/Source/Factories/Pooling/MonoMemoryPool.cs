using UnityEngine;

namespace Zenject
{
    public class MonoMemoryPool<TValue> : MemoryPool<TValue>
        where TValue : Component
    {
        Transform _originalParent;

        [Inject]
        public MonoMemoryPool()
        {
        }

        protected override void OnCreated(TValue item)
        {
            item.gameObject.SetActive(false);
            _originalParent = item.transform.parent;
        }

        protected override void OnDestroyed(TValue item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnSpawned(TValue item)
        {
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(TValue item)
        {
            item.gameObject.SetActive(false);

            if (item.transform.parent != _originalParent)
            {
                item.transform.SetParent(_originalParent, false);
            }
        }
    }

    public class MonoMemoryPool<TParam1, TValue> : MemoryPool<TParam1, TValue>
        where TValue : Component
    {
        Transform _originalParent;

        [Inject]
        public MonoMemoryPool()
        {
        }

        protected override void OnCreated(TValue item)
        {
            item.gameObject.SetActive(false);
            _originalParent = item.transform.parent;
        }

        protected override void OnDestroyed(TValue item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnSpawned(TValue item)
        {
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(TValue item)
        {
            item.gameObject.SetActive(false);

            if (item.transform.parent != _originalParent)
            {
                item.transform.SetParent(_originalParent, false);
            }
        }
    }

    public class MonoMemoryPool<TParam1, TParam2, TValue>
        : MemoryPool<TParam1, TParam2, TValue>
        where TValue : Component
    {
        Transform _originalParent;

        [Inject]
        public MonoMemoryPool()
        {
        }

        protected override void OnCreated(TValue item)
        {
            item.gameObject.SetActive(false);
            _originalParent = item.transform.parent;
        }

        protected override void OnDestroyed(TValue item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnSpawned(TValue item)
        {
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(TValue item)
        {
            item.gameObject.SetActive(false);

            if (item.transform.parent != _originalParent)
            {
                item.transform.SetParent(_originalParent, false);
            }
        }
    }

    public class MonoMemoryPool<TParam1, TParam2, TParam3, TValue>
        : MemoryPool<TParam1, TParam2, TParam3, TValue>
        where TValue : Component
    {
        Transform _originalParent;

        [Inject]
        public MonoMemoryPool()
        {
        }

        protected override void OnCreated(TValue item)
        {
            item.gameObject.SetActive(false);
            _originalParent = item.transform.parent;
        }

        protected override void OnDestroyed(TValue item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnSpawned(TValue item)
        {
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(TValue item)
        {
            item.gameObject.SetActive(false);

            if (item.transform.parent != _originalParent)
            {
                item.transform.SetParent(_originalParent, false);
            }
        }
    }

    public class MonoMemoryPool<TParam1, TParam2, TParam3, TParam4, TValue>
        : MemoryPool<TParam1, TParam2, TParam3, TParam4, TValue>
        where TValue : Component
    {
        Transform _originalParent;

        [Inject]
        public MonoMemoryPool()
        {
        }

        protected override void OnCreated(TValue item)
        {
            item.gameObject.SetActive(false);
            _originalParent = item.transform.parent;
        }

        protected override void OnDestroyed(TValue item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnSpawned(TValue item)
        {
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(TValue item)
        {
            item.gameObject.SetActive(false);

            if (item.transform.parent != _originalParent)
            {
                item.transform.SetParent(_originalParent, false);
            }
        }
    }

    public class MonoMemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TValue>
        : MemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TValue>
        where TValue : Component
    {
        Transform _originalParent;

        [Inject]
        public MonoMemoryPool()
        {
        }

        protected override void OnCreated(TValue item)
        {
            item.gameObject.SetActive(false);
            _originalParent = item.transform.parent;
        }

        protected override void OnDestroyed(TValue item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnSpawned(TValue item)
        {
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(TValue item)
        {
            item.gameObject.SetActive(false);

            if (item.transform.parent != _originalParent)
            {
                item.transform.SetParent(_originalParent, false);
            }
        }
    }
}
