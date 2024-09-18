using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Athena.Editor
{
    public class LayerMaskEx
    {
        /// <summary>
        /// Create a layer at the next available index. Returns silently if layer already exists.
        /// </summary>
        /// <param name="name">Name of the layer to create</param>
        public static void CreateLayer(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name", "New layer name string is either null or empty.");

            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layerProps = tagManager.FindProperty("layers");
            var propCount = layerProps.arraySize;

            SerializedProperty firstEmptyProp = null;

            for (var i = 0; i < propCount; i++)
            {
                var layerProp = layerProps.GetArrayElementAtIndex(i);

                var stringValue = layerProp.stringValue;

                if (stringValue == name) return;

                if (i < 8 || stringValue != string.Empty) continue;

                if (firstEmptyProp == null)
                    firstEmptyProp = layerProp;
            }

            if (firstEmptyProp == null)
            {
                UnityEngine.Debug.LogError("Maximum limit of " + propCount + " layers exceeded. Layer \"" + name + "\" not created.");
                return;
            }

            firstEmptyProp.stringValue = name;
            tagManager.ApplyModifiedProperties();
        }

        // will check if the specified layer names are present and add any missing ones
        // it will simply add them from the first open slots so do not depend on any
        // specific order but rather grab layers from the layer names at runtime
        public static void CreateLayers(string[] layerNames)
        {
            SerializedObject manager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
#if !UNITY_4
            SerializedProperty layersProp = manager.FindProperty("layers");
#endif

            foreach (string name in layerNames)
            {
                // check if layer is present
                bool found = false;
                for (int i = 0; i <= 31; i++)
                {
#if UNITY_4
                    string nm = "User Layer " + i;
                    SerializedProperty sp = manager.FindProperty(nm);
#else
                    SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
#endif
                    if (sp != null && name.Equals(sp.stringValue))
                    {
                        found = true;
                        break;
                    }
                }

                // not found, add into 1st open slot
                if (!found)
                {
                    SerializedProperty slot = null;
                    for (int i = 8; i <= 31; i++)
                    {
#if UNITY_4
                        string nm = "User Layer " + i;
                        SerializedProperty sp = manager.FindProperty(nm);
#else
                        SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
#endif
                        if (sp != null && string.IsNullOrEmpty(sp.stringValue))
                        {
                            slot = sp;
                            break;
                        }
                    }

                    if (slot != null)
                    {
                        slot.stringValue = name;
                    }
                    else
                    {
                        Debug.LogError("Could not find an open Layer Slot for: " + name);
                    }
                }
            }

            // save
            manager.ApplyModifiedProperties();
        }

        public static void CheckTags(string[] tagNames)
        {
            SerializedObject manager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = manager.FindProperty("tags");

            List<string> DefaultTags = new List<string>() { "Untagged", "Respawn", "Finish", "EditorOnly", "MainCamera", "Player", "GameController" };

            foreach (string name in tagNames)
            {
                if (DefaultTags.Contains(name)) continue;

                // check if tag is present
                bool found = false;
                for (int i = 0; i < tagsProp.arraySize; i++)
                {
                    SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                    if (t.stringValue.Equals(name)) { found = true; break; }
                }

                // if not found, add it
                if (!found)
                {
                    tagsProp.InsertArrayElementAtIndex(0);
                    SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                    n.stringValue = name;
                }
            }

            // save
            manager.ApplyModifiedProperties();
        }

        public static void CreateSortingLayers(string[] layers)
        {
            SerializedObject manager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty sortLayersProp = manager.FindProperty("m_SortingLayers");

            //for (int i = 0; i < sortLayersProp.arraySize; i++)
            //{ // used to figure out how all of this works and what properties values look like
            //    SerializedProperty entry = sortLayersProp.GetArrayElementAtIndex(i);
            //    SerializedProperty name = entry.FindPropertyRelative("name");
            //    SerializedProperty unique = entry.FindPropertyRelative("uniqueID");
            //    SerializedProperty locked = entry.FindPropertyRelative("locked");
            //    Debug.Log(name.stringValue + " => " + unique.intValue + " => " + locked.boolValue);
            //}

            foreach (string name in layers)
            {
                // check if tag is present
                bool found = false;
                for (int i = 0; i < sortLayersProp.arraySize; i++)
                {
                    SerializedProperty entry = sortLayersProp.GetArrayElementAtIndex(i);
                    SerializedProperty t = entry.FindPropertyRelative("name");
                    if (t.stringValue.Equals(name)) { found = true; break; }
                }

                // if not found, add it
                if (!found)
                {
                    manager.ApplyModifiedProperties();
                    AddNewSortingLayer();
                    manager.Update();

                    int idx = sortLayersProp.arraySize - 1;
                    SerializedProperty entry = sortLayersProp.GetArrayElementAtIndex(idx);
                    SerializedProperty t = entry.FindPropertyRelative("name");
                    t.stringValue = name;
                }
            }

            // save
            manager.ApplyModifiedProperties();
        }

        // you need 'using System.Reflection;' for these
        private static Assembly editorAsm;
        private static MethodInfo AddSortingLayer_Method;

        /// <summary> add a new sorting layer with default name </summary>
        public static void AddNewSortingLayer()
        {
            if (AddSortingLayer_Method == null)
            {
                if (editorAsm == null) editorAsm = Assembly.GetAssembly(typeof(UnityEditor.Editor));
                System.Type t = editorAsm.GetType("UnityEditorInternal.InternalEditorUtility");
                AddSortingLayer_Method = t.GetMethod("AddSortingLayer", (BindingFlags.Static | BindingFlags.NonPublic), null, new System.Type[0], null);
            }
            AddSortingLayer_Method.Invoke(null, null);
        }
    }
}
