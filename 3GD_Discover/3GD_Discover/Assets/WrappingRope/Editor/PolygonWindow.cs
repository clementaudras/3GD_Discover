using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using WrappingRopeLibrary.Events;

namespace WrappingRopeLibrary.Editors
{
    public class PolygonEditor : EditorWindow, IPolygonView
    {

        //public static PolygonEditor polygonWindow;
        private List<Symbol> _symbols;
        private GUIContent _makeRegularButtonContent;
        private GUIContent _insertPointButtonOffContent;
        private GUIContent _insertPointButtonOnContent;
        private GUIContent _deletePointOnButtonContent;
        private GUIContent _deletePointOffButtonContent;


        private bool _isInsertPointOn;
        private bool _isDeletePointOn;
        private RenderTexture _texture;
        private Rect _currentLayout;
        private float _zoom;
        private PolygonController _controller;

        private RenderTexture texture
        {
            get
            {
                if (_texture == null)
                {
                    _texture = new RenderTexture(1000, 1000, 16, RenderTextureFormat.ARGB32);
                }
                return _texture;
            }
        }

        public List<Vector2> Polygon { get; set; }

        public List<Symbol> Symbols { get { return _symbols; } }


        public float Height
        {
            get
            {
                return position.height;
            }
        }


        public float Width
        {
            get
            {
                return position.width;
            }
        }

        public float Zoom { get { return _zoom; } }

        public event Mouse MouseDown;
        public event Mouse MouseMove;
        public event Mouse MouseUp;
        public event Action<bool> InsertPointChanged;
        public event Action<bool> DeletePointChanged;
        public event Action MakeRegular;


        [MenuItem("Window/Polygon Editor %e")]
        public static void Init()
        {
            PolygonEditor polygonWindow = GetWindow<PolygonEditor>(false, "Polygon Editor", true);
            polygonWindow.maxSize = new Vector2(1000, 1000);
            polygonWindow.Show();
            polygonWindow.Populate();
        }


        void OnFocus() { Populate(); }
        void OnSelectionChange()
        {
            Populate();
            Repaint();
        }
        void OnEnable()
        {
            Initialize();
            Populate();
        }


        void Initialize()
        {
            _makeRegularButtonContent = new GUIContent(Resources.Load<Texture2D>("Make_Regular"), "Make Regular");
            _insertPointButtonOffContent = new GUIContent(Resources.Load<Texture2D>("Add_Point_Off"), "Insert points");
            _insertPointButtonOnContent = new GUIContent(Resources.Load<Texture2D>("Add_Point_On"), "Insert points");
            _deletePointOnButtonContent = new GUIContent(Resources.Load<Texture2D>("Delete_Point_On"), "Delete points");
            _deletePointOffButtonContent = new GUIContent(Resources.Load<Texture2D>("Delete_Point_Off"), "Delete points");

            _zoom = 100;
            _isInsertPointOn = false;
            _isDeletePointOn = false;
            _symbols = new List<Symbol>();
            wantsMouseMove = true;
        }


        void OnGUI()
        {
            string message;
            if (!_controller.Populate(out message))
            {
                GUILayout.TextArea(message);
                return;
            }
            EditorGUILayout.BeginHorizontal();
            var buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.margin = new RectOffset(2, 2, 2, 2);
            buttonStyle.padding = new RectOffset(4, 4, 4, 4);

            if (GUILayout.Button(_makeRegularButtonContent, buttonStyle, GUILayout.ExpandWidth(false)))
            {
                OnMakeRegular();
            }


            var insertPointContent = _isInsertPointOn ? _insertPointButtonOnContent : _insertPointButtonOffContent;
            var deletePointContent = _isDeletePointOn ? _deletePointOnButtonContent : _deletePointOffButtonContent;

            if (GUILayout.Toggle(_isInsertPointOn, insertPointContent, buttonStyle, GUILayout.ExpandWidth(false)))
            {
                if (!_isInsertPointOn)
                {
                    _isInsertPointOn = true;
                    _isDeletePointOn = false;
                    OnInsertPointChanged(true);
                }
            }
            else
            {
                if (_isInsertPointOn)
                {
                    _isInsertPointOn = false;
                    OnInsertPointChanged(false);
                }
            }

            if (GUILayout.Toggle(_isDeletePointOn, deletePointContent, buttonStyle, GUILayout.ExpandWidth(false)))
            {
                if (!_isDeletePointOn)
                {
                    _isDeletePointOn = true;
                    _isInsertPointOn = false;
                    OnDeletePointChanged(true);
                }
            }
            else
            {
                if (_isDeletePointOn)
                {
                    _isDeletePointOn = false;
                    OnDeletePointChanged(false);
                }
            }

            EditorGUILayout.EndHorizontal();
            _currentLayout = GUILayoutUtility.GetRect(1000, 1000);
            var evnt = Event.current;
            var spacePosition = MousePositionToSpace(evnt.mousePosition);
            if (evnt.type == EventType.MouseDown)
            {
                OnMouseDown(spacePosition);
            }
            if (evnt.type == EventType.MouseUp)
            {

                OnMouseUp(spacePosition);
            }

            if (evnt.type == EventType.MouseDrag)
            {
                OnMouseMove(spacePosition);
            }
            if (evnt.type == EventType.MouseMove)
            {
                OnMouseMove(spacePosition);
            }

            Vector2 delta = Vector2.zero;
            if (evnt.type == EventType.ScrollWheel)
            {
                delta = evnt.delta;
                _zoom -= delta.y * 1f;
                if (_zoom < 1)
                    _zoom = 1;
            }
            VisualizeAll();
            _controller.AcceptChanges();
        }

        private void VisualizeAll()
        {
            var strokeList = new List<Stroke>();
            // Чертим "поля"
            strokeList.Add(new Stroke { Start = new Vector2(-0.5f, 0.5f), End = new Vector2(0.5f, 0.5f), Color = new Color(0.2f, 0.2f, 0.2f, 1) });
            strokeList.Add(new Stroke { Start = new Vector2(-0.5f, -0.5f), End = new Vector2(0.5f, -0.5f), Color = new Color(0.2f, 0.2f, 0.2f, 1) });
            strokeList.Add(new Stroke { Start = new Vector2(-0.5f, 0.5f), End = new Vector2(-0.5f, -0.5f), Color = new Color(0.2f, 0.2f, 0.2f, 1) });
            strokeList.Add(new Stroke { Start = new Vector2(0.5f, 0.5f), End = new Vector2(0.5f, -0.5f), Color = new Color(0.2f, 0.2f, 0.2f, 1) });
            // Чертим сам полигон
            strokeList.AddRange(PolygonToStrokeList(Polygon, new Color(100, 100, 0, 100)));
            // Чертим точки полигона
            GetSymbolsForPolygonPoints().ForEach(symbol => strokeList.AddRange(symbol.GetStrokeList(GetColorForSymbol(symbol), 10f / _zoom)));
            // Чертим вспомогательные символы
            _symbols.ForEach(symbol => strokeList.AddRange(symbol.GetStrokeList(10f / _zoom)));
            Draw(strokeList);
        }

        private List<Symbol> GetSymbolsForPolygonPoints()
        {
            var list = new List<Symbol>();
            Polygon.ForEach(point => list.Add(new Dot(point)));
            return list;

        }

        private Color GetColorForSymbol(Symbol symbol)
        {
            switch (symbol.GetType().Name)
            {
                case "Dot": return new Color(0, 1, 0, 1);
                case "Cross": return new Color(1, 0, 0, 1);
                default: return new Color(0, 1, 0, 1);
            }
        }

        void Populate()
        {
            string message;
            if (_controller == null)
                _controller = new PolygonController(this);
            _controller.Populate(out message);
        }

        private void Draw(List<Stroke> strokeList)
        {
            var xPos = (texture.width - position.width) / texture.width / 2;
            var yPos = (texture.height - position.height + _currentLayout.y) / texture.height / 2;
            var chamber = new Rect(xPos, yPos, position.width / texture.width, (position.height - _currentLayout.y) / texture.height);
            Plotter.Render(texture, strokeList, _zoom);
            Graphics.DrawTexture(new Rect(_currentLayout.position, new Vector2(position.width, position.height - _currentLayout.y)), texture, chamber, 0, 0, 0, 0);
            Repaint();
        }

        private List<Stroke> PolygonToStrokeList(List<Vector2> polygon, Color color)
        {
            var strokeList = new List<Stroke>();
            for (var i = 0; i < polygon.Count; i++)
            {
                var nextIndex = i == polygon.Count - 1 ? 0 : i + 1;
                strokeList.Add(new Stroke { Color = color, Start = polygon[i], End = polygon[nextIndex] });
            }
            return strokeList;

        }

        void OnMouseDown(Vector2 position)
        {
            if (MouseDown != null)
            {
                MouseDown(position);
            }
        }

        void OnMouseUp(Vector2 position)
        {
            if (MouseUp != null)
            {
                MouseUp(position);
            }
        }

        void OnMouseMove(Vector2 position)
        {
            if (MouseMove != null)
            {
                MouseMove(position);
            }
        }


        void OnMakeRegular()
        {
            if (MakeRegular != null)
            {
                MakeRegular();
            }
        }


        void OnInsertPointChanged(bool isOn)
        {
            if (InsertPointChanged != null)
            {
                InsertPointChanged(isOn);
            }
        }


        void OnDeletePointChanged(bool isOn)
        {
            if (DeletePointChanged != null)
            {
                DeletePointChanged(isOn);
            }
        }

        Vector2 MousePositionToSpace(Vector2 pos)
        {
            var viewPortHeight = position.height - _currentLayout.y;
            return new Vector2(((position.width) / -2 + pos.x) / _zoom, ((viewPortHeight) / 2 - pos.y + _currentLayout.y) / _zoom);

        }

        //private void Reset()
        //{
        //    _rope = null;
        //    _serializedObject = null;
        //    _profile = null;
        //    _polygon = null;
        //}
    }
}

