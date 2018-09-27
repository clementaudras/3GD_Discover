using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;
using WrappingRopeLibrary.ExtensionMethods;
using WrappingRopeLibrary.Model;
using WrappingRopeLibrary.Utils;
using WrappingRopeLibrary.Enums;
using WrappingRopeLibrary.Scripts;

namespace WrappingRopeLibrary.Editors
{
    [CustomEditor(typeof(Rope))]
    public class RopeEditor : Editor
    {

        private RopeBase _rope;
        private bool isPlolygonWindowVisible;
        private RenderTexture _texture;
        private bool _isPrefab;

        void OnEnable()
        {
            //Tools.hidden = true;
            _texture = new RenderTexture(1000, 100, 16, RenderTextureFormat.ARGB32);
            _rope = (RopeBase)target;
            if (_rope == null)
                return;
            _isPrefab = PrefabUtility.GetPrefabType(_rope) == PrefabType.Prefab;
        }


        void OnDisable()
        {
           // Tools.hidden = false;
        }

        public override void OnInspectorGUI()
        {
            _needRefreshBackBandPoint = false;
            _needRefreshFrontBandPoint = false;
            _needRefreshPiecesProfilesAndSections = false;
            _needRefreshBendPointInstances = false;
            _canUndo = true;
            var materialProcessed = false;
            serializedObject.Update();
 

            EditorGUI.BeginDisabledGroup(_isPrefab && _rope.FrontPiece != null);
            var oldBody = serializedObject.FindProperty("_body").enumValueIndex;
            var body
                = UpdateParameter(
                    "Body"
                    , oldBody
                    , (v) => { return Convert.ToInt32(EditorGUILayout.EnumPopup((BodyType)v)); }
                    , true);
            serializedObject.FindProperty("_body").enumValueIndex = body;
            if (oldBody != body)
            {
                //Undo.SetCurrentGroupName("Change and rebuild");
                //var gr = Undo.GetCurrentGroup();
                //Undo.RegisterCompleteObjectUndo(_rope, "Body type change");
                // rope call
                DoBodyTypeChange(body);
                _canUndo = false;
                //Undo.CollapseUndoOperations(gr);
            }
            EditorGUI.EndDisabledGroup();
            if ((BodyType)body == BodyType.Continuous) 
            {
                var oldMaterial = serializedObject.FindProperty("_material").objectReferenceValue;
                var newMaterial 
                    = UpdateParameter(
                    "Material"
                    , oldMaterial
                    , (v) => { return EditorGUILayout.ObjectField(v, typeof(Material), true); }
                    , true);
                if (!materialProcessed)
                {
                    serializedObject.FindProperty("_material").objectReferenceValue = newMaterial;
                    if (oldMaterial != newMaterial)
                    {
                        DoMaterialChange(newMaterial as Material);
                        materialProcessed = true;
                    }
                }
            }

            var workMaterialProp = serializedObject.FindProperty("_workMaterial");
            var ropeMaterialProp = serializedObject.FindProperty("_material");
            var rend = _rope.GetComponent<MeshRenderer>();

            //Синхронизируем материал рендера и веревки только если материал веревки не изменялся в редакторе и это не префаб
            if (rend != null && !materialProcessed)
            {
                if (!_isPrefab)
                {
                    if (rend.sharedMaterial != null && rend.sharedMaterial != workMaterialProp.objectReferenceValue)
                    {
                        // Копируем материал из рендерера
                        ropeMaterialProp.objectReferenceValue = rend.sharedMaterial;
                        DoMaterialChange(rend.sharedMaterial);
                        materialProcessed = true;
                    }
                    if (rend.sharedMaterial == null)
                    {
                        DoMaterialChange(ropeMaterialProp.objectReferenceValue as Material);
                        materialProcessed = true;
                    }
                }
            }

            EditorGUI.BeginDisabledGroup(_isPrefab);
            var frontEndProp = serializedObject.FindProperty("_frontEnd");
            var oldFrontEnd = frontEndProp.objectReferenceValue;
            var newFrontEnd
                = UpdateParameter(
                    "Front End"
                    , oldFrontEnd
                    , (v) => { return EditorGUILayout.ObjectField(v, typeof(GameObject), true); }
                    , true);
            frontEndProp.objectReferenceValue = newFrontEnd;
            if (oldFrontEnd != newFrontEnd)
            {
                DoFrontEndChange(newFrontEnd);
                _canUndo = false;
            }

            var backEndProp = serializedObject.FindProperty("_backEnd");
            var oldBackEnd = backEndProp.objectReferenceValue;
            var newBackEnd
                = UpdateParameter(
                    "Back End"
                    , oldBackEnd
                    , (v) => { return EditorGUILayout.ObjectField(v, typeof(GameObject), true); }
                    , true);
            backEndProp.objectReferenceValue = newBackEnd;
            if (oldBackEnd != newBackEnd)
            {
                DoBackEndChange(newBackEnd);
                _canUndo = false;
            }

            EditorGUI.EndDisabledGroup();

            var rect = GUILayoutUtility.GetLastRect();
            rect.x = 0;
            rect.y += 14;
            rect.width = EditorGUIUtility.currentViewWidth * 0.35f + 14f;
            var oldThresholdProp = serializedObject.FindProperty("Threshold");
            var newThreshold
                = UpdateParameter(
                    "Threshold"
                    , oldThresholdProp.floatValue
                    , (v) => { return MyFloatFieldInternal(GUILayoutUtility.GetRect(60f, 16f), rect, v, new GUIStyle(GUI.skin.GetStyle("textField"))); }
                    , true);
            if (newThreshold < 0.01f)
                newThreshold = 0.01f;
            oldThresholdProp.floatValue = newThreshold;


            EditorGUI.BeginDisabledGroup(_isPrefab);
            var oldWidth = serializedObject.FindProperty("_width").floatValue;
            rect = GUILayoutUtility.GetLastRect();
            rect.x = 0;
            rect.y += 14;
            rect.width = EditorGUIUtility.currentViewWidth * 0.35f + 14f;
            var width
                = UpdateParameter(
                    "Width"
                    , oldWidth
                    , (v) => { return MyFloatFieldInternal(GUILayoutUtility.GetRect(60f, 16f), rect, v, new GUIStyle(GUI.skin.GetStyle("textField"))); }
                    , true);

            if (width < 0.001f)
                width = 0.001f;
            serializedObject.FindProperty("_width").floatValue = width;
            if (oldWidth != width)
            {
                //Undo.SetCurrentGroupName("Width change.");
                //var gr = Undo.GetCurrentGroup();
                //Undo.RegisterCompleteObjectUndo(_rope, "Width change.");
                DoWidthChange(width);
                //Undo.CollapseUndoOperations(gr);
            }

            var oldWrapDistance = serializedObject.FindProperty("WrapDistance").floatValue;
            var newWrapDistance 
                = UpdateParameter(
                    "WrapDistance"
                    , oldWrapDistance
                    , (v) => { return EditorGUILayout.Slider(v, 0.0005f, width); }
                    , true);
            serializedObject.FindProperty("WrapDistance").floatValue = newWrapDistance;
            if (oldWrapDistance != newWrapDistance)
            {
                DoChangeWrapDistance(newWrapDistance);
                _canUndo = false;
            }
            EditorGUI.EndDisabledGroup();

            if ((BodyType)body != BodyType.Continuous)
            {
                EditorGUI.BeginDisabledGroup(_isPrefab);

                var oldPieceInstanceProp = serializedObject.FindProperty("_pieceInstance");
                var oldPieceInstance = oldPieceInstanceProp.objectReferenceValue;
                var newPieceInstance
                    = UpdateParameter(
                    "Piece Instance"
                    , oldPieceInstance
                    , (v) => { return EditorGUILayout.ObjectField(v, typeof(GameObject), true); }
                    , true);
                oldPieceInstanceProp.objectReferenceValue = newPieceInstance;
                if (oldPieceInstance != newPieceInstance)
                {
                    DoPieceInstanceChange((BodyType)body, !_isPrefab);
                    _canUndo = false;
                }

                var oldBendInstanceProp = serializedObject.FindProperty("BendInstance");
                var oldBendInstance = oldBendInstanceProp.objectReferenceValue;
                var newBendInstance
                    = UpdateParameter(
                        "BendInstance"
                        , oldBendInstance
                        , (v) => { return EditorGUILayout.ObjectField(v, typeof(GameObject), true); }
                        , false);
                oldBendInstanceProp.objectReferenceValue = newBendInstance;
                if (oldBendInstance != newBendInstance)
                {
                    DoBendInstandeChange(newBendInstance);
                    _canUndo = false;
                }
                EditorGUI.EndDisabledGroup();
            }


            if ((BodyType)body == BodyType.FiniteSegments)
            {
                var oldExtendAxis = serializedObject.FindProperty("ExtendAxis").enumValueIndex;
                var newExtendAxis
                    = UpdateParameter(
                        "Extend Axis"
                        , oldExtendAxis
                        , (v) => { return Convert.ToInt32(EditorGUILayout.EnumPopup((Axis)v)); }
                        , false);

                serializedObject.FindProperty("ExtendAxis").enumValueIndex = newExtendAxis;
                if (oldExtendAxis != newExtendAxis)
                    DoExtendAxisChange((Axis)newExtendAxis);
            }

            serializedObject.FindProperty("TexturingMode").enumValueIndex
                = UpdateParameter(
                    "Texturing Mode"
                    , serializedObject.FindProperty("TexturingMode").enumValueIndex
                    , (v) => { return Convert.ToInt32(EditorGUILayout.EnumPopup((TexturingMode)v)); }
                    , false);

            serializedObject.FindProperty("UVLocation").enumValueIndex
                = UpdateParameter(
                    "UV Location"
                    , serializedObject.FindProperty("UVLocation").enumValueIndex
                    , (v) => { return Convert.ToInt32(EditorGUILayout.EnumPopup((UVLocation)v)); }
                    , false);

            var anchoringModeProp = serializedObject.FindProperty("_anchoringMode");
            var oldAnchoringMode = anchoringModeProp.enumValueIndex;
            var newAnchoringMode
                = UpdateParameter(
                    "Anchoring Mode"
                    , oldAnchoringMode
                    , (v) => { return Convert.ToInt32(EditorGUILayout.EnumPopup((AnchoringMode)v)); }
                    , false);
            anchoringModeProp.enumValueIndex = newAnchoringMode;
            if (oldAnchoringMode != newAnchoringMode)
            {
                _rope.AnchoringMode = (AnchoringMode)newAnchoringMode;
                _canUndo = false;
            }

            serializedObject.FindProperty("ElasticModulus").floatValue
                = UpdateParameter(
                    "Elastic Modulus"
                    , serializedObject.FindProperty("ElasticModulus").floatValue
                    , (v) => { return EditorGUILayout.Slider(v, 0.001f, 3f); }
                    , false);

            LayerMaskField();

            if ((BodyType)body == BodyType.Continuous)
                ShowMeshConfiguration();
            if (_canUndo) serializedObject.ApplyModifiedProperties();
            else serializedObject.ApplyModifiedPropertiesWithoutUndo();

            if (_needRefreshPiecesProfilesAndSections)
                RebuildPiecesProfilesAndSections();

            if (_needRefreshFrontBandPoint)
            {
                if (_rope.FrontPiece != null)
                {
                    _rope.FrontPiece.FrontBandPoint = GetWrapPointByGameObject(_rope.FrontEnd);
                    _rope.FrontPiece.Relocate(false);
                }
            }


            if (_needRefreshBackBandPoint)
            {
                if (_rope.BackPiece != null)
                {
                    _rope.BackPiece.BackBandPoint = GetWrapPointByGameObject(_rope.BackEnd);
                    _rope.BackPiece.Relocate(false);
                }
            }

            if(_needRefreshBendPointInstances)
                RefreshBendPointInstance((BodyType)body);

            //serializedObject.ApplyModifiedProperties();

        }

        private bool _needRefreshBendPointInstances;
        private bool _canUndo;

        private void DoBendInstandeChange(Object newBendInstance)
        {
            _needRefreshBendPointInstances = !_isPrefab;
            var bendInstanceRationProp = serializedObject.FindProperty("_bendInstanceRatio");
            if (newBendInstance == null)
                bendInstanceRationProp.floatValue = 1f;
            else
            {
                try
                {
                    var bandInstanceSize = ((GameObject)newBendInstance).GetComponent<MeshRenderer>().bounds.size.x;
                    bendInstanceRationProp.floatValue = _rope.Width / bandInstanceSize;
                }
                catch(Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        bool _needRefreshBackBandPoint;
        bool _needRefreshFrontBandPoint;

        private WrapPoint GetWrapPointByGameObject(GameObject gameObject)
        {
            Debug.Log("GetWrapPointByGameObject");
            var point = new WrapPoint();
            point.Origin = gameObject.transform.InverseTransformPoint(gameObject.transform.position);
            point.LocalShift = Vector3.zero;
            point.Parent = gameObject;
            point.PositionInWorldSpace = gameObject.transform.position;
            return point;
        }


        private void DoFrontEndChange(Object newFrontEnd)
        {
            _needRefreshFrontBandPoint = true;
        }


        private void DoBackEndChange(Object newBackEnd)
        {
            _needRefreshBackBandPoint = true;
        }


        private void DoPieceInstanceChange(BodyType body, bool restroyPieceInstances)
        {
            if (restroyPieceInstances)
                RestroyPieceInstances(body);
            SetPieceInstanceRatio(_rope.Width, _rope.ExtendAxis);
        }

        private T UpdateParameter<T>(string label, T value, Func<T, T> draw, bool light)
        {
            GUIStyle myStyle = new GUIStyle(GUI.skin.GetStyle("label"));
            if (light && value == null)
                myStyle.normal.textColor = new Color(1, 0, 0);
            var width = EditorGUIUtility.currentViewWidth;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, myStyle, GUILayout.Width(width * 0.35f)); //, GUILayout.Width(GUI.skin.label.CalcSize(new GUIContent(label)).x)
            //EditorGUILayout.LabelField(label, myStyle);
            value = draw(value);
            EditorGUILayout.EndHorizontal();
            return value; 
        }

        private void DoWidthChange(float width)
        {
            SetPieceInstanceRatio(width, _rope.ExtendAxis);
            var profile = GetProfile();
            if (profile == null)
                return;
            var workProfile = serializedObject.FindProperty("_workProfile");
            if (workProfile == null)
                return;
            for (var i = 0; i < profile.arraySize; i++)
            {
                if (workProfile.arraySize < i + 1)
                    workProfile.InsertArrayElementAtIndex(i);
                var workProp = workProfile.GetArrayElementAtIndex(i);
                var prop = profile.GetArrayElementAtIndex(i);
                workProp.vector3Value = prop.vector3Value * width;
            }

            var differ = workProfile.arraySize - profile.arraySize;
            if (differ > 0)
            {
                for (var i = 0; i < differ; i++)
                {
                    workProfile.DeleteArrayElementAtIndex(profile.arraySize);
                }
            }

        }


        private void DoMaterialChange(Material material)
        {
            var workMaterialProp = serializedObject.FindProperty("_workMaterial");
            var renderer = _rope.GetComponent<Renderer>();
            if (material != null)
            {
                var workMaterial = new Material(material);
                workMaterialProp.objectReferenceValue = workMaterial;
                if (renderer != null)
                {
                    renderer.sharedMaterial = workMaterial;
                }
            }
            else
            {
                workMaterialProp.objectReferenceValue = null;
                if (renderer != null)
                {
                    renderer.sharedMaterial = null;
                }
            }
        }


        private void DoBodyTypeChange(int bodyType)
        {
            if ((BodyType)bodyType == BodyType.Continuous)
            {
                InitProfile();
                // Возможно, потребуется обратная синхронизация с сериализуемыми полями _baseProfile, _prevVect, _rotation
                RestroyPieceInstances((BodyType)bodyType);
                _needRefreshPiecesProfilesAndSections = true;
            }
            else
            {
                var meshFilter = _rope.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    //Undo.RecordObject(meshFilter, "Mesh delete.");
                    meshFilter.sharedMesh.Clear();
                }
                RestroyPieceInstances((BodyType)bodyType);
                SetPieceInstanceRatio(_rope.Width, _rope.ExtendAxis);
            }
        }


        private void InitProfile()
        {
            var profile = GetProfile();
            var width = serializedObject.FindProperty("_width").floatValue;
            var prevVect = serializedObject.FindProperty("_prevVect");
            var rotation = serializedObject.FindProperty("_rotation");
            var triangulationPath1 = serializedObject.FindProperty("_triangulationPath1");
            var triangulationPath2 = serializedObject.FindProperty("_triangulationPath2");

            var initpoints = new List<Vector3>();

            // Корректируем профиль
            if (profile.arraySize < 3)
            {
                var poly = Geometry.CreatePolygon(3, Axis.Z, 0.5f, 0f);
                for(var i = 0; i < 3; i++)
                {
                    if (profile.arraySize < i + 1)
                        profile.InsertArrayElementAtIndex(i);
                    profile.GetArrayElementAtIndex(i).vector3Value = poly[i];
                }
            }
            // Делаем копию профиля 
            var initProfile = serializedObject.FindProperty("_initProfile");
            for (var i = 0; i < profile.arraySize; i++)
            {
                if (initProfile.arraySize < i + 1)
                    initProfile.InsertArrayElementAtIndex(i);
                var propSource = profile.GetArrayElementAtIndex(i);
                initProfile.GetArrayElementAtIndex(i).vector3Value = propSource.vector3Value;
                initpoints.Add(propSource.vector3Value);
            }

            // Масштабируем и копируем результат в workProfile            
            var workProfile = serializedObject.FindProperty("_workProfile");
            var baseProfile = serializedObject.FindProperty("_baseProfile");
            var workpoints = Geometry.ScalePoly(initpoints, width);
            for (var i = 0; i < initpoints.Count; i++)
            {
                if (workProfile.arraySize < i + 1)
                    workProfile.InsertArrayElementAtIndex(i);
                if (baseProfile.arraySize < i + 1)
                    baseProfile.InsertArrayElementAtIndex(i);
                workProfile.GetArrayElementAtIndex(i).vector3Value = workpoints[i];
                baseProfile.GetArrayElementAtIndex(i).vector3Value = workpoints[i];
            }
            prevVect.vector3Value = Vector3.forward;
            rotation.quaternionValue = new Quaternion(0, 0, 0, 1);

            var triangulator = new Triangulator();
            var profile2d = initpoints.Select(point => (Vector2)point).ToList();
            var path = triangulator.GetTriangulationIndexes(profile2d);
            for (var i = 0; i < path.Count; i++)
            {
                if (triangulationPath1.arraySize < i + 1)
                    triangulationPath1.InsertArrayElementAtIndex(i);
                if (triangulationPath2.arraySize < i + 1)
                    triangulationPath2.InsertArrayElementAtIndex(i);
                triangulationPath1.GetArrayElementAtIndex(i).vector3Value = path[i];
            }
            CutArray(triangulationPath1, path.Count);
            CutArray(triangulationPath2, path.Count);

            for (var i = triangulationPath1.arraySize - 1; i >= 0; i--)
            {
                var triangle = triangulationPath1.GetArrayElementAtIndex(triangulationPath1.arraySize - 1 - i).vector3Value;
                triangulationPath2.GetArrayElementAtIndex(i).vector3Value = new Vector3(triangle.x, triangle.z, triangle.y);
            }

            // Обрезаем все массивы
            CutArray(profile, initpoints.Count);
            CutArray(initProfile, initpoints.Count);
            CutArray(workProfile, initpoints.Count);
            CutArray(baseProfile, initpoints.Count);
        }

        private void CutArray(SerializedProperty array, int count)
        {
            var differ = array.arraySize - count;
            if (differ > 0)
            {
                for (var i = 0; i < differ; i++)
                {
                    array.DeleteArrayElementAtIndex(count);
                }
            }
        }

        private void DoExtendAxisChange(Axis extendAxis)
        {
            SetPieceInstanceRatio(_rope.Width, extendAxis);
        }

        private void SetPieceInstanceRatio(float width, Axis extendAxis)
        {
            GameObject _pieceInstance = (GameObject)serializedObject.FindProperty("_pieceInstance").objectReferenceValue;
            Vector3 pieceInstanceRatio = new Vector3(1, 1, 1);

            if (_pieceInstance != null)
            {
                var renderer = _pieceInstance.GetComponent<MeshRenderer>();
                //if (Body == BodyType.Сontinuous)
                //{
                //    PieceInstanceRatio = new Vector3(1, 1, 1);
                //    return;
                //}
                var pieceInstanceSize = renderer.bounds.size;
                serializedObject.FindProperty("_pieceInstanceSize").vector3Value = pieceInstanceSize;
                switch (extendAxis)
                {
                    case Axis.X:
                        pieceInstanceRatio = new Vector3(1 / pieceInstanceSize.x, width / pieceInstanceSize.y, width / pieceInstanceSize.z);
                        break;
                    case Axis.Y:
                        pieceInstanceRatio = new Vector3(width / pieceInstanceSize.x, 1 / pieceInstanceSize.y, width / pieceInstanceSize.z);
                        break;
                    case Axis.Z:
                        pieceInstanceRatio = new Vector3(width / pieceInstanceSize.x, width / pieceInstanceSize.y, 1 / pieceInstanceSize.z);
                        break;
                }
            }
            serializedObject.FindProperty("_pieceInstanceRatio").vector3Value = pieceInstanceRatio;
        }


        public GameObject GetPieceInstance(BodyType bodyType)
        {
            GameObject pieceInstance;
            GameObject sourcePieceInstance = (GameObject)serializedObject.FindProperty("_pieceInstance").objectReferenceValue;

            if (_rope.AllowProcedural() && bodyType == BodyType.Continuous)
                pieceInstance = new GameObject();
            else if (sourcePieceInstance != null)
                pieceInstance = Instantiate(sourcePieceInstance) as GameObject;
            else
            {
                //if (bodyType == BodyType.Сontinuous)
                //{
                //    if (MeshConfiguration.Profile.Count < 3)
                //        Debug.LogError("Count of points in Rope Profile could be greater 2. Please, increase nomber of points in Profile or set Body Type to FiniteSegments.");
                //}
                //else
                //{
                //    Debug.LogError("Piece Instance not specified. Please, specify Piece Instance or set Body Type to Continuos.");
                //}
                return null;
            }
            pieceInstance.AddComponent<Piece>();
            pieceInstance.transform.parent = _rope.transform;
            return pieceInstance;
        }


        private void RebuildPiecesProfilesAndSections()
        {
            var piece = _rope.FrontPiece;
            while (piece != null)
            {
                piece.InitMeshProfilesAndSections();
                piece = piece.BackPiece;
            }
        }

        private void RefreshBendPointInstance(BodyType body)
        {
            var piece = _rope.FrontPiece;
            while (piece != null)
            {
                piece.RefreshBendPointInstance(body);
                piece = piece.BackPiece;
            }
        }

        protected void RestroyPieceInstances(BodyType bodyType)
        {
            var piece = _rope.FrontPiece;
            Piece prevNewPiece = null;
            if (piece == null)
                return;
            while (piece != null)
            {
                var newObject = GetPieceInstance(bodyType);
                if (newObject == null)
                    return;
                //Undo.RegisterCreatedObjectUndo(newObject, "Refreshed piece create.");
                //piece.gameObject.SetActive(false);
                var newPiece = newObject.GetComponent<Piece>();
                newPiece.GetCopyOf(piece);
                newObject.name = piece.gameObject.name;
                piece.DontReorganizeWhenDestroy = true;
                // Предполагается вызов только при режиме редактирования. Уничтожаем игровой объект
                DestroyImmediate(piece.gameObject);
                if (prevNewPiece != null)
                {
                    prevNewPiece.BackPiece = newPiece;
                    newPiece.FrontPiece = prevNewPiece;
                }
                else
                {
                    //_rope.FrontPiece = newPiece;
                    serializedObject.FindProperty("_frontPiece").objectReferenceValue = newPiece;
                }
                prevNewPiece = newPiece;
                piece = piece.BackPiece;
                if (piece == null)
                    //_rope.BackPiece = newPiece;
                    serializedObject.FindProperty("_backPiece").objectReferenceValue = newPiece;

            }
            _needRefreshBendPointInstances = true;
            //piece = oldFrontPiece;
            //var go = new GameObject();
            ////Undo.RegisterCreatedObjectUndo(go, "Parent pieces.");
            //while (piece != null)
            //{
            //    piece.DontReorganizeWhenDestroy = true;
            //    //Undo.RegisterFullObjectHierarchyUndo(piece.gameObject, "Piece in rope.");
            //    piece.transform.parent = go.transform;
            //    piece = piece.BackPiece;
            //}
            //DestroyImmediate(go);
        }

        private void DoChangeWrapDistance(float wrapDistance)
        {
            RebuildBendPointsPosition(wrapDistance);
        }

        private void RebuildBendPointsPosition(float wrapDistance)
        {
            if (_rope.FrontPiece == null)
                return;
            var piece = _rope.FrontPiece;
            while (piece.BackPiece != null)
            {
                piece.BackBandPoint.SetPointInWorldSpace(wrapDistance);
                piece = piece.BackPiece;
            }
        }

        private void LayerMaskField()
        {
            var layers = InternalEditorUtility.layers;
            var layerNumbers = new List<int>();
            layerNumbers.Clear();

            for (int i = 0; i < layers.Length; i++)
                layerNumbers.Add(LayerMask.NameToLayer(layers[i]));

            int maskWithoutEmpty = 0;

            var oldValue = serializedObject.FindProperty("IgnoreLayer").intValue;

            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if (((1 << layerNumbers[i]) & oldValue) > 0)
                    maskWithoutEmpty |= (1 << i);
            }
            maskWithoutEmpty
                = UpdateParameter(
                    "Ignore Layer"
                    , maskWithoutEmpty
                    , (v) => { return EditorGUILayout.MaskField(v, layers); }
                    , false);

            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                    mask |= (1 << layerNumbers[i]);
            }
            serializedObject.FindProperty("IgnoreLayer").intValue = mask;
        }


        protected virtual void ShowMeshConfiguration()
        {
            var meshConfig = serializedObject.FindProperty("MeshConfiguration");
            if (meshConfig == null)
                return;
            EditorGUILayout.PropertyField(meshConfig);
            if (meshConfig.isExpanded)
            {
                EditorGUI.indentLevel += 1;

                EditorGUI.BeginDisabledGroup(_isPrefab);

                var oldBendCrosssectionsNmbProp = meshConfig.FindPropertyRelative("BendCrossectionsNumber");

                var oldBendCrosssectionsNmb = oldBendCrosssectionsNmbProp.intValue;
                var BendCrosssectionsNmb
                    = UpdateParameter(
                        "Bend Crossections Number"
                        , oldBendCrosssectionsNmb
                        , (v) => { return EditorGUILayout.IntSlider(v, 0, 8); }
                        , true);
                oldBendCrosssectionsNmbProp.intValue = BendCrosssectionsNmb;
                if (oldBendCrosssectionsNmb != BendCrosssectionsNmb)
                {
                    DoChangeCrosssectionsNmb(BendCrosssectionsNmb);
                    _canUndo = false;
                }


                EditorGUILayout.PropertyField(meshConfig.FindPropertyRelative("FlipNormals"));
                EditorGUI.EndDisabledGroup();

                var profile = meshConfig.FindPropertyRelative("Profile");
                if (profile == null)
                    return;
                ShowProfilePreview(profile);
                EditorGUI.indentLevel -= 1;
            }
        }

        private bool _needRefreshPiecesProfilesAndSections;

        private void DoChangeCrosssectionsNmb(int bendCrosssectionsNmb)
        {
            _needRefreshPiecesProfilesAndSections = true;
        }

        protected virtual SerializedProperty GetProfile()
        {
            var meshConfig = serializedObject.FindProperty("MeshConfiguration");
            if (meshConfig == null)
                return null;
            return meshConfig.FindPropertyRelative("Profile");
        }


        protected virtual void ShowProfilePreview(SerializedProperty profile)
        {
            if (!profile.isArray)
                return;
            EditorGUILayout.PropertyField(profile);
            if (profile.isExpanded)
            {
                EditorGUI.indentLevel += 1;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                var polygon = GetPolygonFromProfile(profile);
                DrawPreview(polygon);
                if (GUILayout.Button("Edit profile..."))
                {
                    PolygonEditor polygonWindow = EditorWindow.GetWindow<PolygonEditor>(false, "Polygon Editor", true);
                    polygonWindow.maxSize = new Vector2(1000, 1000);
                    polygonWindow.Show();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel -= 1;
            }
        }


        private List<Vector2> GetPolygonFromProfile(SerializedProperty profile)
        {
            var polygon = new List<Vector2>();
            for (var i = 0; i < profile.arraySize; i++)
            {
                var prop = profile.GetArrayElementAtIndex(i);
                polygon.Add(new Vector2(prop.vector3Value.x, prop.vector3Value.y));
            }
            return polygon;
        }


        protected virtual void DrawPreview(List<Vector2> polygon)
        {
            var style = GUI.skin.GetStyle("Button");
            var layoutRect = GUILayoutUtility.GetRect(100, 100);
            layoutRect.position = new Vector2(layoutRect.position.x + style.padding.left, layoutRect.position.y);
            layoutRect.width -= style.padding.left * 2;
            //layoutRect = EditorGUILayout.GetControlRect();

            Plotter.Render(_texture, polygon, 100f);
            Graphics.DrawTexture(new Rect(layoutRect.position, new Vector2(layoutRect.width, 100)), _texture, new Rect(((1000- layoutRect.width)) / 2 / 1000, 0, layoutRect.width / 1000, 1), 0, 0, 0, 0);
        }


        void OnSceneGUI()
        {
            //checkDragNDrop();
        }

        public void checkDragNDrop()
        {
            switch (Event.current.type)
            {

                case EventType.MouseDown:
                    // reset the DragAndDrop Data
                    //DragAndDrop.PrepareStartDrag();
                    Debug.Log("MouseDown");
                    break;

                case EventType.DragUpdated:

                    //DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    Debug.Log("DragUpdated " + Event.current.mousePosition);
                    break;

                case EventType.DragPerform:

                    //DragAndDrop.AcceptDrag();
                    Debug.Log("Drag accepted");
                    break;

                case EventType.MouseDrag:
                    //DragAndDrop.StartDrag("Dragging");

                    Debug.Log("MouseDrag: " + Event.current.mousePosition);

                    //Event.current.Use();
                    break;

                case EventType.MouseUp:
                    Debug.Log("MouseUp had ");

                    // Clean up, in case MouseDrag never occurred:
                    //DragAndDrop.PrepareStartDrag();

                    break;

                case EventType.DragExited:

                    Debug.Log("DragExited");
                    break;
            }
            //Event.current.Use();
        }

        private MethodInfo doFloatFieldMethod;
        private object recycledEditor;

        private void InitDoFloatFieldMethod()
        {
            Type editorGUIType = typeof(EditorGUI);

            Type RecycledTextEditorType = Assembly.GetAssembly(editorGUIType).GetType("UnityEditor.EditorGUI+RecycledTextEditor");
            Type[] argumentTypes = new Type[] { RecycledTextEditorType, typeof(Rect), typeof(Rect), typeof(int), typeof(float), typeof(string), typeof(GUIStyle), typeof(bool) };
            doFloatFieldMethod = editorGUIType.GetMethod("DoFloatField", BindingFlags.NonPublic | BindingFlags.Static, null, argumentTypes, null);
            FieldInfo fieldInfo = editorGUIType.GetField("s_RecycledEditor", BindingFlags.NonPublic | BindingFlags.Static);
            recycledEditor = fieldInfo.GetValue(null);
        }


        private float MyFloatFieldInternal(Rect position, Rect dragHotZone, float value, [DefaultValue("EditorStyles.numberField")]GUIStyle style)
        {
            if (doFloatFieldMethod == null || recycledEditor == null)
                InitDoFloatFieldMethod();
            float fieldValue = value;
            if (doFloatFieldMethod != null && recycledEditor != null)
            {
                int controlID = GUIUtility.GetControlID("EditorTextField".GetHashCode(), FocusType.Keyboard, position);
                object[] parameters = new object[] { recycledEditor, position, dragHotZone, controlID, value, "g7", style, true };
                fieldValue = (float)doFloatFieldMethod.Invoke(null, parameters);
            }
            return fieldValue;
        }
    }





  
    public class RopeStub : MonoBehaviour
    {

    }
}
