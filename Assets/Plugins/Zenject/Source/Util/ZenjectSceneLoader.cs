#if !NOT_UNITY3D

using System;
using ModestTree;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zenject
{
    public enum LoadSceneRelationship
    {
        None,
        Child,
        Sibling
    }

    public class ZenjectSceneLoader
    {
        readonly ProjectKernel _projectKernel;
        readonly DiContainer _sceneContainer;

        public ZenjectSceneLoader(
            [InjectOptional]
            SceneContext sceneRoot,
            ProjectKernel projectKernel)
        {
            _projectKernel = projectKernel;
            _sceneContainer = sceneRoot == null ? null : sceneRoot.Container;
        }

        public void LoadScene(
            string sceneName,
            LoadSceneMode loadMode = LoadSceneMode.Single,
            Action<DiContainer> extraBindings = null,
            LoadSceneRelationship containerMode = LoadSceneRelationship.None,
            Action<DiContainer> extraBindingsLate = null)
        {
            PrepareForLoadScene(loadMode, extraBindings, extraBindingsLate, containerMode);

            Assert.That(Application.CanStreamedLevelBeLoaded(sceneName),
                "Unable to load scene '{0}'", sceneName);

            SceneManager.LoadScene(sceneName, loadMode);

        }

            public AsyncOperation LoadSceneAsync(
            string sceneName,
            LoadSceneMode loadMode = LoadSceneMode.Single,
            Action<DiContainer> extraBindings = null,
            LoadSceneRelationship containerMode = LoadSceneRelationship.None,
            Action<DiContainer> extraBindingsLate = null)
        {
            PrepareForLoadScene(loadMode, extraBindings, extraBindingsLate, containerMode);

            Assert.That(Application.CanStreamedLevelBeLoaded(sceneName),
                "Unable to load scene '{0}'", sceneName);

            return SceneManager.LoadSceneAsync(sceneName, loadMode);
        }

        void PrepareForLoadScene(
            LoadSceneMode loadMode,
            Action<DiContainer> extraBindings,
            Action<DiContainer> extraBindingsLate,
            LoadSceneRelationship containerMode)
        {
            if (loadMode == LoadSceneMode.Single)
            {
                Assert.IsEqual(containerMode, LoadSceneRelationship.None);

                _projectKernel.ForceUnloadAllScenes();
            }

            if (containerMode == LoadSceneRelationship.None)
            {
                SceneContext.ParentContainers = null;
            }
            else if (containerMode == LoadSceneRelationship.Child)
            {
                if (_sceneContainer == null)
                {
                    SceneContext.ParentContainers = null;
                }
                else
                {
                    SceneContext.ParentContainers = new[] { _sceneContainer };
                }
            }
            else
            {
                Assert.IsNotNull(_sceneContainer,
                    "Cannot use LoadSceneRelationship.Sibling when loading scenes from ProjectContext");
                Assert.IsEqual(containerMode, LoadSceneRelationship.Sibling);
                SceneContext.ParentContainers = _sceneContainer.ParentContainers;
            }

            SceneContext.ExtraBindingsInstallMethod = extraBindings;
            SceneContext.ExtraBindingsLateInstallMethod = extraBindingsLate;
        }

        public void LoadScene(
            int sceneIndex,
            LoadSceneMode loadMode = LoadSceneMode.Single,
            Action<DiContainer> extraBindings = null,
            LoadSceneRelationship containerMode = LoadSceneRelationship.None,
            Action<DiContainer> extraBindingsLate = null)
        {
            PrepareForLoadScene(loadMode, extraBindings, extraBindingsLate, containerMode);

            Assert.That(Application.CanStreamedLevelBeLoaded(sceneIndex),
                "Unable to load scene '{0}'", sceneIndex);

            SceneManager.LoadScene(sceneIndex, loadMode);

        }

        public AsyncOperation LoadSceneAsync(
            int sceneIndex,
            LoadSceneMode loadMode = LoadSceneMode.Single,
            Action<DiContainer> extraBindings = null,
            LoadSceneRelationship containerMode = LoadSceneRelationship.None,
            Action<DiContainer> extraBindingsLate = null)
        {
            PrepareForLoadScene(loadMode, extraBindings, extraBindingsLate, containerMode);

            Assert.That(Application.CanStreamedLevelBeLoaded(sceneIndex),
                "Unable to load scene '{0}'", sceneIndex);

            return SceneManager.LoadSceneAsync(sceneIndex, loadMode);
        }
    }
}

#endif
