using SFML.Graphics;
using SFML.System;
using Violet.Collision;
using Violet.Utility;

namespace VCO.Overworld
{
    internal class Portal : ICollidable
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

        public string Map => this.map;

        public Vector2f PositionTo => this.positionTo;

        public int DirectionTo => this.directionTo;

        public VertexArray DebugVerts { get; private set; }

        public Portal(int x, int y, int width, int height, int xTo, int yTo, int dirTo, string map)
        {
            this.position = new Vector2f(x, y);
            this.positionTo = new Vector2f(xTo, yTo);
            this.directionTo = dirTo;
            this.mesh = new Mesh(new FloatRect(VectorMath.ZERO_VECTOR, new Vector2f(width, height)));
            this.map = map;
            this.solid = true;
            VertexArray vertexArray = new VertexArray(PrimitiveType.LineStrip, (uint)(this.mesh.Vertices.Count + 1));
            for (int i = 0; i < this.mesh.Vertices.Count; i++)
            {
                vertexArray[(uint)i] = new Vertex(this.mesh.Vertices[i], Color.Blue);
            }
            vertexArray[(uint)this.mesh.Vertices.Count] = new Vertex(this.mesh.Vertices[0], Color.Blue);
            this.DebugVerts = vertexArray;
        }

        public void Collision(CollisionContext context)
        {
        }

        private Vector2f position;

        private Vector2f positionTo;

        private readonly int directionTo;

        private readonly Mesh mesh;

        private bool solid;

        private readonly string map;
    }
}
