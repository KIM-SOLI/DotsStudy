using System;
using System.Collections.Generic;
#if USING_PLATFORMS_PACKAGE
using Unity.Build;
#endif
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hash128 = Unity.Entities.Hash128;

namespace Unity.Scenes.Editor
{

    /// <summary>
    /// Utility methods for handling subscene behavior
    /// </summary>
    public static class SubSceneExUtility
    {
        /// <summary>
        /// Marks a set of subscenes as editable if possible
        /// </summary>
        /// <param name="scenes">The list of subscenes to mark as editable</param>
        public static void EditScene(params SubSceneEx[] scenes)
        {
            foreach (var subScene in scenes)
            {
                if (SubSceneExInspectorUtility.CanEditScene(subScene))
                {
                    Scene scene;
                    if (Application.isPlaying)
                        scene = EditorSceneManager.LoadSceneInPlayMode(subScene.EditableScenePath, new LoadSceneParameters(LoadSceneMode.Additive));
                    else
                        scene = EditorSceneManager.OpenScene(subScene.EditableScenePath, OpenSceneMode.Additive);
                    SubSceneExInspectorUtility.SetSceneAsSubScene(scene);
                }
            }
        }
    }

    [InitializeOnLoad]
    internal static class SubSceneExInspectorUtility
    {
        internal delegate void RepaintAction();

        internal static event RepaintAction WantsRepaint;

        static SubSceneExInspectorUtility()
        {
            Unsupported.SetUsingAuthoringScenes(true);
        }

        public static Transform GetUncleanHierarchyObject(SubSceneEx[] subscenes)
        {
            foreach (var scene in subscenes)
            {
                var res = GetUncleanHierarchyObject(scene.transform);
                if (res != null)
                    return res;
            }

            return null;
        }

        public static Transform GetUncleanHierarchyObject(Transform child)
        {
            while (child)
            {
                if (child.localPosition != Vector3.zero)
                    return child;
                if (child.localRotation != Quaternion.identity)
                    return child;
                if (child.localScale != Vector3.one)
                    return child;

                child = child.parent;
            }

            return null;
        }

        public static bool HasChildren(SubSceneEx[] scenes)
        {
            foreach (var scene in scenes)
            {
                if (scene.transform.childCount != 0)
                    return true;
            }

            return false;
        }

        public static void CloseSceneWithoutSaving(params SubSceneEx[] scenes)
        {
            foreach (var scene in scenes)
                EditorSceneManager.CloseScene(scene.EditingScene, true);
        }

        public struct LoadableScene
        {
            public Entity Scene;
            public string Name;
            public SubSceneEx SubSceneEx;
            public int SectionIndex;
            public bool IsLoaded;
            public bool Section0IsLoaded;
            public int NumSubSceneSectionsLoaded;
        }

        static unsafe NativeArray<Entity> GetActiveWorldSections(World world, Hash128 sceneGUID)
        {
            if (world == null || !world.IsCreated) return default;

            var sceneSystem = world.GetExistingSystem<SceneSystem>();
            var statePtr = world.Unmanaged.ResolveSystemState(sceneSystem);
            if (statePtr == null)
                return default;

            var entities = world.EntityManager;

            var sceneEntity = SceneSystem.GetSceneEntity(world.Unmanaged, sceneGUID);

            if (!entities.HasComponent<ResolvedSectionEntity>(sceneEntity))
                return default;

            return entities.GetBuffer<ResolvedSectionEntity>(sceneEntity).Reinterpret<Entity>().AsNativeArray();
        }

        public static SubSceneExInspectorUtility.LoadableScene[] GetLoadableScenes(SubSceneEx[] scenes)
        {
            var loadables = new List<SubSceneExInspectorUtility.LoadableScene>();
            DefaultWorldInitialization.DefaultLazyEditModeInitialize(); // workaround for occasional null World at this point
            var world = World.DefaultGameObjectInjectionWorld;
            var entityManager = world.EntityManager;
            foreach (var scene in scenes)
            {
                bool section0IsLoaded = false;
                var numSections = 0;
                var numSectionsLoaded = 0;
                foreach (var section in GetActiveWorldSections(world, scene.SceneGUID))
                {
                    if (entityManager.HasComponent<SceneSectionData>(section))
                    {
                        var name = scene.SceneAsset != null ? scene.SceneAsset.name : "Missing Scene Asset";
                        var sectionIndex = entityManager.GetComponentData<SceneSectionData>(section).SubSectionIndex;
                        if (sectionIndex != 0)
                            name += $" Section: {sectionIndex}";

                        numSections += 1;
                        var isLoaded = entityManager.HasComponent<RequestSceneLoaded>(section);
                        if (isLoaded)
                            numSectionsLoaded += 1;
                        if (sectionIndex == 0)
                            section0IsLoaded = isLoaded;

                        loadables.Add(new SubSceneExInspectorUtility.LoadableScene
                        {
                            Scene = section,
                            Name = name,
                            SubSceneEx = scene,
                            SectionIndex = sectionIndex,
                            IsLoaded = isLoaded,
                            Section0IsLoaded = section0IsLoaded,
                        });
                    }
                }

                // Go over all sections of this SubSceneEx and set the number of sections that are loaded.
                // This is needed to decide whether are able to unload section 0.
                for (int i = 0; i < numSections; i++)
                {
                    var idx = numSections - 1 - i;
                    var l = loadables[idx];
                    l.NumSubSceneSectionsLoaded = numSectionsLoaded;
                    loadables[idx] = l;
                }
            }

            return loadables.ToArray();
        }

        public static unsafe void ForceReimport(params SubSceneEx[] scenes)
        {
            bool needRefresh = false;
            foreach (var world in World.All)
            {
                var sceneSystem = world.GetExistingSystem<SceneSystem>();
                var statePtr = world.Unmanaged.ResolveSystemState(sceneSystem);
                if (statePtr != null)
                {
                    var buildConfigGuid = world.EntityManager.GetComponentData<SceneSystemData>(sceneSystem).BuildConfigurationGUID;
                    foreach (var scene in scenes)
                        needRefresh |= SceneWithBuildConfigurationGUIDs.Dirty(scene.SceneGUID, buildConfigGuid);
                }
            }
            if (needRefresh)
                AssetDatabase.Refresh();
        }

        public static bool CanEditScene(SubSceneEx SubSceneEx)
        {
            if (!SubSceneEx.CanBeLoaded())
                return false;

            return !SubSceneEx.IsLoaded;
        }

        public static void SetSceneAsSubScene(Scene scene)
        {
            scene.isSubScene = true;
        }

        public static void CloseAndAskSaveIfUserWantsTo(params SubSceneEx[] subScenes)
        {
            if (!Application.isPlaying)
            {
                var dirtyScenes = new List<Scene>();
                foreach (var scene in subScenes)
                {
                    if (scene.EditingScene.isLoaded && scene.EditingScene.isDirty)
                    {
                        dirtyScenes.Add(scene.EditingScene);
                    }
                }

                if (dirtyScenes.Count != 0)
                {
                    if (!EditorSceneManager.SaveModifiedScenesIfUserWantsTo(dirtyScenes.ToArray()))
                        return;
                }

                CloseSceneWithoutSaving(subScenes);
            }
            else
            {
                foreach (var scene in subScenes)
                {
                    if (scene.EditingScene.isLoaded)
                        EditorSceneManager.UnloadSceneAsync(scene.EditingScene);
                }
            }
        }

        public static void SaveScene(SubSceneEx scene)
        {
            if (scene.EditingScene.isLoaded && scene.EditingScene.isDirty)
            {
                EditorSceneManager.SaveScene(scene.EditingScene);
            }
        }

        public static MinMaxAABB GetActiveWorldMinMax(World world, UnityEngine.Object[] targets)
        {
            MinMaxAABB bounds = MinMaxAABB.Empty;

            if (world == null)
                return bounds;

            var entities = world.EntityManager;
            foreach (SubSceneEx SubSceneEx in targets)
            {
                foreach (var section in GetActiveWorldSections(World.DefaultGameObjectInjectionWorld, SubSceneEx.SceneGUID))
                {
                    if (entities.HasComponent<SceneBoundingVolume>(section))
                        bounds.Encapsulate(entities.GetComponentData<SceneBoundingVolume>(section).Value);
                }
            }

            return bounds;
        }

        // Visualize SubSceneEx using bounding volume when it is selected.
        public static void DrawSubsceneBounds(SubSceneEx scene)
        {
            var isEditing = scene.IsLoaded;

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
                return;

            var entities = world.EntityManager;
            foreach (var section in GetActiveWorldSections(World.DefaultGameObjectInjectionWorld, scene.SceneGUID))
            {
                if (!entities.HasComponent<SceneBoundingVolume>(section))
                    continue;

                if (isEditing)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.gray;

                AABB aabb = entities.GetComponentData<SceneBoundingVolume>(section).Value;
                Gizmos.DrawWireCube(aabb.Center, aabb.Size);
            }
        }

        /// <summary>
        /// Forces a Repaint event on the <see cref="SubSceneInspector"/> editor which are currently active.
        /// </summary>
        internal static void RepaintSubSceneInspector()
        {
            WantsRepaint?.Invoke();
        }
    }
}
