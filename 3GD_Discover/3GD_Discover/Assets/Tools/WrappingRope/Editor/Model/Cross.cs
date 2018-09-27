using System.Collections.Generic;
using UnityEngine;

namespace WrappingRopeLibrary.Editors
{
    public class Cross : Symbol
    {
        public Cross() : base()
        {
        }

        public Cross(Vector2 position) : base(position)
        {
        }

        public override List<Stroke> GetStrokeList(Color color, float size)
        {
            size = size / 2;
            var list = new List<Stroke>();
            list.Add(new Stroke() { Start = new Vector2(size, size) + Position, End = new Vector2(-size, -size) + Position, Color = color });
            list.Add(new Stroke() { Start = new Vector2(-size, size) + Position, End = new Vector2(size, -size) + Position, Color = color });
            return list;

        }
    }
}
