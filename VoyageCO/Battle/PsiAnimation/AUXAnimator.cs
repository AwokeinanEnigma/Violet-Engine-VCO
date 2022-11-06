using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using VCO.Battle.UI;
using VCO.GUI.Modifiers;
using Violet.Graphics;

namespace VCO.Battle.AUXAnimation
{
    internal class AUXAnimator
    {
        public bool Complete => this.complete;

        public event AUXAnimator.AnimationCompleteHandler OnAnimationComplete;

        public AUXAnimator(RenderPipeline pipeline, List<IGraphicModifier> graphicModifiers, AUXElementList animation, Graphic senderGraphic, Graphic[] targetGraphics, CardBar cardBar, int[] targetCardIds)
        {
            this.pipeline = pipeline;
            this.graphicModifiers = graphicModifiers;
            this.animation = animation;
            this.senderGraphic = senderGraphic;
            this.targetGraphics = targetGraphics;
            this.cardBar = cardBar;
            this.targetCardIds = targetCardIds;
            this.screenShape = new RectangleShape(new Vector2f(320f, 180f))
            {
                FillColor = new Color(0, 0, 0, 0)
            };
        }

        private void DarkenScreen(Color darkenColor, int depth)
        {
            if (this.screenDarkenShape != null)
            {
                this.pipeline.Remove(this.screenDarkenShape);
            }
            this.targetAlpha = darkenColor.A;
            this.sourceAlpha = this.screenShape.FillColor.A;
            this.alphaMultiplier = 0f;
            FloatRect localBounds = this.screenShape.GetLocalBounds();
            this.screenShape.FillColor = new Color(darkenColor.R, darkenColor.G, darkenColor.B, this.sourceAlpha);
            this.screenDarkenShape = new ShapeGraphic(this.screenShape, new Vector2f(0f, 0f), new Vector2f(0f, 0f), new Vector2f(localBounds.Width, localBounds.Height), depth);
            this.pipeline.Add(this.screenDarkenShape);
            if (this.sourceAlpha == 0)
            {
                if (this.depthMemory == null)
                {
                    this.depthMemory = new Dictionary<Graphic, int>();
                }
                else
                {
                    this.depthMemory.Clear();
                }
                for (int i = 0; i < this.targetGraphics.Length; i++)
                {
                    if (this.targetCardIds[i] < 0)
                    {
                        Graphic graphic = this.targetGraphics[i];
                        this.depthMemory.Add(graphic, graphic.Depth);
                        graphic.Depth = 32677;
                    }
                }
            }
            this.darkenedFlag = false;
        }

        private void UpdateDarkenColor()
        {
            Color fillColor = this.screenDarkenShape.Shape.FillColor;
            this.alphaMultiplier += 0.2f;
            fillColor.A = (byte)(sourceAlpha + (this.targetAlpha - this.sourceAlpha) * this.alphaMultiplier);
            this.screenDarkenShape.Shape.FillColor = fillColor;
        }

        public void Update()
        {
            List<AUXElement> elementsAtTime = this.animation.GetElementsAtTime(this.step);
            if (elementsAtTime != null && elementsAtTime.Count > 0)
            {
                foreach (AUXElement AUXElement in elementsAtTime)
                {
                    if (AUXElement.Animation != null)
                    {
                        this.pipeline.Add(AUXElement.Animation);
                        AUXElement.Animation.OnAnimationComplete += this.GraphicAnimationComplete;
                        this.animatingCount++;
                        if (AUXElement.LockToTargetPosition)
                        {
                            AUXElement.Animation.Position = this.targetGraphics[AUXElement.PositionIndex].Position;
                            AUXElement.Animation.Position += AUXElement.Offset;
                        }
                    }
                    if (AUXElement.Sound != null)
                    {
                        AUXElement.Sound.Play();
                    }
                    Color? screenDarkenColor = AUXElement.ScreenDarkenColor;
                    if (screenDarkenColor != null)
                    {
                        Color? screenDarkenColor2 = AUXElement.ScreenDarkenColor;
                        this.DarkenScreen(screenDarkenColor2.Value, AUXElement.ScreenDarkenDepth ?? 32667);
                        this.animatingCount++;
                    }
                    Color? targetFlashColor = AUXElement.TargetFlashColor;
                    if (targetFlashColor != null)
                    {
                        foreach (Graphic graphic in this.targetGraphics)
                        {
                            if (graphic is IndexedColorGraphic)
                            {
                                IndexedColorGraphic graphic2 = graphic as IndexedColorGraphic;
                                Color? targetFlashColor2 = AUXElement.TargetFlashColor;
                                GraphicFader item = new GraphicFader(graphic2, targetFlashColor2.Value, AUXElement.TargetFlashBlendMode, AUXElement.TargetFlashFrames, AUXElement.TargetFlashCount);
                                this.graphicModifiers.Add(item);
                            }
                        }
                    }
                    Color? senderFlashColor = AUXElement.SenderFlashColor;
                    if (senderFlashColor != null && this.senderGraphic is IndexedColorGraphic)
                    {
                        IndexedColorGraphic graphic3 = this.senderGraphic as IndexedColorGraphic;
                        Color? senderFlashColor2 = AUXElement.SenderFlashColor;
                        GraphicFader item2 = new GraphicFader(graphic3, senderFlashColor2.Value, AUXElement.SenderFlashBlendMode, AUXElement.SenderFlashFrames, AUXElement.SenderFlashCount);
                        this.graphicModifiers.Add(item2);
                    }
                    foreach (int num in this.targetCardIds)
                    {
                        if (num >= 0)
                        {
                            this.cardBar.SetSpring(num, AUXElement.CardSpringMode, AUXElement.CardSpringAmplitude, AUXElement.CardSpringSpeed, AUXElement.CardSpringDecay);
                        }
                    }
                }
            }
            if (this.screenDarkenShape != null && !this.darkenedFlag)
            {
                if (Math.Abs(this.targetAlpha - this.screenDarkenShape.Shape.FillColor.A) > 1)
                {
                    this.UpdateDarkenColor();
                }
                else
                {
                    Color fillColor = this.screenDarkenShape.Shape.FillColor;
                    fillColor.A = this.targetAlpha;
                    this.screenDarkenShape.Shape.FillColor = fillColor;
                    if (this.targetAlpha == 0)
                    {
                        foreach (Graphic graphic4 in this.targetGraphics)
                        {
                            if (this.depthMemory.ContainsKey(graphic4))
                            {
                                graphic4.Depth = this.depthMemory[graphic4];
                            }
                        }
                    }
                    this.animatingCount--;
                    this.darkenedFlag = true;
                }
            }
            this.step++;
            this.complete = (!this.animation.HasElements && this.animatingCount == 0);
            if (this.complete && !this.completedFlag)
            {
                if (this.OnAnimationComplete != null)
                {
                    this.OnAnimationComplete(this);
                }
                this.completedFlag = true;
            }
        }

        private void GraphicAnimationComplete(MultipartAnimation anim)
        {
            anim.Visible = false;
            this.pipeline.Remove(anim);
            anim.OnAnimationComplete -= this.GraphicAnimationComplete;
            this.animatingCount--;
        }

        private const int DARKEN_SHAPE_DEPTH = 32667;

        private const int DARKEN_GRAPHIC_DEPTH = 32677;

        private const float FADE_SPEED = 0.2f;

        private readonly RenderPipeline pipeline;

        private readonly AUXElementList animation;

        private readonly Graphic senderGraphic;

        private readonly Graphic[] targetGraphics;

        private readonly CardBar cardBar;

        private readonly Shape screenShape;

        private ShapeGraphic screenDarkenShape;

        private byte sourceAlpha;

        private byte targetAlpha;

        private float alphaMultiplier;

        private bool darkenedFlag;

        private Dictionary<Graphic, int> depthMemory;

        private readonly List<IGraphicModifier> graphicModifiers;

        private readonly int[] targetCardIds;

        private bool complete;

        private bool completedFlag;

        private int step;

        private int animatingCount;

        public delegate void AnimationCompleteHandler(AUXAnimator anim);
    }
}
