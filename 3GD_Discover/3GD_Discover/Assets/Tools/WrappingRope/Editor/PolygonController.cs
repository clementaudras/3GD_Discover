using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WrappingRopeLibrary.Enums;
using WrappingRopeLibrary.Scripts;
using WrappingRopeLibrary.Utils;

namespace WrappingRopeLibrary.Editors
{
    public class PolygonController
    {
        private IPolygonView _view;
        private List<Vector2> _polygon;
        private SerializedProperty _profile;
        private SerializedObject _serializedObject;
        private List<Vector2> _edges = new List<Vector2>();

        private bool _mouseDown;
        private int _currentPointIndex = -1;
        private Mode _editMode;
        private readonly Symbol _cross;
        private readonly Symbol _dot;

        public float Zoom { get { return _view.Zoom; } }

        public PolygonController(IPolygonView view)
        {
            _cross = new Cross() { Position = Vector2.zero, Color = new Color(1, 0, 0, 1) };
            _dot = new Dot() { Position = Vector2.zero, Color = new Color(1, 0, 0, 1)};
            _view = view;
            _view.MouseDown += _view_MouseDown;
            _view.MouseMove += _view_MouseMove;
            _view.MouseUp += _view_MouseUp;
            _view.MakeRegular += _view_MakeRegular;
            _view.InsertPointChanged += _view_InsertPointChanged;
            _view.DeletePointChanged += _view_DeletePointChanged;
        }

        private void _view_DeletePointChanged(bool state)
        {
            if (state)
                _editMode = Mode.DeletePoint;
            else
                _editMode = Mode.Free;
        }

        private void _view_InsertPointChanged(bool state)
        {
            if (state)
                _editMode = Mode.InsertPoint;
            else
                _editMode = Mode.Free;
        }


        private void _view_MakeRegular()
        {
            var regularPoly = Geometry.CreatePolygon(_polygon.Count, Axis.Z, 0.5f, 0);
            for(var i = 0; i < _polygon.Count; i++)
            {
                _polygon[i] = regularPoly[i];
            }
            BuildEdges();
        }


        private void BuildEdges()
        {
            _edges.Clear();
            for (var i=0; i<_polygon.Count; i++)
            {
                var nextI = i == _polygon.Count - 1 ? 0 : i + 1;
                _edges.Add(GetLineParameters(_polygon[i], _polygon[nextI]));
            }
        }


        private Vector2 GetLineParameters(Vector2 p1, Vector2 p2)
        {
            var a = (p1.y - p2.y);
            var b = (p1.x - p2.x);
            return new Vector2(a / b, p1.y - a / b * p1.x);
        }


        private void _view_MouseUp(Vector2 position)
        {
            _mouseDown = false;
            _currentPointIndex = -1;
        }

        private void _view_MouseMove(Vector2 position)
        {
            if (_mouseDown && _currentPointIndex > -1)
            {
                TryMovePoint(position);
                return;
            }
            if (_editMode == Mode.InsertPoint)
            {
                SetSymbolForInsertPosition(position);
                return;
            }
            if (_editMode == Mode.DeletePoint)
            {
                SetSymbolForDeletePosition(position);
            }
        }

        private void SetSymbolForDeletePosition(Vector2 position)
        {
            int pointIndex;
            if (CatchPoint(position, out pointIndex))
            {
                _dot.Position = _polygon[pointIndex];
                if (!_view.Symbols.Contains(_dot))
                {
                    _view.Symbols.Add(_dot);
                }
            }
            else
            {
                _view.Symbols.Remove(_dot);
            }
        }

        private void SetSymbolForInsertPosition(Vector2 position)
        {
            int edgeIndex;
            Vector2 pointOnEdge;
            if (TryFindNearestEdgeIndex(position, ZoomedTreshHold, out edgeIndex, out pointOnEdge))
            {
                _cross.Position = pointOnEdge;
                if (!_view.Symbols.Contains(_cross))
                {
                    _view.Symbols.Add(_cross);
                }
            }
            else
            {
                _view.Symbols.Remove(_cross);
            }
        }

        private void TryMovePoint(Vector2 position)
        {
            var prevPosition = _polygon[_currentPointIndex];
            _polygon[_currentPointIndex] = position;
            RecalcNaibourParameters(_currentPointIndex);
            float limitX;
            float limitY;
            if (!CanMovePoint(_currentPointIndex, out limitX, out limitY))
            {
                _polygon[_currentPointIndex] = prevPosition;
                //_polygon[_currentPointIndex] = new Vector2(limitX, limitY);
                RecalcNaibourParameters(_currentPointIndex);
            }
            else if (IsPolygonReverted())
            {
                Debug.Log("Reverted");
                _polygon.Reverse();
                BuildEdges();
                _currentPointIndex = _polygon.Count - _currentPointIndex - 1;
            }
        }


        private void RecalcNaibourParameters(int index)
        {
            int prevIndex, nextIndex;
            GetNaiboureIndexes(index, out prevIndex, out nextIndex);
            _edges[prevIndex] = GetLineParameters(_polygon[prevIndex], _polygon[index]);
            _edges[index] = GetLineParameters(_polygon[index], _polygon[nextIndex]);
        }

        private void GetNaiboureIndexes(int index, out int prevIndex, out int nextIndex)
        {
            if (index == 0)
            {
                prevIndex = _polygon.Count - 1;
                nextIndex = 1;
            }
            else if (index == _polygon.Count - 1)
            {
                prevIndex = index - 1;
                nextIndex = 0;
            }
            else
            {
                prevIndex = index - 1;
                nextIndex = index + 1;
            }
        }

        private void _view_MouseDown(Vector2 position)
        {
            _mouseDown = true;
            if (_editMode == Mode.Free)
            {
                int pointIndex;
                if (CatchPoint(position, out pointIndex))
                    _currentPointIndex = pointIndex;
                return;
            }
            if (_editMode == Mode.InsertPoint)
            {
                TryAddPoint(position);
                return;
            }
            if (_editMode == Mode.DeletePoint)
            {
                int pointIndex;
                if (TryDeletePoint(position))
                {
                    if (CatchPoint(position, out pointIndex))
                        _dot.Position = _polygon[pointIndex];
                    else
                        _view.Symbols.Remove(_dot);
                }
                return;
            }
        }


        private void TryAddPoint(Vector2 point)
        {
            int edgeIndex;
            Vector2 pointOnEdge;
            if (TryFindNearestEdgeIndex(point, ZoomedTreshHold, out edgeIndex, out pointOnEdge))
            {
                _polygon.Insert(edgeIndex + 1, pointOnEdge);
                var edgeLineParams = _edges[edgeIndex];
                _edges.Insert(edgeIndex + 1, edgeLineParams);
            }
        }


        private bool TryDeletePoint(Vector2 position)
        {
            if (_polygon.Count < 4)
                return false;
            int pointIndex;
            if (!CatchPoint(position, out pointIndex))
                return false;
            _polygon.RemoveAt(pointIndex);
            _edges.RemoveAt(pointIndex);
            var prevIndex = pointIndex == 0 ? _polygon.Count - 1 : pointIndex - 1;
            if (pointIndex == _polygon.Count)
                pointIndex = 0;
            var edgeLineParams = GetLineParameters(_polygon[prevIndex], _polygon[pointIndex]);
            _edges[prevIndex] = edgeLineParams;
            return true;
        }

        const float _TRESHHOLD = 5f; // В пикселях!!!
        private float ZoomedTreshHold
        {
            get { return _TRESHHOLD / Zoom; }
        }


        private float GetSignOfPointsTriangle(int index)
        {
            float result = 0;
            int prevIndex;
            int nextIndex;
            GetNaiboureIndexes(index, out prevIndex, out nextIndex);
            result = new Plane(_polygon[prevIndex], _polygon[index], _polygon[nextIndex]).normal.z;
            return result;

        }


        private bool IsPointsProximatlyInTriplet(int index)
        {
            var result = false;
            int prevIndex;
            int nextIndex;
            GetNaiboureIndexes(index, out prevIndex, out nextIndex);
            result = _polygon[index] == _polygon[prevIndex] || _polygon[index] == _polygon[nextIndex];
            return result;
        }

        private bool IsPolygonReverted()
        {
            float minX = _polygon[0].x;
            float maxX = _polygon[0].x;
            float minY = _polygon[0].y;
            float maxY = _polygon[0].y;
            var boundPoints = new PointInfo[4]
            {
                new PointInfo { Index = 0, PositionOnAxis = _polygon[0].x }
                , new PointInfo { Index = 0, PositionOnAxis = _polygon[0].x }
                , new PointInfo { Index = 0, PositionOnAxis = _polygon[0].y }
                , new PointInfo { Index = 0, PositionOnAxis = _polygon[0].y }
            };
            int index = 0;
            foreach (var point in _polygon)
            {
                if (!IsPointsProximatlyInTriplet(index))
                {

                    if (point.x < minX)
                    {
                        minX = point.x;
                        boundPoints[0].Index = index;
                        boundPoints[0].PositionOnAxis = point.x;
                    }
                    if (point.x > maxX)
                    {
                        maxX = point.x;
                        boundPoints[1].Index = index;
                        boundPoints[1].PositionOnAxis = point.x;
                    }
                    if (point.y < minY)
                    {
                        minY = point.y;
                        boundPoints[2].Index = index;
                        boundPoints[2].PositionOnAxis = point.y;
                    }
                    if (point.y > maxY)
                    {
                        maxY = point.y;
                        boundPoints[3].Index = index;
                        boundPoints[3].PositionOnAxis = point.y;
                    }
                }
                index++;
            }
            foreach (var info in boundPoints)
            {
                if (GetSignOfPointsTriangle(info.Index) == 1)
                    return true;
            }
            return false;
        }


        private bool CatchPoint(Vector2 position, out int pointIndex)
        {
            pointIndex = -1;
            var zoomedTreshHold = ZoomedTreshHold;
            var sqrTreshHold = zoomedTreshHold * zoomedTreshHold;
            for (var i = 0; i < _polygon.Count; i++)
            {
                var point = _polygon[i];

                if ((point - position).sqrMagnitude < sqrTreshHold)
                {
                    pointIndex = i;
                    return true;
                }
            }
            return false;
        }

        private bool CanMovePoint(int index, out float limitX, out float limitY)
        {
            limitX = 0;
            limitY = 0;
            if (CheckEdgeIntersect(index, out limitX, out limitY))
            {
                return false;
            }
            if (CheckEdgeIntersect(index == 0 ? _polygon.Count - 1 : index - 1, out limitX, out limitY))
            {
                return false;
            }
            return true;
        }


        private bool CheckEdgeIntersect(int index, out float x, out float y)
        {
            x = 0;
            y = 0;
            int i = index + 2;
            for (var count = 0; count < _edges.Count - 3; count++)
            {
                if (i > _polygon.Count - 1)
                    i = i - _polygon.Count;
                var intersect = IsIntersectEdges(index, i, out x, out y);
                if (intersect)
                {
                    return true;
                }
                i++;
            }
            return false;
        }


        private bool IsIntersectEdges(int index1, int index2, out float x, out float y)
        {
            x = 0;
            y = 0;
            var secondIndex1 = index1 == _polygon.Count - 1 ? 0 : index1 + 1;
            var secondIndex2 = index2 == _polygon.Count - 1 ? 0 : index2 + 1;
            var p11 = _polygon[index1];
            var p12 = _polygon[secondIndex1];
            var p21 = _polygon[index2];
            var p22 = _polygon[secondIndex2];
            var lineParams1 = _edges[index1];
            var lineParams2 = _edges[index2];
            return IsIntersect(p11, p12, p21, p22, lineParams1, lineParams2, out x, out y);
        }

        private bool IsIntersect(Vector2 p11, Vector2 p12, Vector2 p21, Vector2 p22, Vector2 lineParams1, Vector2 lineParams2, out float x, out float y)
        {
            x = 0;
            y = 0;
            float res1;
            float res2;
            float res3;
            float res4;

            if (Mathf.Approximately(lineParams1.x, lineParams2.x)) return false;
            if (float.IsInfinity(lineParams1.x))
            {
                x = p11.x;
                y = x * lineParams2.x + lineParams2.y;
            }
            else if (float.IsInfinity(lineParams2.x))
            {
                x = p21.x;
                y = x * lineParams1.x + lineParams1.y;
            }
            else
            {
                x = -(lineParams1.y - lineParams2.y) / (lineParams1.x - lineParams2.x);
                y = x * lineParams1.x + lineParams1.y;
            }
            if (!Mathf.Approximately(p11.x, p12.x))
            {
                // Определяем для первого отрезка по "иксу"
                res1 = x - p11.x;
                res2 = x - p12.x;
            }
            else
            {
                // Определяем для первого отрезка по "игрику"
                res1 = y - p11.y;
                res2 = y - p12.y;
            }
            if (!Mathf.Approximately(p21.x, p22.x))
            {
                // Определяем для второго отрезка по "иксу"
                res3 = x - p21.x;
                res4 = x - p22.x;
            }
            else
            {
                // Определяем для второго отрезка по "игрику"
                res3 = y - p21.y;
                res4 = y - p22.y;
            }
            return (res1 != 0 && res2 != 0 && Mathf.Sign(res1) != Mathf.Sign(res2) && res3 != 0 && res4 != 0 && Mathf.Sign(res3) != Mathf.Sign(res4));
        }

        private bool TryFindNearestEdgeIndex(Vector2 position, float treshHold, out int edgeIndex, out Vector2 poinOnEdge)
        {
            edgeIndex = 0;
            poinOnEdge = position;
            var selections = new List<EdgeBrokeInfo>();
            for(var i = 0; i < _polygon.Count; i++)
            {
                var sqrMagnitude = treshHold;
                poinOnEdge = position;
                if (IsPointNearEdge(position, i, treshHold, out sqrMagnitude, out poinOnEdge))
                {
                    selections.Add(new EdgeBrokeInfo { EdgeIndex = i, sqrDistanceFromPosition = sqrMagnitude, PointOnEdge = poinOnEdge });
                }
            }
            if (selections.Count == 0)
                return false;
            var minDistance = treshHold * treshHold;
            foreach(var edgeBrokeInfo in selections)
            {
                if (edgeBrokeInfo.sqrDistanceFromPosition < minDistance)
                {
                    edgeIndex = edgeBrokeInfo.EdgeIndex;
                    minDistance = edgeBrokeInfo.sqrDistanceFromPosition;
                    poinOnEdge = edgeBrokeInfo.PointOnEdge;
                }

            }
            return true;
        }

        private bool IsPointNearEdge(Vector2 point, int edgeIndex, float treshHold, out float sqrMistance, out Vector2 poinOnEdge)
        {
            sqrMistance = treshHold;
            var secondIndex = edgeIndex == _polygon.Count - 1 ? 0 : edgeIndex + 1;
            var normal = _polygon[edgeIndex] - _polygon[secondIndex];
            var origin = _polygon[edgeIndex] - point;
            var projection = Vector3.Project(origin, normal);
            poinOnEdge = _polygon[edgeIndex] - (Vector2)projection;

            // Определяем отрезок, перпендикулярный ребру, выходящий из заданной точки
            var perpendic = (origin - (Vector2)projection);
            // Определяем его длину
            sqrMistance = perpendic.sqrMagnitude;
            var p11 = point;
            var p12 = point + perpendic.normalized * treshHold;
            var p21 = _polygon[edgeIndex];
            var p22 = _polygon[secondIndex];
            var lineParams1 = GetLineParameters(p11, p12);
            var lineParams2 = _edges[edgeIndex];
            float x, y;

            return IsIntersect(p11, p12, p21, p22, lineParams1, lineParams2, out x, out y);
        }

        public class EdgeBrokeInfo
        {
            public int EdgeIndex { get; set; }
            public float sqrDistanceFromPosition { get; set; }
            public Vector2 PointOnEdge { get; set; }
        }


        public class PointInfo
        {
            public int Index { get; set; }
            public float PositionOnAxis { get; set; }
            public float SignOfTriangle { get; set; }
        }


        private void CopyPolygonToProfile()
        {
            for (var i = 0; i < _polygon.Count; i++)
            {
                if (_profile.arraySize < i + 1)
                    _profile.InsertArrayElementAtIndex(i);
                var prop = _profile.GetArrayElementAtIndex(i);
                prop.vector3Value = _polygon[i];
            }
            var differ = _profile.arraySize - _polygon.Count;
            if (differ > 0)
            {
                for (var i = 0; i < differ; i++)
                {
                    _profile.DeleteArrayElementAtIndex(_polygon.Count);
                }
            }
        }

        protected void SetPolygon()
        {
            if (_serializedObject == null)
                return;
            var meshConfig = _serializedObject.FindProperty("MeshConfiguration");
            if (meshConfig == null)
                return;
            var profile = meshConfig.FindPropertyRelative("Profile");
            if (profile == null)
                return;
            _profile = profile;
            _polygon = GetPolygonFromProfile(profile);

            if (_view == null)
            {
                Debug.Log("The view of controller is null. The polygon of view could not be obtained.");
                return;
            }
            if (_polygon.Count < 3)
            {
                _polygon.Clear();
                var regularPoly = Geometry.CreatePolygon(3, Axis.Z, 0.5f, 0);
                regularPoly.ForEach(point => _polygon.Add(point));
            }
            BuildEdges();
            _view.Polygon = _polygon;
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

        private RopeBase _rope;

        public bool Populate(out string reason)
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(RopeBase), SelectionMode.Assets);
            reason = string.Empty;
            if (selection.Length > 0)
            {
                if (selection[0] != null)
                {
                    var rope = (RopeBase)selection[0];
                    if (PrefabUtility.GetPrefabType(rope) == PrefabType.Prefab)
                    {
                        reason = "Prefab profile could not be changed.";
                        return false;
                    }
                    if (rope != _rope)
                        SetupRope(rope);
                    else
                        _serializedObject.Update();
                    return true;
                }
            }
            Reset();
            reason = "Please, select a game object with the Rope component for edit profile in this window.";
            return false;
        }


        private void Reset()
        {
            _rope = null;
            _serializedObject = null;
            _profile = null;
            _polygon = null;
        }

        public void SetupRope(RopeBase rope)
        {
            _serializedObject = new SerializedObject(rope);
            _rope = rope;
            if (_view != null)
                _view.Symbols.Clear();
            SetPolygon();
        }


        public void AcceptChanges()
        {
            CopyPolygonToProfile();
            InitProfile();
            _serializedObject.ApplyModifiedProperties();
            RebuildPiecesProfilesAndSections();
        }


        private void InitProfile()
        {
            var width = _serializedObject.FindProperty("_width").floatValue;
            var prevVect = _serializedObject.FindProperty("_prevVect");
            var rotation = _serializedObject.FindProperty("_rotation");
            var triangulationPath1 = _serializedObject.FindProperty("_triangulationPath1");
            var triangulationPath2 = _serializedObject.FindProperty("_triangulationPath2");

            var initpoints = new List<Vector3>();

            // Делаем копию профиля 
            var initProfile = _serializedObject.FindProperty("_initProfile");
            for (var i = 0; i < _profile.arraySize; i++)
            {
                if (initProfile.arraySize < i + 1)
                    initProfile.InsertArrayElementAtIndex(i);
                var propSource = _profile.GetArrayElementAtIndex(i);
                initProfile.GetArrayElementAtIndex(i).vector3Value = propSource.vector3Value;
                initpoints.Add(propSource.vector3Value);
            }

            // Масштабируем и копируем результат в workProfile            
            var workProfile = _serializedObject.FindProperty("_workProfile");
            var baseProfile = _serializedObject.FindProperty("_baseProfile");
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


        public enum Mode
        {
            Free = 0,
            InsertPoint = 1,
            DeletePoint = 2
        }
    }
}
