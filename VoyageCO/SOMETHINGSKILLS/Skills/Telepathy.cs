using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using VCO.AUX;
using VCO.Battle;
using VCO.Battle.Actions;
using VCO.Battle.Combatants;
using VCO.Data;
using VCO.GUI.Modifiers;
using Violet.Graphics;

namespace VCO.SOMETHING
{
    public class Telepathy : AUXBase
    {
        public override int AUCost => 0;
        public override TargetingMode TargetMode => TargetingMode.Enemy;
        public override int[] Symbols => new int[2];
        public override string QualifiedName => "Telapathy";
        public override string Key => "1";
        internal override IAUX identifier => new AssistiveAUX();
        private RenderPipeline pipeline;
        public Telepathy()
        {
            Console.WriteLine("THE PURPOSE OF MAN IS TO PERFORM TELEPATHY");
        }

        private Graphic[] targetGraphics;
        private Shape screenShape;
        private ShapeGraphic screenDarkenShape;
        private byte sourceAlpha;
        private byte targetAlpha;
        private float alphaMultiplier;
        private bool darkenedFlag;
        private Dictionary<Graphic, int> depthMemory;
        private List<IGraphicModifier> graphicModifiers;
        private int[] targetCardIds;
        internal override void Initialize(PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, Combatant[] targets, int level)
        {
            string message = string.Format("{0} tried {1} {2}!", CharacterNames.GetName(combantant.Character), QualifiedName, AUXLetters.Get(action.AUXLevel));

            action.state = PlayerAUXAction.State.WaitForUI;
            interfaceController.OnTextboxComplete += OnTextboxComplete;
            interfaceController.ShowMessage(message, false);
            interfaceController.PopCard(combantant.ID, 23);

            int[] array = new int[targets.Length];
            Graphic[] array2 = new Graphic[targets.Length];
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].Faction == BattleFaction.EnemyTeam)
                {
                    array2[i] = interfaceController.GetEnemyGraphic(targets[i].ID);
                    array[i] = -1;
                }
                else if (targets[i].Faction == BattleFaction.PlayerTeam)
                {
                    array2[i] = interfaceController.GetCardGraphic(targets[i].ID);
                    array[i] = targets[i].ID;
                }
            }
            this.targetCardIds = array;
            this.targetGraphics = array2;

            pipeline = interfaceController.pipeline;
            graphicModifiers = interfaceController.graphicModifiers;
            this.screenShape = new RectangleShape(new Vector2f(320f, 180f))
            {
                FillColor = new Color(0, 0, 0, 128)
            };
            void OnTextboxComplete()
            {
                interfaceController.OnTextboxComplete -= OnTextboxComplete;
                action.state = PlayerAUXAction.State.Animate;
            }
            Console.WriteLine("initialize");
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


        internal override void Animate(PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, Combatant[] targets, int level)
        {
            /*
            if (!darkenedFlag)
            {
                DarkenScreen(new Color(new Color(0, 0, 0, 128)), 0);
            }
            if (this.screenDarkenShape != null && !this.darkenedFlag)
            {
                if (Math.Abs((int)(this.targetAlpha - this.screenDarkenShape.Shape.FillColor.A)) > 1)
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
                 //   this.animatingCount--;
                    this.darkenedFlag = true;
                }
            }*/
            action.state = PlayerAUXAction.State.DamageNumbers;
        }

        internal override void Act(Combatant[] combatants, PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, int level)
        {
            Console.WriteLine("act");
            foreach (Combatant combatant in combatants)
            {
                EnemyCombatant enemy = combatant as EnemyCombatant;
                if (enemy == null)
                {
                    throw new Exception("Combatant is not an enemy or somehow has null data!");
                    return;
                }
                interfaceController.ShowStyledMessage($"{enemy.Enemy.GetStringQualifiedName("telepathy")}", true, "Data/Graphics/window3.dat");
                interfaceController.OnTextboxComplete += InterfaceController_OnTextboxComplete;
                void InterfaceController_OnTextboxComplete()
                {
                    interfaceController.OnTextboxComplete -= InterfaceController_OnTextboxComplete;
                    interfaceController.ResetTextboxStyle();
                    action.state = PlayerAUXAction.State.Finish;
                }
            }
            StatSet statChange2 = new StatSet
            {
                PP = -this.AUCost,
                Meter = 0.026666667f
            };
            combantant.AlterStats(statChange2);
            action.state = PlayerAUXAction.State.WaitForUI;

        }


        internal override void ScaleToLevel(PlayerCombatant combatant, int level)
        {
        }

        internal override void ShowUnavaliableMessage(PlayerCombatant combatant, BattleInterfaceController interfaceController)
        {
            interfaceController.ShowMessage("Not enough AU!", false);
        }

        internal override void Finish(Combatant[] combatants, PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, int level)
        {
            interfaceController.PopCard(combantant.ID, 0);
            action.Finish();
        }
    }
}
