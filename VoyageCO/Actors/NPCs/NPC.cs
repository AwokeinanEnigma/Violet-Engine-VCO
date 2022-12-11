using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using VCO.Actors.Animation;
using VCO.Actors.NPCs.Movement;
using VCO.Data;
using VCO.Overworld;
using Violet;
using Violet.Actors;
using Violet.Collision;
using Violet.Graphics;
using Violet.Maps;
using Violet.Utility;

namespace VCO.Actors.NPCs
{
    internal class NPC : SolidActor
    {
        private readonly RenderPipeline pipeline;
        private IndexedColorGraphic npcGraphic;
        private Graphic shadowGraphic;
        private IndexedColorGraphic effectGraphic;
        private readonly string name;
        private int direction;
        private bool talkPause;
        private NPC.State state;
        private NPC.State lastState;
        private NPC.State stateMemory;
        private List<Map.NPCtext> text;
        private List<Map.NPCtext> teleText;
        private Vector2f lastVelocity;
        private Vector2f startPosition;
        private Vector2f graphicOffset;
        private float lastZOffset;
        private readonly float speed;
        private readonly int delay;
        private readonly int distance;
        private float hopFactor;
        private long hopFrame;
        private Mover mover;
        private bool changed;
        private readonly bool shadow;
        private readonly bool sticky;
        private int depth;
        private bool depthOverride;
        private bool hasSprite;
        private bool forceSpriteUpdate;
        private AnimationControl animator;
        private int animationLoopCount;
        private int animationLoopCountTarget;

        public List<Map.NPCtext> Text => this.text;
        public List<Map.NPCtext> TeleText => this.teleText;

        public string Name => this.name;

        public int Direction
        {
            get => this.direction;
            set
            {
                this.direction = value;
                this.forceSpriteUpdate = true;
            }
        }

        public float HopFactor
        {
            get => this.hopFactor;
            set
            {
                this.hopFactor = value;
                this.hopFrame = Engine.Frame;
            }
        }
        public int Depth => this.depth;
        public Vector2f EmoticonPoint => new Vector2f(this.position.X, this.position.Y - this.npcGraphic.Origin.Y);
        public NPC(RenderPipeline pipeline, CollisionManager colman, Map.NPC npcData, object moverData) : base(null)
        {
            this.pipeline = pipeline;
            this.name = npcData.Name;
            this.direction = npcData.Direction;
            this.text = npcData.Text;
            this.teleText = npcData.TeleText;
            this.shadow = npcData.Shadow;
            this.sticky = npcData.Sticky;
            NPC.MoveMode mode = (NPC.MoveMode)npcData.Mode;
            this.speed = npcData.Speed;
            this.delay = npcData.Delay;
            this.distance = npcData.Distance;
            this.startPosition.X = npcData.X;
            this.startPosition.Y = npcData.Y;
            this.SetMoveMode(mode, moverData);
            this.position = this.startPosition;
            this.depthOverride = (npcData.DepthOverride > int.MinValue);
            this.depth = (this.depthOverride ? npcData.DepthOverride : ((int)this.position.Y));
            this.pipeline = pipeline;
            if (npcData.Sprite != null && npcData.Sprite.Length > 0)
            {
                this.hasSprite = true;
                //Debug.Log("loading npc thing");
                this.ChangeSprite(DataHandler.instance.Load(npcData.Sprite + ".dat"), "stand south");
                if (this.shadow)
                {
                    this.shadowGraphic = new IndexedColorGraphic(DataHandler.instance.Load("shadow.dat"), ShadowSize.GetSubsprite(this.npcGraphic.Size), this.position, this.depth - 1);
                    this.pipeline.Add(this.shadowGraphic);
                }
                int width = this.npcGraphic.TextureRect.Width;
                int height = this.npcGraphic.TextureRect.Height;
                this.mesh = new Mesh(new FloatRect(-(width / 2), -3f, width, 6f));
            }
            else
            {
                this.mesh = new Mesh(new FloatRect(0f, 0f, npcData.Width, npcData.Height));
            }
            this.aabb = this.mesh.AABB;
            this.isSolid = npcData.Solid;
            this.collisionManager = colman;
            this.collisionManager.Add(this);
            this.lastVelocity = VectorMath.ZERO_VECTOR;
            this.state = NPC.State.Idle;
            this.ChangeState(this.state);
        }
        public void SetMoveMode(NPC.MoveMode moveMode, object moverData)
        {
            NPC.MoveMode moveMode2 = moveMode;
            if (moverData is Map.Path)
            {
                moveMode2 = NPC.MoveMode.Path;
            }
            if (moverData is Map.Area)
            {
                moveMode2 = NPC.MoveMode.Area;
            }
            switch (moveMode2)
            {
                case NPC.MoveMode.RandomTurn:
                    this.mover = new RandomTurnMover(60);
                    return;
                case NPC.MoveMode.FacePlayer:
                    this.mover = new FacePlayerMover();
                    return;
                case NPC.MoveMode.Random:
                    this.mover = new RandomMover(this.speed, distance, this.delay);
                    return;
                case NPC.MoveMode.Path:
                    {
                        Map.Path path = (Map.Path)moverData;
                        bool loop = moveMode > NPC.MoveMode.None;
                        this.mover = new PathMover(this.speed, this.delay, loop, path.Points);
                        this.startPosition.X = (int)path.Points[0].X;
                        this.startPosition.Y = (int)path.Points[0].Y;
                        return;
                    }
                case NPC.MoveMode.Area:
                    {
                        Map.Area area = (Map.Area)moverData;
                        this.mover = new AreaMover(this.speed, this.delay, distance, area.Rectangle.Left, area.Rectangle.Top, area.Rectangle.Width, area.Rectangle.Height);
                        //         isSolid = false;
                        return;
                    }
                default:
                    this.mover = new NoneMover();
                    return;
            }
        }
        public void SetMover(Mover mover)
        {
            this.mover = mover;
        }
        public void Telepathize()
        {
            this.effectGraphic = new IndexedColorGraphic(DataHandler.instance.Load("telepathy.dat"), "pulse", VectorMath.Truncate(this.position - new Vector2f(0f, this.npcGraphic.Origin.Y - this.npcGraphic.Size.Y / 4f)), 2147450881);
            this.pipeline.Add(this.effectGraphic);
        }
        public void Untelepathize()
        {
            this.pipeline.Remove(this.effectGraphic);
            this.effectGraphic.Dispose();
            this.effectGraphic = null;
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
                IsTalk = (this.state == NPC.State.Talking)
            };
        }
        public void ChangeSprite(string resource, string subsprite)
        {
            if (this.npcGraphic != null)
            {
                this.pipeline.Remove(this.npcGraphic);
            }
            this.npcGraphic = new IndexedColorGraphic(resource, subsprite, this.position, this.depth);
            this.pipeline.Add(this.npcGraphic);
            if (this.animator == null)
            {
                this.animator = new AnimationControl(this.npcGraphic, this.direction);
            }
            this.animator.ChangeGraphic(this.npcGraphic);
            this.animator.UpdateSubsprite(this.GetAnimationContext());
            this.hasSprite = true;
        }
        public void OverrideSubsprite(string subsprite)
        {
            if (this.hasSprite)
            {
                this.animator.OverrideSubsprite(subsprite);
            }
        }
        public void ClearOverrideSubsprite()
        {
            if (this.hasSprite)
            {
                this.animator.ClearOverride();
                this.animator.UpdateSubsprite(this.GetAnimationContext());
                if (this.animationLoopCountTarget > 0)
                {
                    this.animationLoopCount = 0;
                    this.animationLoopCountTarget = 0;
                    this.npcGraphic.OnAnimationComplete -= this.npcGraphic_OnAnimationComplete;
                }
            }
        }
        public void SetAnimationLoopCount(int loopCount)
        {
            if (this.hasSprite && this.animator.Overriden)
            {
                this.animationLoopCount = 0;
                this.animationLoopCountTarget = Math.Max(1, loopCount);
                this.npcGraphic.OnAnimationComplete += this.npcGraphic_OnAnimationComplete;
            }
        }
        private void npcGraphic_OnAnimationComplete(AnimatedRenderable renderable)
        {
            this.animationLoopCount++;
            if (this.animationLoopCount >= this.animationLoopCountTarget)
            {
                this.npcGraphic.SpeedModifier = 0f;
                this.npcGraphic.OnAnimationComplete -= this.npcGraphic_OnAnimationComplete;
            }
        }
        private void ChangeState(NPC.State newState)
        {
            if (newState != this.state)
            {
                this.lastState = this.state;
                this.state = newState;
            }
        }
        public void StartTalking()
        {
            if (!this.talkPause)
            {
                this.velocity.X = 0f;
                this.velocity.Y = 0f;
                this.stateMemory = this.state;
            }
            this.ChangeState(NPC.State.Talking);
            this.talkPause = false;
        }
        public void PauseTalking()
        {
            if (this.state == NPC.State.Talking)
            {
                this.ChangeState(NPC.State.Idle);
                this.talkPause = true;
            }
        }
        public void StopTalking()
        {
            this.ChangeState(this.stateMemory);
            this.isMovementLocked = false;
            this.talkPause = false;
            this.animator.ClearOverride();
        }
        public void StartMoving()
        {
            this.ChangeState(NPC.State.Moving);
        }
        public void ForceDepth(int newDepth)
        {
            this.depthOverride = true;
            this.depth = newDepth;
            this.forceSpriteUpdate = true;
        }
        public void ResetDepth()
        {
            this.depthOverride = false;
        }
        public override void Update()
        {
            this.lastVelocity = this.velocity;
            if (this.state != NPC.State.Talking)
            {
                if (!this.MovementLocked)
                {
                    this.changed = this.mover.GetNextMove(ref this.position, ref this.velocity, ref this.direction);
                }
                base.Update();
                if (this.hopFactor >= 1f)
                {
                    this.lastZOffset = this.zOffset;
                    this.zOffset = (float)Math.Sin((Engine.Frame - this.hopFrame) / (this.hopFactor * 0.3f)) * this.hopFactor;
                    if (this.zOffset < 0f)
                    {
                        this.zOffset = 0f;
                        this.hopFactor = 0f;
                    }
                }
                if ((int)this.lastPosition.X != (int)this.position.X || (int)this.lastPosition.Y != (int)this.position.Y || (int)this.lastZOffset != (int)this.zOffset || this.forceSpriteUpdate)
                {
                    if (this.state != NPC.State.Moving)
                    {
                        this.ChangeState(NPC.State.Moving);
                    }
                    if (!this.depthOverride)
                    {
                        this.depth = (int)this.position.Y;
                    }
                    if (this.hasSprite)
                    {
                        this.graphicOffset.Y = -this.zOffset;
                        this.npcGraphic.Position = VectorMath.Truncate(this.position + this.graphicOffset);
                        this.npcGraphic.Depth = this.depth;
                        this.pipeline.Update(this.npcGraphic);
                        if (this.shadow)
                        {
                            this.shadowGraphic.Position = VectorMath.Truncate(this.position);
                            this.shadowGraphic.Depth = this.depth - 1;
                            this.pipeline.Update(this.shadowGraphic);
                        }
                    }
                    this.forceSpriteUpdate = false;
                }
                else if (this.state != NPC.State.Idle)
                {
                    this.ChangeState(NPC.State.Idle);
                }
            }
            if (this.hasSprite)
            {
                this.animator.UpdateSubsprite(this.GetAnimationContext());
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.pipeline.Remove(this.npcGraphic);
                    this.pipeline.Remove(this.shadowGraphic);

                    this.npcGraphic.Dispose();
                    npcGraphic = null;

                    // clear text, because it gets stuck in memory otherwise
                    text.Clear();
                    text = null;

                    teleText.Clear();
                    teleText = null;

                    if (this.shadow)
                    {
                        this.shadowGraphic.Dispose();
                        this.shadowGraphic = null;
                    }

                }
                this.disposed = true;
                base.Dispose(disposing);
            }
        }

        public enum State
        {
            Idle,
            Talking,
            Moving
        }
        public enum MoveMode
        {
            None,
            RandomTurn,
            FacePlayer,
            Random,
            Path,
            Area,
            Teleporter
        }
    }
}
