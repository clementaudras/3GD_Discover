using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace WrappingRope.Demo
{
    public class GUICharacter : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 200, 220));

            GUILayout.BeginHorizontal("box");
            GUILayout.BeginVertical();
            GUILayout.Label("Please, make sure that a Rope script is executed before a CharacterRopeInteraction script in the Script Execution Order settings.");
            GUILayout.Label("W - move forward, S - move backward, A - rotate to left, D - rotate to right, SPACE - jump");

            if (GUILayout.Button("Reset"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

        }
    }
}
