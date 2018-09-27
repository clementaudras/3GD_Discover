using System.Collections.Generic;
using UnityEngine;

namespace WrappingRopeLibrary.Editors
{
    public abstract class Symbol
    {
        public Vector2 Position { get; set; }

        public Color Color { get; set; }
        public abstract List<Stroke> GetStrokeList(Color color, float size);

        public List<Stroke> GetStrokeList(float size)
        {
            return GetStrokeList(Color, size);
        }

        public Symbol()
        {
        }

        public Symbol(Vector2 position)
        {
            Position = position;
        }

    }
    public enum SymbolType
    {
        Dot = 0,
        Cross = 1
    }
}
