using System;
using System.Collections.Generic;
using UnityEngine;
using WrappingRopeLibrary.Events;

namespace WrappingRopeLibrary.Editors
{
    public interface IPolygonView
    {
        event Mouse MouseDown;
        event Mouse MouseUp;
        event Mouse MouseMove;
        event Action MakeRegular;
        event Action<bool> InsertPointChanged;
        event Action<bool> DeletePointChanged;

        float Width { get; }
        float Height { get; }

        float Zoom { get; }
        List<Vector2> Polygon { get; set; }

        List<Symbol> Symbols { get; }
    }
}
