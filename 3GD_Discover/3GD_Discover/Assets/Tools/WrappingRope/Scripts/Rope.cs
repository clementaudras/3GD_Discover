#define DEBUG
#undef DEBUG
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using WrappingRopeLibrary.Enums;

namespace WrappingRopeLibrary.Scripts
{

    public class Rope : RopeBase 
    {

#if UNITY_EDITOR
        [MenuItem("GameObject/Wrapping Rope", false, 10)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject rope = new GameObject("Rope");
            rope.AddComponent<Rope>();
            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(rope, null);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(rope, "Create " + rope.name);
            Selection.activeObject = rope;
        }
#endif
        void FixedUpdate()
        {
            MainUpdate();
        }


        void Update()
        {
    #if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                if (CheckAndUpdateByChanges())
                    MainUpdate();
            }
    #endif
            if (Body == BodyType.FiniteSegments)
                Texturing();
        }


        protected bool CheckAndUpdateByChanges()
        {
            if (FrontEnd == null || BackEnd == null)
            {
                return false;
            }
            if (FrontPiece == null && !TryInitPieceSystem())
            {
                return false;
            }
            return true;
        }

    }
}