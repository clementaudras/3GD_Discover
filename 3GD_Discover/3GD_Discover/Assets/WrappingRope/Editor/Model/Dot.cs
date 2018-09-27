using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WrappingRopeLibrary.Enums;
using WrappingRopeLibrary.Utils;

namespace WrappingRopeLibrary.Editors
{
    public class Dot : Symbol
    {
        public Dot() : base()
        {
        }

        public Dot(Vector2 position) : base(position)
        {
        }

        public override List<Stroke> GetStrokeList(Color color, float size)
        {
            size = size / 2;

            var list = new List<Stroke>();
            var polygon = Geometry.CreatePolygon(6, Axis.Z, size, 0).Select(point3d => (Vector2)point3d).ToList();
            for(var i = 0; i < polygon.Count; i++)
            {
                var nextIndex = i == polygon.Count - 1 ? 0 : i + 1;
                list.Add( new Stroke() { Start = polygon[i] + Position, End = polygon[nextIndex] + Position, Color = color });
            }
            return list;
        }
    }
}
