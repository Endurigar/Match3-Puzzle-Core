#if !NOT_UNITY3D

using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine.SceneManagement;

namespace Zenject
{
    public class ProjectKernel : MonoKernel
    {
        [Inject]
        ZenjectSettings _settings = null;

        [Inject]
        SceneContextRegistry _contextRegistry = null;

        public void OnApplicationQuit()
        {
            if (_settings.EnsureDeterministicDestructionOrderOnApplicationQuit)
            {
                DestroyEverythingInOrder();
            }
        }

        public void DestroyEverythingInOrder()
        {
            ForceUnloadAllScenes(true);

            Assert.That(!IsDestroyed);
            DestroyImmediate(gameObject);
            Assert.That(IsDestroyed);
        }

        public void ForceUnloadAllScenes(bool immediate = false)
        {
            Assert.That(!IsDestroyed);

            var sceneOrder = new List<Scene>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                sceneOrder.Add(SceneManager.GetSceneAt(i));
            }

            foreach (var sceneContext in _contextRegistry.SceneContexts.OrderByDescending(x => sceneOrder.IndexOf(x.gameObject.scene)).ToList())
            {
                if (immediate)
                {
                    DestroyImmediate(sceneContext.gameObject);
                }
                else
                {
                    Destroy(sceneContext.gameObject);
                }
            }
        }
    }
}

#endif
