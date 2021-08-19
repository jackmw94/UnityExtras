using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtras.Code.Core
{
    public static class EditorUtilities
    {
#if UNITY_EDITOR
    
        [MenuItem("Tools/Force layout rebuild")]
        public static void ForceLayoutRebuild()
        {
            GameObject selectedObject = Selection.activeGameObject;
            if (!selectedObject)
            {
                Debug.LogError("No object was selected!");
                return;
            }

            RectTransform selectedRectTransform = selectedObject.GetComponent<RectTransform>();
            if (!selectedRectTransform)
            {
                Debug.LogError("Selected object has no rect transform component");
                return;
            }
        
            LayoutRebuilder.ForceRebuildLayoutImmediate(selectedRectTransform);
            Debug.Log($"Forced layout rebuild on {selectedObject}");
        }
#endif
    }
}