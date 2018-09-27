using UnityEngine;
using WrappingRopeLibrary.Scripts;

namespace WrappingRope.Demo
{
    public class GUIElastic : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

            _rope = Rope as GameObject;
            if (_rope != null)
                _ropeEntity = _rope.GetComponent<Rope>();

        }

        // Update is called once per frame
        void Update()
        {

        }


        public GameObject Rope;
        private GameObject _rope;
        private Rope _ropeEntity;


        void OnGUI()
        {
            TestControls();
        }


        private void TestControls()
        {
            GUILayout.BeginArea(new Rect(10, 10, 200, 300));
            GUILayout.BeginHorizontal("box");

            if (_ropeEntity != null)
            {
                _ropeEntity.ElasticModulus = GUILayout.HorizontalScrollbar(_ropeEntity.ElasticModulus, 0.02F, 0.12F, 1.2F, GUILayout.Width(100));
                GUILayout.Label(string.Format("Elastic Modulus: {0}", Mathf.Round(_ropeEntity.ElasticModulus * 100f) / 100f));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button("Create object"))
            {
                PrimitiveType primitive = PrimitiveType.Cube;
                var rndVal = Random.value;
                if (rndVal < 0.5f)
                    primitive = PrimitiveType.Cube;
                //else if (rndVal < 0.7)
                //    primitive = PrimitiveType.Sphere;
                else if (rndVal < 1)
                    primitive = PrimitiveType.Cylinder;
                //primitive = PrimitiveType.Sphere;
                NewObject(primitive);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }


        private void NewObject(PrimitiveType primitive)
        {
            float a = 0.1f + Random.value * 0.9f;
            float b = 0.3f + Random.value * 0.7f;
            float c = 2 + Random.value * 0.9f;
            float x = 3 + Random.value * 2;
            float y = 3f;
            float z = -1.4f + Random.value * 0.8f;
            GameObject gObj = null;
            switch (primitive)
            {
                case PrimitiveType.Cube:
                    gObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    gObj.transform.localScale = new Vector3(a, b, c);
                    Destroy(gObj.GetComponent<BoxCollider>());
                    break;
                //case PrimitiveType.Sphere:
                //    gObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //    gObj.transform.localScale = new Vector3(c * 0.6f, c * 0.6f, c * 0.6f);
                //    Destroy(gObj.GetComponent<SphereCollider>());
                //    break;
                case PrimitiveType.Cylinder:
                    gObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    gObj.transform.localScale = new Vector3(a, b, a);
                    gObj.transform.Rotate(new Vector3(90, 0, 0));
                    Destroy(gObj.GetComponent<CapsuleCollider>());
                    break;
            }
            if (gObj == null)
                return;
            gObj.transform.position = new Vector3(x, y, z);
            gObj.AddComponent<Rigidbody>();
            var collider = gObj.AddComponent<MeshCollider>();
            collider.convex = true;

        }
    }
}
