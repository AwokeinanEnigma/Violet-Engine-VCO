using SFML.Graphics;
using SFML.System;
using System;
using VCO.Actors.Animation;
using VCO.Actors.NPCs.Movement;
using VCO.Data;
using VCO.Data.Enemies;
using VCO.Overworld;
using Violet.Actors;
using Violet.Collision;
using Violet.Graphics;
using Violet.Utility;

namespace VCO.Actors.NPCs
{
    internal class EnemyNPC : SolidActor
    {
        #region Properties
        public EnemyData Type => this.enemyType;
        public Graphic Graphic => this.npcGraphic;
        #endregion

        //private static readonly Vector2f HALO_OFFSET = new Vector2f(0f, -32f);
        private readonly RenderPipeline pipeline;
        private readonly IndexedColorGraphic npcGraphic;
        //private readonly IndexedColorGraphic haloGraphic;
        private readonly Graphic shadowGraphic;
        private Mover mover;
        private readonly bool[] hasDirection;
        private Vector2f lastVelocity;
        private int direction;
        private bool changed;
        private readonly EnemyData enemyType;
        private readonly AnimationControl animator;

        public EnemyNPC(RenderPipeline pipeline, CollisionManager colman, EnemyData enemyType, Vector2f position, FloatRect spawnArea) : base(colman)
        {
            //Console.WriteLine("enemy");

            this.pipeline = pipeline;
            this.position = position;
            this.enemyType = enemyType;
            this.mover = new LookForTroubleMover(100, 2.5f);//LookForTroubleMover(100, 2);//new ZigZagMover(colman, this, new FloatRect(new Vector2f(10, 10), new Vector2f(10, 10)), 10, 2,
                                                            //    100); //new MushroomMover(this, 100f, 2f);
            this.npcGraphic = new IndexedColorGraphic(Paths.GRAPHICS + enemyType.OverworldSprite + ".dat"/*"mushroom.dat"*/, "walk south", this.Position, (int)this.Position.Y);
            this.pipeline.Add(this.npcGraphic);
            this.hasDirection = new bool[8];
            this.hasDirection[0] = (this.npcGraphic.GetSpriteDefinition("walk east") != null);
            this.hasDirection[1] = (this.npcGraphic.GetSpriteDefinition("walk northeast") != null);
            this.hasDirection[2] = (this.npcGraphic.GetSpriteDefinition("walk north") != null);
            this.hasDirection[3] = (this.npcGraphic.GetSpriteDefinition("walk northwest") != null);
            this.hasDirection[4] = (this.npcGraphic.GetSpriteDefinition("walk west") != null);
            this.hasDirection[5] = (this.npcGraphic.GetSpriteDefinition("walk southwest") != null);
            this.hasDirection[6] = (this.npcGraphic.GetSpriteDefinition("walk south") != null);
            this.hasDirection[7] = (this.npcGraphic.GetSpriteDefinition("walk southeast") != null);
            this.shadowGraphic = new IndexedColorGraphic(Paths.GRAPHICS + "shadow.dat", ShadowSize.GetSubsprite(this.npcGraphic.Size), this.Position, (int)(this.Position.Y - 1f));
            this.pipeline.Add(this.shadowGraphic);
            int width = this.npcGraphic.TextureRect.Width;
            int height = this.npcGraphic.TextureRect.Height;
            this.mesh = new Mesh(new FloatRect(-(width / 2), -3f, width, 6f));
            this.aabb = this.mesh.AABB;
            this.animator = new AnimationControl(this.npcGraphic, this.direction);
            this.animator.UpdateSubsprite(this.GetAnimationContext());
            results = new ICollidable[1];
        }
        private AnimationContext GetAnimationContext()
        {
            return new AnimationContext
            {
                Velocity = this.velocity,
                SuggestedDirection = this.direction,
                TerrainType = TerrainType.None,
                IsDead = false,
                IsCrouch = false,
                IsNauseous = false,
                IsTalk = false
            };
        }

        public void OverrideSubsprite(string subsprite)
        {
            this.animator.OverrideSubsprite(subsprite);
        }

        public void ClearOverrideSubsprite()
        {
            this.animator.ClearOverride();
        }

        public void FreezeSpriteForever()
        {
            this.npcGraphic.SpeedModifier = 0f;
        }

        public bool hasEnteredBattle = false;
        protected override void HandleCollision(ICollidable[] collisionObjects)
        {
            base.HandleCollision(collisionObjects);
    
        }

        public delegate void OnDestroyed(EnemyNPC npc);
        public event OnDestroyed onDestroy;

        public void Destroy()
        {
            onDestroy?.Invoke(this);
            Dispose(true);
        }

        private static readonly Type[] enemyofType =
            new Type[]
            {
                typeof(EnemyNPC)
            };


        public override void Update()
        {
            this.lastVelocity = this.velocity;
            if (!this.isMovementLocked)
            {
                this.changed = this.mover.GetNextMove(ref this.position, ref this.velocity, ref this.direction);
            }
            if (this.changed)
            {
                //Console.WriteLine("active");
                this.animator.UpdateSubsprite(this.GetAnimationContext());
                this.npcGraphic.Position = VectorMath.Truncate(this.position);
                this.npcGraphic.Depth = (int)this.position.Y;
                this.pipeline.Update(this.npcGraphic);
                this.shadowGraphic.Position = VectorMath.Truncate(this.position);
                this.shadowGraphic.Depth = (int)this.position.Y - 1;
                this.pipeline.Update(this.shadowGraphic);
                Vector2f v = new Vector2f(this.velocity.X, 0f);
                Vector2f v2 = new Vector2f(0f, this.velocity.Y);
                this.lastPosition = this.position;
                if (this.collisionManager.PlaceFree(this, this.position + v, results, enemyofType))
                {
                    this.position += v;
                }
                else
                {
                    HandleCollision(results);
                }
                if (this.collisionManager.PlaceFree(this, this.position + v2, results, enemyofType))
                {
                    this.position += v2;
                }
                else
                {
                    HandleCollision(results);
                }

                this.collisionManager.Update(this, this.lastPosition, this.position);
                this.changed = false;
            }
        }

        public ICollidable[] results;
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                try
                {
                    if (disposing)
                    {
                        this.pipeline.Remove(this.npcGraphic);
                        this.pipeline.Remove(this.shadowGraphic);
                        this.npcGraphic.Dispose();
                        this.shadowGraphic.Dispose();
                    }
                    this.disposed = true;
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }
    }
}
