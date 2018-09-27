using UnityEngine;
using System.Collections;
using WrappingRopeLibrary.Scripts;
using WrappingRopeLibrary.Enums;

namespace WrappingRope.Demo
{
    public class GUIPanel : MonoBehaviour
    {
        public GameObject Rope;
        private GameObject _rope;
        private Rope _ropeEntity;

        // Use this for initialization
        void Start()
        {

        }


        private bool _isRope;

        void OnGUI()
        {
            TestControls();
        }


        private void TestControls()
        {

            GUILayout.BeginArea(new Rect(10, 10, 200, 300));
            if (GUILayout.Button(_isRope ? "Destroy rope" : "Instantiate rope"))
            {
                if (_isRope)
                {
                    Destroy(_rope);
                    _ropeEntity = null;
                }
                else
                {
                    _rope = Instantiate(Rope) as GameObject;
                    _ropeEntity = _rope.GetComponent<Rope>();
                }
                _isRope = !_isRope;

            }
            if (_isRope && _ropeEntity != null)
            {
                GUILayout.BeginHorizontal("box");
                _ropeEntity.Width = GUILayout.HorizontalScrollbar(_ropeEntity.Width, 0.03F, 0.01F, 0.3F, GUILayout.Width(100));
                GUILayout.Label(string.Format("Rope width: {0}", Mathf.Round(_ropeEntity.Width * 100f) / 100f));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal("box");
                if (GUILayout.RepeatButton("Cut front end"))
                {
                    _ropeEntity.CutRope(0.2f, Direction.FrontToBack);
                }
                if (GUILayout.RepeatButton("Cut back end"))
                {
                    _ropeEntity.CutRope(0.2f, Direction.BackToFront);
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginVertical("box");
                GUILayout.Label("Select texturing mode:");
                if (GUILayout.Button("None"))
                {
                    _ropeEntity.TexturingMode = TexturingMode.None;
                }
                if (GUILayout.Button("Stretched"))
                {
                    _ropeEntity.TexturingMode = TexturingMode.Stretched;
                }
                if (GUILayout.Button("TiledFromFrontEnd"))
                {
                    _ropeEntity.TexturingMode = TexturingMode.TiledFromFrontEnd;
                }
                if (GUILayout.Button("TiledFromBackEnd"))
                {
                    _ropeEntity.TexturingMode = TexturingMode.TiledFromBackEnd;
                }
                GUILayout.EndVertical();

            }
            GUILayout.EndArea();
        }


        // Update is called once per frame
        void Update()
        {

        }
    }

}
