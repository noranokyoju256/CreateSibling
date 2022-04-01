using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Noranokyoju.Editor
{
    internal static class CreateSibling
    {
        [MenuItem("GameObject/Create Empty Sibling %&n")]
        private static void CreateEmptySibling()
        { 
            var selectedObject = Selection.activeGameObject;
            if (!selectedObject)
            {
                return;
            }
            var parent = selectedObject.transform.parent;
            if (!parent)
            {
                return;
            }
            Place(ObjectFactory.CreateGameObject("GameObject"), parent.gameObject);
        }
   
        private static void SetGameObjectParent(GameObject go, Transform parentTransform)
        {
            var transform = go.transform;
            Undo.SetTransformParent(transform, parentTransform, "Reparenting");
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            go.layer = parentTransform.gameObject.layer;

            if (parentTransform.GetComponent<RectTransform>())
                ObjectFactory.AddComponent<RectTransform>(go);
        }

        private static void Place(GameObject go, GameObject parent)
        {
            SetGameObjectParent(go, parent.transform);
            GameObjectUtility.EnsureUniqueNameForSibling(go);
            Undo.SetCurrentGroupName("Create " + go.name);
            var shwType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            EditorWindow.FocusWindowIfItsOpen(shwType);
            Selection.activeGameObject = go;
            shwType.InvokeMember("FrameAndRenameNewGameObject", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod , null, null, new object[] { });
        }
    }
}