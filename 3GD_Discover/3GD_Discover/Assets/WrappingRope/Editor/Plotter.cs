using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WrappingRopeLibrary.Enums;
using WrappingRopeLibrary.Utils;

namespace WrappingRopeLibrary.Editors
{
    public class Plotter
    {
        private static Material LineMaterial = CreateMaterial();
        private static List<Vector2> pointTemplate = CreatePointTemplate();
        private static Material CreateMaterial()
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            var material = new Material(shader);
            material.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            material.SetInt("_ZWrite", 0);
            return material;
        }

        private static List<Vector2> CreatePointTemplate()
        {
            var template = new List<Vector2>();
            template.AddRange(Geometry.CreatePolygon(16, Axis.Z, 0.02f, 0).Select(point => new Vector2(point.x, point.y)));
            return template;
        }

        public static void Render(RenderTexture texture, List<Vector2> polygon, float aspect, bool drawPoints = false)
        {
            if (texture == null)
                return;
            var oldTarget = RenderTexture.active;
            LineMaterial.SetPass(0);
            Graphics.SetRenderTarget(texture);
            GL.PushMatrix();

            GL.LoadPixelMatrix(-0.5f * texture.width / aspect, 0.5f * texture.width / aspect, -0.5f * texture.height / aspect, 0.5f * texture.height / aspect);
            GL.Clear(true, true, new Color(0, 0, 0, 1));
            for (var i = 0; i < polygon.Count; i++)
            {
                var point = polygon[i];
                DrawPoint(point);
                GL.Begin(GL.LINES);
                GL.Color(new Color(1, 1, 0, 1));
                GL.Vertex3(point.x, point.y, 0);
                var nextI = i + 1 == polygon.Count ? 0 : i + 1;
                point = polygon[nextI];
                GL.Vertex3(point.x, point.y, 0);
                GL.End();
            }
            GL.PopMatrix();
            Graphics.SetRenderTarget(oldTarget);
        }


        public static void Render(RenderTexture texture, List<Stroke> strokeList, float aspect)
        {
            if (texture == null)
                return;
            var oldTarget = RenderTexture.active;
            LineMaterial.SetPass(0);
            Graphics.SetRenderTarget(texture);
            GL.PushMatrix();

            GL.LoadPixelMatrix(-0.5f * texture.width / aspect, 0.5f * texture.width / aspect, -0.5f * texture.height / aspect, 0.5f * texture.height / aspect);
            GL.Clear(true, true, new Color(0, 0, 0, 1));
            foreach(var stroke in strokeList)
            {
                GL.Begin(GL.LINES);
                GL.Color(stroke.Color);
                GL.Vertex3(stroke.Start.x, stroke.Start.y, 0);
                GL.Vertex3(stroke.End.x, stroke.End.y, 0);
                GL.End();
            }
            GL.PopMatrix();
            Graphics.SetRenderTarget(oldTarget);
        }

        private static void DrawPoint(Vector2 position)
        {
            GL.Begin(GL.LINES);
            GL.Color(new Color(0, 1, 0, 1));

            for (var i = 0; i < pointTemplate.Count; i++)
            {
                var point = pointTemplate[i];
                GL.Vertex3(point.x + position.x, point.y + position.y, 0);
                var nextI = i + 1 == pointTemplate.Count ? 0 : i + 1;
                point = pointTemplate[nextI];
                GL.Vertex3(point.x + position.x, point.y + position.y, 0);
            }
            GL.End();
        }
    }
}
