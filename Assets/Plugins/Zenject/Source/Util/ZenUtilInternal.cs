using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
#if !NOT_UNITY3D
using UnityEngine.SceneManagement;
using UnityEngine;
#endif

namespace Zenject.Internal
{
    public static class ZenUtilInternal
    {
#if UNITY_EDITOR
        static GameObject _disabledIndestructibleGameObject;
#endif

        public static bool IsNull(System.Object obj)
        {
            return obj == null || obj.Equals(null);
        }

#if UNITY_EDITOR
        public static bool IsOutsideUnity()
        {
            return AppDomain.CurrentDomain.FriendlyName != "Unity Child Domain";
        }
#endif

        public static bool AreFunctionsEqual(Delegate left, Delegate right)
        {
            return left.Target == right.Target && left.Method() == right.Method();
        }

        public static int GetInheritanceDelta(Type derived, Type parent)
        {
            Assert.That(derived.DerivesFromOrEqual(parent));

            if (parent.IsInterface())
            {
                return 1;
            }

            if (derived == parent)
            {
                return 0;
            }

            int distance = 1;

            Type child = derived;

            while ((child = child.BaseType()) != parent)
            {
                distance++;
            }

            return distance;
        }

#if !NOT_UNITY3D
        public static IEnumerable<SceneContext> GetAllSceneContexts()
        {
            foreach (var scene in UnityUtil.AllLoadedScenes)
            {
                var contexts = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<SceneContext>()).ToList();

                if (contexts.IsEmpty())
                {
                    continue;
                }

                Assert.That(contexts.Count == 1,
                    "Found multiple scene contexts in scene '{0}'", scene.name);

                yield return contexts[0];
            }
        }

        public static void AddStateMachineBehaviourAutoInjectersInScene(Scene scene)
        {
            foreach (var rootObj in GetRootGameObjects(scene))
            {
                if (rootObj != null)
                {
                    AddStateMachineBehaviourAutoInjectersUnderGameObject(rootObj);
                }
            }
        }

        public static void AddStateMachineBehaviourAutoInjectersUnderGameObject(GameObject root)
        {
#if ZEN_INTERNAL_PROFILING
            using (ProfileTimers.CreateTimedBlock("Searching Hierarchy"))
#endif
            {
                var animators = root.GetComponentsInChildren<Animator>(true);

                foreach (var animator in animators)
                {
                    if (animator.gameObject.GetComponent<ZenjectStateMachineBehaviourAutoInjecter>() == null)
                    {
                        animator.gameObject.AddComponent<ZenjectStateMachineBehaviourAutoInjecter>();
                    }
                }
            }
        }

        public static void GetInjectableMonoBehavioursInScene(
            Scene scene, List<MonoBehaviour> monoBehaviours)
        {
#if ZEN_INTERNAL_PROFILING
            using (ProfileTimers.CreateTimedBlock("Searching Hierarchy"))
#endif
            {
                foreach (var rootObj in GetRootGameObjects(scene))
                {
                    if (rootObj != null)
                    {
                        GetInjectableMonoBehavioursUnderGameObjectInternal(rootObj, monoBehaviours);
                    }
                }
            }
        }

        public static void GetInjectableMonoBehavioursUnderGameObject(
            GameObject gameObject, List<MonoBehaviour> injectableComponents)
        {
#if ZEN_INTERNAL_PROFILING
            using (ProfileTimers.CreateTimedBlock("Searching Hierarchy"))
#endif
            {
                GetInjectableMonoBehavioursUnderGameObjectInternal(gameObject, injectableComponents);
            }
        }

        static void GetInjectableMonoBehavioursUnderGameObjectInternal(
            GameObject gameObject, List<MonoBehaviour> injectableComponents)
        {
            if (gameObject == null)
            {
                return;
            }

            var monoBehaviours = gameObject.GetComponents<MonoBehaviour>();

            for (int i = 0; i < monoBehaviours.Length; i++)
            {
                var monoBehaviour = monoBehaviours[i];

                if (monoBehaviour != null
                        && monoBehaviour.GetType().DerivesFromOrEqual<GameObjectContext>())
                {
                    injectableComponents.Add(monoBehaviour);
                    return;
                }
            }

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var child = gameObject.transform.GetChild(i);

                if (child != null)
                {
                    GetInjectableMonoBehavioursUnderGameObjectInternal(child.gameObject, injectableComponents);
                }
            }

            for (int i = 0; i < monoBehaviours.Length; i++)
            {
                var monoBehaviour = monoBehaviours[i];

                if (monoBehaviour != null
                    && IsInjectableMonoBehaviourType(monoBehaviour.GetType()))
                {
                    injectableComponents.Add(monoBehaviour);
                }
            }
        }

        public static bool IsInjectableMonoBehaviourType(Type type)
        {
            return type != null && !type.DerivesFrom<MonoInstaller>() && TypeAnalyzer.HasInfo(type);
        }

        public static IEnumerable<GameObject> GetRootGameObjects(Scene scene)
        {
#if ZEN_INTERNAL_PROFILING
            using (ProfileTimers.CreateTimedBlock("Searching Hierarchy"))
#endif
            {
                if (scene.isLoaded)
                {
                    return scene.GetRootGameObjects()
                        .Where(x => x.GetComponent<ProjectContext>() == null);
                }

                return Resources.FindObjectsOfTypeAll<GameObject>()
                    .Where(x => x.transform.parent == null
                            && x.GetComponent<ProjectContext>() == null
                            && x.scene == scene);
            }
        }

#if UNITY_EDITOR
        public static Transform GetOrCreateInactivePrefabParent()
        {
            if(_disabledIndestructibleGameObject == null || (!Application.isPlaying && _disabledIndestructibleGameObject.scene != SceneManager.GetActiveScene()))
            {
                var go = new GameObject("ZenUtilInternal_PrefabParent");
                go.hideFlags = HideFlags.HideAndDontSave;
                go.SetActive(false);

                if(Application.isPlaying)
                {
                    UnityEngine.Object.DontDestroyOnLoad(go);
                }

                _disabledIndestructibleGameObject = go;
            }

            return _disabledIndestructibleGameObject.transform;
        }
#endif

#endif
    }
}
