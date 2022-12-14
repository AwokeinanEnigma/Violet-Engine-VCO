using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using Violet.Collision;
using Violet.Utility;

namespace VCO.Overworld
{
    internal class TriggerArea : ICollidable
    {
        public Vector2f Position
        {
            get => this.position;
            set => this.position = value;
        }

        public Vector2f Velocity => VectorMath.ZERO_VECTOR;

        public AABB AABB => this.mesh.AABB;

        public Mesh Mesh => this.mesh;

        public bool Solid
        {
            get => this.solid;
            set => this.solid = value;
        }

        public int Flag => this.flag;

        public string Script => this.script;

        public VertexArray DebugVerts { get; private set; }

        public TriggerArea(Vector2f position, List<Vector2f> points, int flag, string script)
        {
            this.position = position;
            this.mesh = new Mesh(points);
            this.flag = flag;
            this.script = script;
            this.solid = true;
            VertexArray vertexArray = new VertexArray(PrimitiveType.LineStrip, (uint)(this.mesh.Vertices.Count + 1));
            for (int i = 0; i < this.mesh.Vertices.Count; i++)
            {
                vertexArray[(uint)i] = new Vertex(this.mesh.Vertices[i], Color.Magenta);
            }
            vertexArray[(uint)this.mesh.Vertices.Count] = new Vertex(this.mesh.Vertices[0], Color.Magenta);
            this.DebugVerts = vertexArray;
        }

        public void Collision(CollisionContext context)
        {
        }

        private Vector2f position;

        private readonly Mesh mesh;

        private bool solid;

        private readonly int flag;

        private readonly string script;
    }
}
