using System.Linq;
using System.Collections.Generic;
using Unity.Scenes;
using Unity.Scenes.Editor;
using UnityEngine;
using SubSceneUtility = Unity.Scenes.Editor.SubSceneUtility;

namespace UnityEditor.UI
{
    internal static class SubSceneExHierarchyOverlay
    {
        static class Styles
        {
            public static float subSceneEditingButtonWidth = 16f;
            public static GUIContent subSceneEditingTooltip = EditorGUIUtility.TrTextContent(string.Empty, "Toggle whether the Sub Scene is open for editing.");
        }

        internal static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject != null)
            {
                SubSceneEx SubSceneEx;
                if (gameObject.TryGetComponent(out SubSceneEx))
                {
                    if (!SubSceneEx.CanBeLoaded())
                        return;

                    if (PrefabUtility.IsOutermostPrefabInstanceRoot(SubSceneEx.gameObject))
                        return;

                    var evt = Event.current;
                    Rect buttonRect = selectionRect;
                    buttonRect.x = buttonRect.xMax;
                    buttonRect.width = Styles.subSceneEditingButtonWidth;

                    var loaded = SubSceneEx.EditingScene.isLoaded;
                    var wantsLoaded = EditorGUI.Toggle(buttonRect, loaded);
                    if (wantsLoaded != loaded)
                    {
                        SubSceneEx[] subScenes;
                        var selectedSubScenes = Selection.GetFiltered<SubSceneEx>(SelectionMode.TopLevel);
                        if (selectedSubScenes.Contains(SubSceneEx))
                            subScenes = selectedSubScenes;
                        else
                            subScenes = new[] { SubSceneEx };

                        if (wantsLoaded)
                        {
                            SubSceneExUtility.EditScene(subScenes);
                        }
                        else
                        {
                            // find child scenes
                            HashSet<SubSceneEx> seenSubScene = new HashSet<SubSceneEx>();
                            List<SubSceneEx> subscenesToUnload = new List<SubSceneEx>();

                            Stack<SubSceneEx> subSceneStack = new Stack<SubSceneEx>();
                            foreach (SubSceneEx ss in subScenes)
                                subSceneStack.Push(ss);

                            while (subSceneStack.Count > 0)
                            {
                                SubSceneEx itr = subSceneStack.Pop();
                                if (seenSubScene.Contains(itr) || !itr.EditingScene.isLoaded)
                                    continue;

                                seenSubScene.Add(itr);
                                subscenesToUnload.Add(itr);

                                if (itr.SceneAsset != null)
                                {
                                    foreach (GameObject ssGameObject in itr.EditingScene.GetRootGameObjects())
                                    {
                                        foreach (SubSceneEx childSubScene in ssGameObject.GetComponentsInChildren<SubSceneEx>())
                                            subSceneStack.Push(childSubScene);
                                    }
                                }
                            }

                            // process children before parents
                            subScenes = subscenesToUnload.ToArray();
                            System.Array.Reverse(subScenes);

                            SubSceneExInspectorUtility.CloseAndAskSaveIfUserWantsTo(subScenes);
                        }

                        // When opening or closing the scene from the hierarchy, the scene does not become dirty.
                        // Because of that, the SubSceneInspector cannot refresh itself automatically and update the
                        // state of the selected subscenes.
                        SubSceneExInspectorUtility.RepaintSubSceneInspector();
                    }

                    if (buttonRect.Contains(evt.mousePosition))
                    {
                        GUI.Label(buttonRect, Styles.subSceneEditingTooltip);
                    }
                }
            }
        }
    }
}
