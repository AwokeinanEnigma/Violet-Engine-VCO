using SFML.Graphics;
using SFML.System;
using System;
using VCO.Actors.Animation;
using VCO.Data;
using VCO.Overworld;
using Violet.Collision;
using Violet.Graphics;
using Violet.Utility;

namespace VCO.Actors
{
    internal class PartyFollower : IDisposable, ICollidable
    {
        // (get) Token: 0x06000388 RID: 904 RVA: 0x00016AEC File Offset: 0x00014CEC
        // (set) Token: 0x06000389 RID: 905 RVA: 0x00016AF4 File Offset: 0x00014CF4
        public int Place
        {
            get => this.place;
            set => this.place = value;
        }
        // (get) Token: 0x0600038A RID: 906 RVA: 0x00016AFD File Offset: 0x00014CFD
        public float Width => this.followerGraphic.Size.X;
        // (get) Token: 0x0600038B RID: 907 RVA: 0x00016B0F File Offset: 0x00014D0F
        public CharacterType Character => this.character;
        // (get) Token: 0x0600038C RID: 908 RVA: 0x00016B17 File Offset: 0x00014D17
        public int Direction => this.direction;
        // (get) Token: 0x0600038D RID: 909 RVA: 0x00016B1F File Offset: 0x00014D1F
        // (set) Token: 0x0600038E RID: 910 RVA: 0x00016B27 File Offset: 0x00014D27
        public Vector2f Position
        {
            get => this.position;
            set
            {
            }
        }
        // (get) Token: 0x0600038F RID: 911 RVA: 0x00016B29 File Offset: 0x00014D29
        public Vector2f Velocity => this.velocity;
        // (get) Token: 0x06000390 RID: 912 RVA: 0x00016B31 File Offset: 0x00014D31
        public AABB AABB => this.aabb;
        // (get) Token: 0x06000391 RID: 913 RVA: 0x00016B39 File Offset: 0x00014D39
        public Mesh Mesh => this.mesh;
        // (get) Token: 0x06000392 RID: 914 RVA: 0x00016B41 File Offset: 0x00014D41
        // (set) Token: 0x06000393 RID: 915 RVA: 0x00016B49 File Offset: 0x00014D49
        public bool Solid
        {
            get => this.solid;
            set => this.solid = value;
        }
        // (get) Token: 0x06000394 RID: 916 RVA: 0x00016B52 File Offset: 0x00014D52
        public VertexArray DebugVerts => this.GetDebugVerts();
        public PartyFollower(RenderPipeline pipeline, CollisionManager colman, PartyTrain recorder, CharacterType character, Vector2f position, int direction, bool useShadow)
        {
            this.pipeline = pipeline;
            this.recorder = recorder;
            this.character = character;
            this.place = 0;
            this.isDead = (CharacterStats.GetStats(this.character).HP <= 0);
            this.useShadow = (useShadow && !this.isDead);
            this.position = position;
            this.velocity = VectorMath.ZERO_VECTOR;
            this.direction = direction;
            string file = CharacterGraphics.GetFile(character);
            this.followerGraphic = new IndexedColorGraphic(file, "walk south", this.position, (int)this.position.Y - 1)
            {
                SpeedModifier = 0f,
                Frame = 0f
            };
            this.pipeline.Add(this.followerGraphic);
            if (this.useShadow)
            {
                this.shadowGraphic = new IndexedColorGraphic(DataHandler.instance.Load("shadow.dat"), ShadowSize.GetSubsprite(this.followerGraphic.Size), this.Position, (int)this.position.Y - 2);
                this.pipeline.Add(this.shadowGraphic);
            }
            this.animator = new AnimationControl(this.followerGraphic, this.direction);
            this.animator.UpdateSubsprite(this.GetAnimationContext());
            int width = this.followerGraphic.TextureRect.Width;
            int height = this.followerGraphic.TextureRect.Height;
            this.mesh = new Mesh(new FloatRect(-(width / 2), -3f, width, 6f));
            this.aabb = this.mesh.AABB;
            this.solid = true;
            this.collisionManager = colman;
        }
        ~PartyFollower()
        {
            this.Dispose(false);
        }
        private VertexArray GetDebugVerts()
        {
            if (this.debugVerts == null)
            {
                Color color = new Color(61, 129, 166);
                VertexArray vertexArray = new VertexArray(PrimitiveType.LineStrip, (uint)(this.mesh.Vertices.Count + 1));
                for (int i = 0; i < this.mesh.Vertices.Count; i++)
                {
                    vertexArray[(uint)i] = new Vertex(this.mesh.Vertices[i], color);
                }
                vertexArray[(uint)this.mesh.Vertices.Count] = new Vertex(this.mesh.Vertices[0], color);
                this.debugVerts = vertexArray;
            }
            return this.debugVerts;
        }
        private void RecorderReset(Vector2f position, int direction)
        {
            this.position = position;
            this.direction = direction;
        }
        private AnimationContext GetAnimationContext()
        {
            return new AnimationContext
            {
                Velocity = this.velocity * (this.isRunning ? 2f : 1f) * (this.moving ? 1f : 0f),
                SuggestedDirection = this.direction,
                TerrainType = this.terrain,
                IsDead = this.isDead,
                IsCrouch = this.isCrouch,
                IsNauseous = false,
                IsTalk = false
            };
        }
        public void Update(Vector2f newPosition, Vector2f newVelocity, TerrainType newTerrain)
        {
            this.lastPosition = this.position;
            this.position = newPosition;
            this.velocity = newVelocity;
            this.terrain = newTerrain;
            this.lastRunning = this.isRunning;
            this.isRunning = this.recorder.Running;
            this.isCrouch = this.recorder.Crouching;
            this.direction = VectorMath.VectorToDirection(this.velocity);
            this.lastMoving = this.moving;
            if ((int)this.lastPosition.X != (int)this.position.X || (int)this.lastPosition.Y != (int)this.position.Y)
            {
                this.followerGraphic.Position = new Vector2f((int)this.position.X, (int)this.position.Y);
                this.followerGraphic.Depth = (int)this.Position.Y;
                this.pipeline.Update(this.followerGraphic);
                if (this.useShadow)
                {
                    this.shadowGraphic.Position = this.followerGraphic.Position;
                    this.shadowGraphic.Depth = (int)this.Position.Y - 1;
                    this.pipeline.Update(this.shadowGraphic);
                }
                this.collisionManager.Update(this, this.lastPosition, this.position);
                this.moving = true;
            }
            else
            {
                this.moving = false;
            }
            this.animator.UpdateSubsprite(this.GetAnimationContext());
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.pipeline.Remove(this.followerGraphic);
                    this.pipeline.Remove(this.shadowGraphic);
                    this.followerGraphic.Dispose();
                    this.shadowGraphic.Dispose();
                }
                this.disposed = true;
            }
        }
        public void Collision(CollisionContext context)
        {
        }
        private bool disposed;
        private readonly RenderPipeline pipeline;
        private readonly PartyTrain recorder;
        private readonly CharacterType character;
        private int place;
        private int direction;
        private Vector2f velocity;
        private Vector2f position;
        private Vector2f lastPosition;
        private TerrainType terrain;
        private readonly IndexedColorGraphic followerGraphic;
        private readonly Graphic shadowGraphic;
        private bool isRunning;
        private bool lastRunning;
        private bool moving;
        private bool lastMoving;
        private readonly bool useShadow;
        private readonly bool isDead;
        private bool isCrouch;
        private AABB aabb;
        private readonly Mesh mesh;
        private bool solid;
        private VertexArray debugVerts;
        private readonly CollisionManager collisionManager;
        private readonly AnimationControl animator;
    }
}
