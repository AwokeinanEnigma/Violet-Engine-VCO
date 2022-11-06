﻿using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using VCO.AUX;
using VCO.Battle.Actions;
using VCO.Battle.AUXAnimation;
using VCO.Battle.Background;
using VCO.Battle.Combatants;
using VCO.Battle.UI;
using VCO.Battle.UI.Modifiers;
using VCO.Data;
using VCO.GUI;
using VCO.GUI.Modifiers;
using VCO.Scripts.Text;
using VCO.SOMETHING;
using VCO.Utility;
using Violet.Actors;
using Violet.Audio;
using Violet.Graphics;
using Violet.GUI;
using Violet.Input;
using Violet.Utility;

namespace VCO.Battle
{
    internal class BattleInterfaceController : IDisposable
    {
        public BattleBackground background;

        public bool AllowUndo
        {
            get
            {
                return this.isUndoAllowed;
            }
            set
            {
                this.isUndoAllowed = value;
            }
        }

        public int ActiveCharacter
        {
            get
            {
                return this.activeCharacter;
            }
            set
            {
                this.activeCharacter = value;
            }
        }

        public bool RunAttempted { get; set; }

        public VioletSound PrePlayerAttack
        {
            get
            {
                return this.prePlayerAttack;
            }
        }

        public VioletSound PreEnemyAttack
        {
            get
            {
                return this.preEnemyAttack;
            }
        }

        public VioletSound PreAUXSound
        {
            get
            {
                return this.preAUXSound;
            }
        }

        public VioletSound TalkSound
        {
            get
            {
                return this.talkSound;
            }
        }

        public VioletSound EnemyDeathSound
        {
            get
            {
                return this.enemyDeathSound;
            }
        }

        public VioletSound GroovySound
        {
            get
            {
                return this.groovySound;
            }
        }

        public VioletSound ReflectSound
        {
            get
            {
                return this.reflectSound;
            }
        }

        public BattleController controller;

        public event BattleInterfaceController.InteractionCompletionHandler OnInteractionComplete;

        public event BattleInterfaceController.TextboxCompletionHandler OnTextboxComplete;

        public BattleInterfaceController(RenderPipeline pipeline, ActorManager actorManager, CombatantController combatantController, bool letterboxing)
        {
            this.pipeline = pipeline;
            this.actorManager = actorManager;
            this.combatantController = combatantController;
            this.topLetterbox = new RectangleShape(new Vector2f(350f, 35f));
            this.topLetterbox.FillColor = Color.Black;
            this.topLetterbox.Rotation = 5.5f;
            this.topLetterbox.Position = new Vector2f(4f, 14f);
            this.topLetterboxY = this.topLetterbox.Position.Y;
            this.topLetterboxTargetY = letterboxing ? -35L : -54;
            this.bottomLetterbox = new RectangleShape(new Vector2f(350f, 35f));
            this.bottomLetterbox.FillColor = Color.Black;
            this.bottomLetterbox.Rotation = 5.5f;
            this.bottomLetterbox.Position = new Vector2f(0f, 180f);
            this.bottomLetterboxY = this.bottomLetterbox.Position.Y;
            this.bottomLetterboxTargetY = 180L + (letterboxing ? -35L : 0L);
            this.buttonBar = new ButtonBar(pipeline);
            actorManager.Add(this.buttonBar);

            Combatant[] factionCombatants = combatantController.GetFactionCombatants(BattleFaction.PlayerTeam);
            CharacterType[] array = new CharacterType[factionCombatants.Length];
            for (int i = 0; i < factionCombatants.Length; i++)
            {
                array[i] = ((PlayerCombatant)factionCombatants[i]).Character;
            }

            this.cardBar = new CardBar(pipeline, array, this);
            actorManager.Add(this.cardBar);
            this.AUXMenu = new SectionedAUXBox(this.pipeline, 1, 14f);
            //this.pipeline.Add(this.AUXMenu);
            this.selectionMarkers = new Dictionary<Graphic, Graphic>();
            for (int j = 0; j < array.Length; j++)
            {
                Graphic cardGraphic = this.cardBar.GetCardGraphic(j);
                Graphic graphic = new IndexedColorGraphic(Paths.GRAPHICS + "cursor.dat", "down", VectorMath.Truncate(cardGraphic.Position - cardGraphic.Origin + new Vector2f(cardGraphic.Size.X / 2f, 4f)), cardGraphic.Depth + 10);
                graphic.Visible = false;
                this.pipeline.Add(graphic);
                this.selectionMarkers.Add(cardGraphic, graphic);
            }
            region.Visible = false;
            pipeline.Add(region);
            this.enemyGraphics = new Dictionary<int, IndexedColorGraphic>();
            this.enemyIDs = new List<int>();
            this.partyIDs = new List<int>();
            foreach (Combatant combatant in combatantController.CombatantList)
            {
                switch (combatant.Faction)
                {
                    case BattleFaction.PlayerTeam:
                        {
                            PlayerCombatant playerCombatant = (PlayerCombatant)combatant;
                            playerCombatant.OnStatChange += this.OnPlayerStatChange;
                            playerCombatant.OnStatusEffectChange += this.OnPlayerStatusEffectChange;
                            this.partyIDs.Add(playerCombatant.ID);
                            break;
                        }
                    case BattleFaction.EnemyTeam:
                        {
                            EnemyCombatant enemyCombatant = (EnemyCombatant)combatant;
                            enemyCombatant.OnStatusEffectChange += this.OnEnemyStatusEffectChange;
                            IndexedColorGraphic indexedColorGraphic = new IndexedColorGraphic(Paths.GRAPHICS_ENEMIES + $"{enemyCombatant.Enemy.SpriteName}.dat", "front", default(Vector2f), 0);
                            indexedColorGraphic.CurrentPalette = uint.MaxValue;
                            indexedColorGraphic.CurrentPalette = 0U;
                            this.enemyGraphics.Add(enemyCombatant.ID, indexedColorGraphic);
                            pipeline.Add(indexedColorGraphic);
                            this.enemyIDs.Add(enemyCombatant.ID);
                            Graphic graphic2 = new IndexedColorGraphic(Paths.GRAPHICS + "cursor.dat", "down", VectorMath.Truncate(indexedColorGraphic.Position - indexedColorGraphic.Origin + new Vector2f(indexedColorGraphic.Size.X / 2f, 4f)), indexedColorGraphic.Depth + 10);
                            graphic2.Visible = false;
                            this.pipeline.Add(graphic2);
                            this.selectionMarkers.Add(indexedColorGraphic, graphic2);
                            break;
                        }
                }
            }
            this.AlignEnemyGraphics();
            this.textbox = new BattleTextbox(pipeline, 0);
            this.textbox.OnTextboxComplete += this.TextboxComplete;
            this.textbox.OnTextTrigger += this.TextTrigger;
            //pipeline.Add(this.textbox);
            this.dimmer = new ScreenDimmer(pipeline, Color.Transparent, 0, 15);
            this.state = BattleInterfaceController.State.Waiting;
            this.selectionState = default(SelectionState);
            this.selectedTargetId = -1;
            this.comboCircle = new ComboAnimator(pipeline, 0);

            this.moveBeepX = AudioManager.Instance.Use(Paths.SFX_MENU + "cursorx.wav", AudioType.Sound);
            this.moveBeepY = AudioManager.Instance.Use(Paths.SFX_MENU + "cursory.wav", AudioType.Sound);
            this.selectBeep = AudioManager.Instance.Use(Paths.SFX_MENU + "confirm.wav", AudioType.Sound);
            this.cancelBeep = AudioManager.Instance.Use(Paths.SFX_MENU + "cancel.wav", AudioType.Sound);

            this.prePlayerAttack = AudioManager.Instance.Use(Paths.SFX_BATTLE + "prePlayerAttack.wav", AudioType.Sound);
            this.preEnemyAttack = AudioManager.Instance.Use(Paths.SFX_BATTLE + "preEnemyAttack.wav", AudioType.Sound);

            this.preAUXSound = AudioManager.Instance.Use(Paths.SFX_BATTLE_AUX + "prePSI.wav", AudioType.Sound);

            this.talkSound = AudioManager.Instance.Use(Paths.SFX_BATTLE + "floydTalk.wav", AudioType.Sound);
            this.enemyDeathSound = AudioManager.Instance.Use(Paths.SFX_BATTLE + "enemyDeath.wav", AudioType.Sound);
            this.smashSound = AudioManager.Instance.Use(Paths.SFX_BATTLE + "smaaash.wav", AudioType.Sound);
            this.comboHitA = AudioManager.Instance.Use(Paths.SFX_BATTLE + "hitA.wav", AudioType.Sound);
            this.comboHitB = AudioManager.Instance.Use(Paths.SFX_BATTLE + "hitB.wav", AudioType.Sound);
            this.comboSuccess = AudioManager.Instance.Use(Paths.SFX_BATTLE + "Combo16.wav", AudioType.Sound);
            Console.WriteLine("combo");

            this.comboSoundMap = new Dictionary<CharacterType, List<VioletSound>>();
            for (int k = 0; k < array.Length; k++)
            {
                List<VioletSound> list;
                if (this.comboSoundMap.ContainsKey(array[k]))
                {
                    list = this.comboSoundMap[array[k]];
                }
                else
                {
                    list = new List<VioletSound>();
                    this.comboSoundMap.Add(array[k], list);
                }
                for (int l = 0; l < 3; l++)
                {
                    string str = CharacterComboSounds.Get(array[k], 0, l, 120);
                    VioletSound item = AudioManager.Instance.Use(Paths.SFX_BATTLE_COMBO + str, AudioType.Sound);
                    list.Add(item);
                }
            }
            this.winSounds = new Dictionary<int, VioletSound>();

            this.winSounds.Add(0, AudioManager.Instance.Use(Paths.SFX_BATTLE + "win1.wav", AudioType.Stream));
            this.winSounds.Add(1, AudioManager.Instance.Use(Paths.SFX_BATTLE + "win2.wav", AudioType.Stream));
            this.winSounds.Add(2, AudioManager.Instance.Use(Paths.SFX_BATTLE + "win3.wav", AudioType.Stream));
            this.winSounds.Add(3, AudioManager.Instance.Use(Paths.SFX_BATTLE + "win4.wav", AudioType.Stream));

            this.groovySound = AudioManager.Instance.Use(Paths.SFX_BATTLE + "Groovy.wav", AudioType.Sound);
            this.reflectSound = AudioManager.Instance.Use(Paths.SFX_BATTLE + "homerun.wav", AudioType.Sound);
            this.jingler = new LevelUpJingler(array, true);
            this.graphicModifiers = new List<IGraphicModifier>();
            this.damageNumbers = new List<DamageNumber>();
            this.AUXAnimators = new List<AUXAnimator>();
            InputManager.Instance.AxisPressed += this.AxisPressed;
            InputManager.Instance.ButtonPressed += this.ButtonPressed;


        }

        public void KillCharacter(BattleCard card)
        {


            Combatant[] combat = combatantController.GetFactionCombatants(BattleFaction.PlayerTeam);
            foreach (PlayerCombatant playa in combat)
            {
                Console.WriteLine($"type: {playa.Character} card: {card.cType}");

                if (playa.Character == card.cType)
                {
                    Console.WriteLine("kill");

                    controller.AddAction(new PlayerDeathAction(new ActionParams()
                    {
                        targets = new Combatant[1]
                        {
                            playa
                        },
                        data = null,
                        priority = int.MaxValue,
                        sender = playa,
                        controller = controller,
                    }
                    )
                    );



                    playa.AddStatusEffect(StatusEffect.Unconscious, 500);
                }
            }
        }

        ~BattleInterfaceController()
        {
            this.Dispose(false);
        }

        private void TextTrigger(TextTrigger trigger)
        {
            switch (trigger.Type)
            {
                case 0:
                    this.youWon = new YouWon(this.pipeline);
                    return;
                case 1:
                    {
                        bool flag = Enum.TryParse<CharacterType>(trigger.Data[0], true, out CharacterType character);
                        if (flag)
                        {
                            this.jingler.Play(character);
                            return;
                        }
                        break;
                    }
                case 2:
                    {
                        int.TryParse(trigger.Data[0], out int i);
                        int.TryParse(trigger.Data[1], out int hp);
                        StatSet statChange = new StatSet
                        {
                            HP = hp
                        };
                        this.combatantController[i].AlterStats(statChange);
                        return;
                    }
                case 3:
                    {
                        int.TryParse(trigger.Data[0], out int i2);
                        int.TryParse(trigger.Data[1], out int pp);
                        StatSet statChange2 = new StatSet
                        {
                            PP = pp
                        };
                        this.combatantController[i2].AlterStats(statChange2);
                        break;
                    }
                default:
                    return;
            }
        }

        public void PlayWinBGM(int type)
        {
            if (this.winSounds.ContainsKey(type))
            {
                this.winSounds[type].Play();
            }
        }

        public void StopWinBGM()
        {
            foreach (VioletSound VioletSound in this.winSounds.Values)
            {
                VioletSound.Stop();
            }
        }

        public void PlayLevelUpBGM()
        {
            this.jingler.Play();
        }

        public void EndLevelUpBGM()
        {
            this.jingler.End();
        }

        public void StopLevelUpBGM()
        {
            this.jingler.Stop();
        }

        private VioletSound GetComboSound(CharacterType character, int index)
        {
            VioletSound result = null;
            if (this.comboSoundMap.ContainsKey(character))
            {
                result = this.comboSoundMap[character][index % this.comboSoundMap[character].Count];
            }
            return result;
        }

        private void OnPlayerStatChange(Combatant sender, StatSet change)
        {
            PlayerCombatant playerCombatant = (PlayerCombatant)sender;


            this.UpdatePlayerCard(playerCombatant.ID, playerCombatant.Stats.HP, playerCombatant.Stats.PP, playerCombatant.Stats.Meter);
        }

        public int talkerID;
        public PlayerCombatant talker;

        private void OnPlayerStatusEffectChange(Combatant sender, StatusEffect statusEffect, bool added)
        {
            if (added)
            {
                if (statusEffect == StatusEffect.Talking)
                {
                    this.TalkifyPlayer(sender as PlayerCombatant);
                    this.SetCardSpring(sender.ID, BattleCard.SpringMode.BounceUp, new Vector2f(0f, 8f), new Vector2f(0f, 0.1f), new Vector2f(0f, 1f));
                    return;
                }
                switch (statusEffect)
                {
                    case StatusEffect.Shield:
                        this.SetCardGlow(sender.ID, BattleCard.GlowType.Shield);
                        return;
                    case StatusEffect.AUXShield:
                        this.SetCardGlow(sender.ID, BattleCard.GlowType.AUXSheild);
                        return;
                    case StatusEffect.Counter:
                        this.SetCardGlow(sender.ID, BattleCard.GlowType.Counter);
                        return;
                    case StatusEffect.AUXCounter:
                        this.SetCardGlow(sender.ID, BattleCard.GlowType.AUXCounter);
                        return;
                    case StatusEffect.Eraser:
                        this.SetCardGlow(sender.ID, BattleCard.GlowType.Eraser);
                        return;
                    default:
                        return;
                }
            }
            else
            {
                if (statusEffect == StatusEffect.Talking)
                {
                    this.RemoveTalker(this.cardBar.GetCardGraphic(sender.ID));
                    this.SetCardSpring(sender.ID, BattleCard.SpringMode.Normal, new Vector2f(0f, 0f), new Vector2f(0f, 0f), new Vector2f(0f, 0f));
                    return;
                }
                switch (statusEffect)
                {
                    case StatusEffect.Shield:
                    case StatusEffect.AUXShield:
                    case StatusEffect.Counter:
                    case StatusEffect.AUXCounter:
                    case StatusEffect.Eraser:
                        this.SetCardGlow(sender.ID, BattleCard.GlowType.None);
                        return;
                    default:
                        return;
                }
            }
        }

        private void OnEnemyStatusEffectChange(Combatant sender, StatusEffect statusEffect, bool added)
        {
            if (added)
            {
                if (statusEffect != StatusEffect.Talking)
                {
                    return;
                }
                this.TalkifyEnemy(sender as EnemyCombatant);
                return;
            }
            else
            {
                if (statusEffect != StatusEffect.Talking)
                {
                    return;
                }
                this.RemoveTalker(this.enemyGraphics[sender.ID]);
                return;
            }
        }

        public Graphic GetEnemyGraphic(int id)
        {
            return this.enemyGraphics[id];
        }

        public AUXAnimator AddAUXAnimation(AUXElementList animation, Combatant sender, Combatant[] targets)
        {
            Graphic senderGraphic = null;
            if (sender.Faction == BattleFaction.EnemyTeam)
            {
                senderGraphic = this.enemyGraphics[sender.ID];
            }
            else if (sender.Faction == BattleFaction.PlayerTeam)
            {
                senderGraphic = this.cardBar.GetCardGraphic(sender.ID);
            }
            int[] array = new int[targets.Length];
            Graphic[] array2 = new Graphic[targets.Length];
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].Faction == BattleFaction.EnemyTeam)
                {
                    array2[i] = this.enemyGraphics[targets[i].ID];
                    array[i] = -1;
                }
                else if (targets[i].Faction == BattleFaction.PlayerTeam)
                {
                    array2[i] = this.cardBar.GetCardGraphic(targets[i].ID);
                    array[i] = targets[i].ID;
                }
            }
            AUXAnimator AUXAnimator = new AUXAnimator(this.pipeline, this.graphicModifiers, animation, senderGraphic, array2, this.cardBar, array);
            this.AUXAnimators.Add(AUXAnimator);
            return AUXAnimator;
        }

        public DamageNumber AddDamageNumber(Combatant combatant, int number, string customNumberSet = "")
        {
            Vector2f offset = default(Vector2f);
            Vector2f position;
            if (combatant.Faction == BattleFaction.PlayerTeam)
            {
                Graphic cardGraphic = this.cardBar.GetCardGraphic(combatant.ID);
                position = new Vector2f((int)cardGraphic.Position.X, (int)cardGraphic.Position.Y) + new Vector2f((int)(cardGraphic.Size.X / 2f), 2f);
                offset.Y = -10f;
            }
            else if (combatant.Faction == BattleFaction.EnemyTeam)
            {
                Graphic graphic = this.enemyGraphics[combatant.ID];
                position = new Vector2f((int)graphic.Position.X, (int)graphic.Position.Y);
                offset.Y = (int)(-graphic.Size.Y / 3f);
            }
            else
            {
                position = new Vector2f(-320f, -180f);
            }
            DamageNumber damageNumber = new DamageNumber(this.pipeline, position, offset, 30, number, customNumberSet);
            damageNumber.SetVisibility(true);
            this.damageNumbers.Add(damageNumber);
            damageNumber.Start();
            return damageNumber;
        }

        public void StartComboCircle(EnemyCombatant enemy, PlayerCombatant player)
        {
            Graphic graphic = this.enemyGraphics[enemy.ID];
            this.comboCircle.Setup(graphic, player);
        }

        public void StopComboCircle(bool explode)
        {
            this.comboCircle.Stop(explode);
            if (explode)
            {
                this.comboSuccess.Stop();
                this.comboSuccess.Play();
            }
        }

        public void AddComboHit(int damage, int comboCount, CharacterType character, Combatant target, bool smash)
        {
            this.comboCircle.AddHit(damage, smash);
            if ((comboCount + 1) % 4 != 0)
            {
                this.comboHitA.Play();
            }
            else
            {
                this.comboHitB.Play();
            }
            if (this.hitSound != null)
            {
                this.hitSound.Stop();
            }
            this.hitSound = this.GetComboSound(character, comboCount);
            if (this.hitSound != null)
            {
                this.hitSound.Play();
            }
            if (smash)
            {
                this.smashSound.Stop();
                this.smashSound.Play();
                new BattleSmash(this.pipeline, this.enemyGraphics[target.ID].Position);
            }
        }

        public bool IsComboCircleDone()
        {
            return this.comboCircle.Stopped;
        }

        public void FlashEnemy(EnemyCombatant combatant, Color color, int duration, int count)
        {
            this.FlashEnemy(combatant, color, ColorBlendMode.Multiply, duration, count);
        }

        public void FlashEnemy(EnemyCombatant combatant, Color color, ColorBlendMode blendMode, int duration, int count)
        {
            this.graphicModifiers.Add(new GraphicFader(this.enemyGraphics[combatant.ID], color, blendMode, duration, count));
        }

        public void BlinkEnemy(EnemyCombatant combatant, int duration, int count)
        {
            this.graphicModifiers.Add(new GraphicBlinker(this.enemyGraphics[combatant.ID], duration, count));
        }

        public void TalkifyPlayer(PlayerCombatant combatant)
        {
            this.graphicModifiers.Add(new GraphicTalker(this.pipeline, this.cardBar.GetCardGraphic(combatant.ID)));
        }

        public void TalkifyEnemy(EnemyCombatant combatant)
        {
            this.graphicModifiers.Add(new GraphicTalker(this.pipeline, this.enemyGraphics[combatant.ID]));
            this.graphicModifiers.Add(new GraphicBouncer(this.enemyGraphics[combatant.ID], GraphicBouncer.SpringMode.BounceUp, new Vector2f(0f, 4f), new Vector2f(0f, 0.1f), new Vector2f(0f, 1f)));
        }

        public void RemoveTalker(Graphic graphic)
        {
            foreach (IGraphicModifier graphicModifier in this.graphicModifiers)
            {
                if (graphicModifier is GraphicTalker && graphicModifier.Graphic == graphic)
                {
                    (graphicModifier as GraphicTalker).Dispose();
                }

                if (graphicModifier is GraphicBouncer && graphicModifier.Graphic == graphic)
                {
                    (graphicModifier as GraphicBouncer).Dispose();
                }
            }
            this.graphicModifiers.RemoveAll((IGraphicModifier x) => x is GraphicTalker && x.Graphic == graphic);
            this.graphicModifiers.RemoveAll((IGraphicModifier x) => x is GraphicBouncer && x.Graphic == graphic);
        }

        public void RemoveTalkers()
        {
            foreach (IGraphicModifier graphicModifier in this.graphicModifiers)
            {
                if (graphicModifier is GraphicTalker)
                {
                    (graphicModifier as GraphicTalker).Dispose();
                }
                if (graphicModifier is GraphicBouncer)
                {
                    (graphicModifier as GraphicBouncer).Dispose();
                }
            }
            this.graphicModifiers.RemoveAll((IGraphicModifier x) => x is GraphicTalker);
            this.graphicModifiers.RemoveAll((IGraphicModifier x) => x is GraphicBouncer);
            foreach (PlayerCombatant com in combatantController.GetFactionCombatants(BattleFaction.PlayerTeam))
            {
                com.RemoveStatusEffect(StatusEffect.Talking);
            }
        }
        public void AddShieldAnimation(Combatant combatant)
        {
            Graphic graphic = null;
            if (combatant is PlayerCombatant)
            {
                graphic = this.cardBar.GetCardGraphic(combatant.ID);
            }
            else if (combatant is EnemyCombatant)
            {
                graphic = this.enemyGraphics[combatant.ID];
            }
            if (graphic != null)
            {
                this.graphicModifiers.Add(new GraphicShielder(this.pipeline, graphic));
            }
        }
        TextRegion region = new TextRegion(new Vector2f(3, 3), 32767, Fonts.Main, "thing");

        private void SetSelectionMarkerVisibility(Graphic graphic, bool visible)
        {
            Graphic graphic2 = this.selectionMarkers[graphic];
            if (visible)
            {
                Vector2f cursorPosition = VectorMath.Truncate(graphic.Position - graphic.Origin + new Vector2f(graphic.Size.X / 2f, 4f)); ;
                graphic2.Position = cursorPosition;
                graphic2.Depth = 32767;
                this.pipeline.Update(graphic2);
                //int onTheY = ;
                //this.card.Position.Y - this.card.Origin.Y + (float)((int)(Engine.Random.NextDouble() * (double)this.card.TextureRect.Height))
                int val = DictionaryExtensions.KeyByValue<int, IndexedColorGraphic>(enemyGraphics, (IndexedColorGraphic)graphic);
                IEnumerable<Combatant> combatboy = combatantController.GetFactionCombatants(BattleFaction.EnemyTeam).Where(x => x.ID == val);
                EnemyCombatant enemyCombatant = (EnemyCombatant)combatboy.FirstOrDefault();
                string enemyName = enemyCombatant.Enemy.PlayerFriendlyName;
                //	Vector2f testMiddle = - graphic.Size / 2);
                //Vector2f position = new Vector2f(cursorPosition.X - graphic.Size.X - (enemyName.Length/2), (int)(graphic.Position.Y - graphic.Origin.Y + graphic.TextureRect.Height));
                //int thing = (cursorPosition.X - enemyName.Length * 2.35f);

                Vector2f position = new Vector2f(cursorPosition.X - enemyName.Length * 2.27f, (int)(graphic.Position.Y - graphic.Origin.Y + graphic.TextureRect.Height));
                region.Position = VectorMath.Truncate(position);//new Vector2f(graphic.Position.X, onTheY); //VectorMath.Truncate(graphic.Position - graphic.Origin + new Vector2f(graphic.Size.X / 2f, 4f));
                region.Reset(enemyName, 0, enemyName.Length);
                pipeline.Update(region);
                ShowMessage(enemyName, false);
            }
            //region.Visible = true;
            graphic2.Visible = visible;
        }
        private void ResetTargetingSelection()
        {
            using (Dictionary<int, IndexedColorGraphic>.Enumerator enumerator = this.enemyGraphics.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<int, IndexedColorGraphic> kvp = enumerator.Current;
                    this.graphicModifiers.RemoveAll(x => x.Graphic == kvp.Value && x is GraphicFader);
                    if (this.selectionState.TargetingMode == TargetingMode.Enemy)
                    {
                        KeyValuePair<int, IndexedColorGraphic> kvp18 = kvp;
                        if (kvp18.Key == this.selectedTargetId)
                        {
                            KeyValuePair<int, IndexedColorGraphic> kvp2 = kvp;
                            kvp2.Value.Color = Color.White;
                            List<IGraphicModifier> list = this.graphicModifiers;
                            KeyValuePair<int, IndexedColorGraphic> kvp3 = kvp;
                            list.Add(new GraphicFader(kvp3.Value, new Color(64, 64, 64), ColorBlendMode.Screen, 30, -1));
                            KeyValuePair<int, IndexedColorGraphic> kvp4 = kvp;
                            this.SetSelectionMarkerVisibility(kvp4.Value, true);
                        }
                        else
                        {
                            KeyValuePair<int, IndexedColorGraphic> kvp5 = kvp;
                            kvp5.Value.ColorBlendMode = ColorBlendMode.Multiply;
                            KeyValuePair<int, IndexedColorGraphic> kvp6 = kvp;
                            kvp6.Value.Color = new Color(128, 128, 128);
                            KeyValuePair<int, IndexedColorGraphic> kvp7 = kvp;
                            this.SetSelectionMarkerVisibility(kvp7.Value, false);
                        }
                    }
                    else if (this.selectionState.TargetingMode == TargetingMode.AllEnemies)
                    {
                        KeyValuePair<int, IndexedColorGraphic> kvp8 = kvp;
                        kvp8.Value.Color = Color.White;
                        List<IGraphicModifier> list2 = this.graphicModifiers;
                        KeyValuePair<int, IndexedColorGraphic> kvp9 = kvp;
                        list2.Add(new GraphicFader(kvp9.Value, new Color(64, 64, 64), ColorBlendMode.Screen, 30, -1));
                        KeyValuePair<int, IndexedColorGraphic> kvp10 = kvp;
                        this.SetSelectionMarkerVisibility(kvp10.Value, true);
                    }
                    else if (this.selectionState.TargetingMode == TargetingMode.PartyMember || this.selectionState.TargetingMode == TargetingMode.AllPartyMembers)
                    {
                        KeyValuePair<int, IndexedColorGraphic> kvp11 = kvp;
                        kvp11.Value.ColorBlendMode = ColorBlendMode.Multiply;
                        KeyValuePair<int, IndexedColorGraphic> kvp12 = kvp;
                        kvp12.Value.Color = new Color(128, 128, 128);
                        KeyValuePair<int, IndexedColorGraphic> kvp13 = kvp;
                        this.SetSelectionMarkerVisibility(kvp13.Value, false);
                    }
                    else
                    {
                        KeyValuePair<int, IndexedColorGraphic> kvp14 = kvp;
                        kvp14.Value.ColorBlendMode = ColorBlendMode.Multiply;
                        KeyValuePair<int, IndexedColorGraphic> kvp15 = kvp;
                        kvp15.Value.Color = Color.White;
                        KeyValuePair<int, IndexedColorGraphic> kvp16 = kvp;
                        this.SetSelectionMarkerVisibility(kvp16.Value, false);
                    }
                }
            }
            for (int i = 0; i < this.partyIDs.Count; i++)
            {
                Graphic cardGraphic = this.cardBar.GetCardGraphic(i);
                if (this.selectionState.TargetingMode == TargetingMode.PartyMember)
                {
                    this.SetSelectionMarkerVisibility(cardGraphic, this.partySelectIndex == i);
                }
                else if (this.selectionState.TargetingMode == TargetingMode.AllPartyMembers)
                {
                    this.SetSelectionMarkerVisibility(cardGraphic, true);
                }
                else
                {
                    this.SetSelectionMarkerVisibility(cardGraphic, false);
                }
            }
        }
        private void AlignEnemyGraphics()
        {
            int num = 0;
            int num2 = 320 / (this.enemyGraphics.Count + 1);
            for (int i = 0; i < this.enemyIDs.Count; i++)
            {
                int id = this.enemyIDs[i];
                num += num2;
                Vector2f vector2f = new Vector2f(num, 78 + ((i % 2 == 0) ? 0 : 12));
                this.enemyGraphics[id].Depth = (int)vector2f.Y - 78;
                if (this.graphicModifiers != null)
                {
                    Console.WriteLine("old:({0},{1}) new:({2},{3})", new object[]
                    {
                        this.enemyGraphics[id].Position.X,
                        this.enemyGraphics[id].Position.Y,
                        vector2f.X,
                        vector2f.Y
                    });
                    for (int j = 0; j < this.graphicModifiers.Count; j++)
                    {
                        if (this.graphicModifiers[j].Graphic == this.enemyGraphics[id])
                        {
                            this.graphicModifiers.Remove(this.graphicModifiers[j]);
                            Console.WriteLine("removed");
                        }
                    }
                    this.graphicModifiers.RemoveAll((IGraphicModifier x) => x.Graphic == this.enemyGraphics[id] && x is GraphicTranslator);
                    this.graphicModifiers.Add(new GraphicTranslator(this.enemyGraphics[id], vector2f, 10));
                }
                else
                {
                    this.enemyGraphics[id].Position = vector2f;
                }
            }
        }

        public void DoGroovy(int id)
        {
            if (this.groovy != null)
            {
                this.groovy.Dispose();
            }
            Vector2f cardTopMiddle = this.cardBar.GetCardTopMiddle(id);
            this.groovy = new Groovy(this.pipeline, cardTopMiddle);
            this.groovySound.Play();
        }

        private void TextboxComplete()
        {
            this.textboxHideFlag = true;
            if (this.youWon != null)
            {
                this.youWon.Remove();
                this.youWon.Dispose();
                this.youWon = null;
            }
            if (this.OnTextboxComplete != null)
            {
                this.OnTextboxComplete();
            }
        }

        private void AxisPressed(InputManager sender, Vector2f axis)
        {
            bool flag = axis.X < 0f;
            bool flag2 = axis.X > 0f;
            bool flag3 = axis.Y < 0f;
            bool flag4 = axis.Y > 0f;
            if (this.state != BattleInterfaceController.State.Waiting)
            {
                if (flag || flag2)
                {
                    this.moveBeepX.Play();
                }
                if (flag3 || flag4)
                {
                    this.moveBeepY.Play();
                }
            }
            switch (this.state)
            {
                case BattleInterfaceController.State.Waiting:
                case BattleInterfaceController.State.AUXAttackSelection:
                case BattleInterfaceController.State.SpecialSelection:
                case BattleInterfaceController.State.ItemSelection:
                    break;
                case BattleInterfaceController.State.TopLevelSelection:
                    if (flag)
                    {
                        this.buttonBar.SelectLeft();
                        return;
                    }
                    if (flag2)
                    {
                        this.buttonBar.SelectRight();
                        return;
                    }
                    break;
                case BattleInterfaceController.State.AUXTypeSelection:
                    if (flag3)
                    {
                        this.AUXMenu.SelectUp();
                        return;
                    }
                    if (flag4)
                    {
                        this.AUXMenu.SelectDown();
                        return;
                    }
                    if (flag)
                    {
                        this.AUXMenu.SelectLeft();
                        return;
                    }
                    if (flag2)
                    {
                        this.AUXMenu.SelectRight();
                        return;
                    }
                    break;
                case BattleInterfaceController.State.EnemySelection:
                    if (flag)
                    {
                        this.enemySelectIndex--;
                        if (this.enemySelectIndex < 0)
                        {
                            this.enemySelectIndex = this.enemyIDs.Count - 1;
                        }
                        this.selectedTargetId = this.enemyIDs[this.enemySelectIndex];
                        this.ResetTargetingSelection();
                        return;
                    }
                    if (flag2)
                    {
                        this.enemySelectIndex++;
                        if (this.enemySelectIndex >= this.enemyIDs.Count)
                        {
                            this.enemySelectIndex = 0;
                        }
                        this.selectedTargetId = this.enemyIDs[this.enemySelectIndex];
                        this.ResetTargetingSelection();
                        return;
                    }
                    break;
                case BattleInterfaceController.State.AllySelection:
                    if (flag)
                    {
                        this.partySelectIndex--;
                        if (this.partySelectIndex < 0)
                        {
                            this.partySelectIndex = this.partyIDs.Count - 1;
                        }
                        this.selectedTargetId = this.partyIDs[this.partySelectIndex];
                        this.ResetTargetingSelection();
                        return;
                    }
                    if (flag2)
                    {
                        this.partySelectIndex++;
                        if (this.partySelectIndex >= this.partyIDs.Count)
                        {
                            this.partySelectIndex = 0;
                        }
                        this.selectedTargetId = this.partyIDs[this.partySelectIndex];
                        this.ResetTargetingSelection();
                    }
                    break;
                default:
                    return;
            }
        }

        private void ShowAUXTypeSelector(Func<PlayerCombatant> CurrentPlayerCombatant)
        {
            throw new NotImplementedException();
        }

        private void ButtonPressed(InputManager sender, Button b)
        {

            if (this.state != BattleInterfaceController.State.Waiting)
            {
                if (b == Button.A)
                {
                    this.selectBeep.Play();
                }
                if (b == Button.B)
                {
                    this.cancelBeep.Play();
                }
                if (b == Button.One)
                {
                    this.SetCardGlow(combatantController.GetFactionCombatants(BattleFaction.PlayerTeam)[0].ID, BattleCard.GlowType.Counter);
                    Console.WriteLine($"counter");

                }
                if (b == Button.Two)
                {
                    this.SetCardGlow(combatantController.GetFactionCombatants(BattleFaction.PlayerTeam)[0].ID, BattleCard.GlowType.Eraser);
                    Console.WriteLine($"eraser");
                }

                if (b == Button.Three)
                {
                    this.SetCardGlow(combatantController.GetFactionCombatants(BattleFaction.PlayerTeam)[0].ID, BattleCard.GlowType.AUXCounter);
                    Console.WriteLine($"AUXCounter");
                }
                if (b == Button.Five)
                {
                    this.SetCardGlow(combatantController.GetFactionCombatants(BattleFaction.PlayerTeam)[0].ID, BattleCard.GlowType.AUXSheild);
                    Console.WriteLine($"AUXCounter");
                }
                if (b == Button.Five)
                {
                    this.SetCardGlow(combatantController.GetFactionCombatants(BattleFaction.PlayerTeam)[0].ID, BattleCard.GlowType.Shield);
                    Console.WriteLine($"Shield");
                }
                if (b == Button.Five)
                {
                    this.SetCardGlow(combatantController.GetFactionCombatants(BattleFaction.PlayerTeam)[0].ID, BattleCard.GlowType.None);
                    Console.WriteLine($"none");
                }
            }
            switch (this.state)
            {
                case BattleInterfaceController.State.Waiting:
                    break;
                case BattleInterfaceController.State.TopLevelSelection:
                    this.TopLevelSelection(b);
                    return;
                case BattleInterfaceController.State.AUXTypeSelection:
                    this.AUXTypeSelection(b);
                    return;
                case BattleInterfaceController.State.AUXAttackSelection:
                    this.AUXAttackSelection(b);
                    return;
                case BattleInterfaceController.State.SpecialSelection:
                    this.SpecialSelection(b);
                    return;
                case BattleInterfaceController.State.ItemSelection:
                    this.ItemSelection(b);
                    return;
                case BattleInterfaceController.State.EnemySelection:
                case BattleInterfaceController.State.AllySelection:
                    this.TargetSelection(b);
                    break;
                default:
                    return;
            }
        }

        private PlayerCombatant CurrentPlayerCombatant()
        {
            return (PlayerCombatant)this.combatantController.GetFactionCombatants(BattleFaction.PlayerTeam)[this.cardBar.SelectedIndex];
        }

        private void StartTargetSelection()
        {
            if (this.selectionState.TargetingMode == TargetingMode.None)
            {
                this.CompleteTargetSelection(this.buttonBar.SelectedAction);
                return;
            }
            if (this.selectionState.TargetingMode == TargetingMode.Enemy)
            {
                this.state = BattleInterfaceController.State.EnemySelection;
                this.selectedTargetId = this.enemyIDs[this.enemySelectIndex % this.enemyIDs.Count];
            }
            else if (this.selectionState.TargetingMode == TargetingMode.AllEnemies)
            {
                this.state = BattleInterfaceController.State.EnemySelection;
                this.selectedTargetId = -1;
            }
            else if (this.selectionState.TargetingMode == TargetingMode.PartyMember)
            {
                this.state = BattleInterfaceController.State.AllySelection;
                this.selectedTargetId = this.partyIDs[this.partySelectIndex % this.partyIDs.Count];
            }
            else if (this.selectionState.TargetingMode == TargetingMode.AllPartyMembers)
            {
                this.state = BattleInterfaceController.State.AllySelection;
                this.selectedTargetId = -1;
            }
            this.buttonBar.Hide();
            this.ResetTargetingSelection();
        }

        public void HideAUX()
        {
            //	this.AUXMenu
        }

        private void TopLevelSelection(Button b)
        {
            switch (b)
            {
                case Button.A:
                    switch (this.buttonBar.SelectedAction)
                    {
                        case ButtonBar.Action.Bash:
                            this.selectionState.TargetingMode = TargetingMode.Enemy;
                            this.StartTargetSelection();
                            return;
                        case ButtonBar.Action.AUX:
                            {
                                Console.Write("AUX selected");
                                PlayerCombatant playerCombatant = this.CurrentPlayerCombatant();
                                //if (playerCombatant.GetStatusEffects().ToList().Contains())
                                this.AUXMenu.Reset();
                                this.AUXMenu.MaxLevel = 3; // this.CurrentPlayerCombatant().Stats.Level;
                                Console.WriteLine($"Current Character: { playerCombatant.Character}");
                                this.AUXMenu.OffenseAUXItems = AUXManager.Instance.GetCharacterOffenseAUX(playerCombatant.Character);
                                foreach (OffenseAUX AUX in AUXMenu.OffenseAUXItems)
                                {
                                    Console.WriteLine(AUX.aux.QualifiedName);
                                }
                                this.AUXMenu.DefenseAUXItems = AUXManager.Instance.GetCharacterDefenseAUX(playerCombatant.Character);
                                this.AUXMenu.AssistAUXItems = AUXManager.Instance.GetCharacterAssistAUX(playerCombatant.Character);
                                this.AUXMenu.OtherAUXItems = AUXManager.Instance.GetCharacterOtherAUX(playerCombatant.Character);
                                this.state = BattleInterfaceController.State.AUXTypeSelection;
                                this.buttonBar.Hide();
                                this.AUXMenu.Show();
                                return;
                            }
                        case ButtonBar.Action.Items:
                            this.state = BattleInterfaceController.State.ItemSelection;
                            this.buttonBar.Hide();
                            return;
                        case ButtonBar.Action.Talk:
                            this.selectionState.TargetingMode = TargetingMode.Enemy;
                            this.state = BattleInterfaceController.State.EnemySelection;
                            this.buttonBar.Hide();
                            this.selectedTargetId = this.enemyIDs[this.enemySelectIndex % this.enemyIDs.Count];
                            this.ResetTargetingSelection();
                            return;
                        case ButtonBar.Action.Guard:
                            this.CompleteMenuGuard();
                            return;
                        case ButtonBar.Action.Run:
                            this.buttonBar.Hide();
                            this.RunAttempted = true;
                            this.CompleteMenuRun();
                            this.state = BattleInterfaceController.State.Waiting;
                            return;
                        default:
                            throw new NotImplementedException("Tried to use unimplemented button action.");
                    }
                    break;
                case Button.B:
                    if (this.isUndoAllowed)
                    {
                        this.CompleteMenuUndo();
                    }
                    return;
                default:
                    return;
            }
        }

        private void AUXTypeSelection(Button b)
        {
            switch (b)
            {
                case Button.A:
                    {
                        if (this.AUXMenu.InTypeSelection())
                        {
                            this.AUXMenu.SelectRight();
                            return;
                        }
                        AUXType AUXType = this.AUXMenu.SelectedAUXType();
                        Console.WriteLine(AUXType);
                        int num = this.AUXMenu.SelectedLevel();
                        IAUX AUX;
                        switch (AUXType)
                        {
                            case AUXType.Offense:
                                AUX = this.AUXMenu.SelectOffenseAUX();
                                break;
                            case AUXType.Defense:
                                AUX = this.AUXMenu.SelectDefenseAUX();
                                break;
                            case AUXType.Assist:
                                AUX = this.AUXMenu.SelectAssistAUX();
                                break;
                            case AUXType.Other:
                                AUX = this.AUXMenu.SelectOtherAUX();
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                        AUXBase aux = AUX.aux;
                        if (!aux.GetAvailiability(CurrentPlayerCombatant(), this))
                        {
                            aux.ShowUnavaliableMessage(CurrentPlayerCombatant(), this);
                            return;
                        }
                        this.AUXMenu.Hide();
                        this.selectionState.TargetingMode = aux.TargetMode;
                        this.StartTargetSelection();
                        this.selectionState.AUX = aux;
                        selectionState.Wrapper = AUX;
                        this.selectionState.AUXLevel = num;
                        return;
                    }
                case Button.B:
                    this.AUXMenu.Hide();
                    this.state = BattleInterfaceController.State.TopLevelSelection;
                    this.buttonBar.Show();
                    return;
                default:
                    return;
            }
        }

        private void AUXAttackSelection(Button b)
        {
            switch (b)
            {
                case Button.A:
                    break;
                case Button.B:
                    this.AUXMenu.Show();
                    this.state = BattleInterfaceController.State.TopLevelSelection;
                    break;
                default:
                    return;
            }
        }

        private void SpecialSelection(Button b)
        {
            switch (b)
            {
                case Button.A:
                    break;
                case Button.B:
                    this.state = BattleInterfaceController.State.TopLevelSelection;
                    this.buttonBar.Show();
                    break;
                default:
                    return;
            }
        }

        private void ItemSelection(Button b)
        {
            switch (b)
            {
                case Button.A:
                    break;
                case Button.B:
                    this.state = BattleInterfaceController.State.TopLevelSelection;
                    this.buttonBar.Show();
                    break;
                default:
                    return;
            }
        }

        private void TargetSelection(Button b)
        {
            switch (b)
            {
                case Button.A:
                    this.CompleteTargetSelection(this.buttonBar.SelectedAction);
                    return;
                case Button.B:
                    this.selectedTargetId = -1;
                    this.selectionState.TargetingMode = TargetingMode.None;
                    this.ResetTargetingSelection();
                    this.state = BattleInterfaceController.State.TopLevelSelection;
                    this.ShowButtonBar();
                    return;
                default:
                    return;
            }
        }

        private void CompleteMenuUndo()
        {
            if (this.OnInteractionComplete != null)
            {
                this.selectionState.Type = SelectionState.SelectionType.Undo;
                this.OnInteractionComplete(this.selectionState);
            }
        }

        private void CompleteTargetSelection(ButtonBar.Action buttonAction)
        {
            if (this.OnInteractionComplete != null)
            {
                switch (buttonAction)
                {
                    case ButtonBar.Action.Bash:
                        this.selectionState.Type = SelectionState.SelectionType.Bash;
                        break;
                    case ButtonBar.Action.AUX:
                        this.selectionState.Type = SelectionState.SelectionType.AUX;
                        break;
                    case ButtonBar.Action.Talk:
                        this.selectionState.Type = SelectionState.SelectionType.Talk;
                        break;
                }
                switch (this.selectionState.TargetingMode)
                {
                    case TargetingMode.PartyMember:
                    case TargetingMode.Enemy:
                        this.selectionState.Targets = new Combatant[]
                        {
                            this.combatantController[this.selectedTargetId]
                        };
                        break;
                    case TargetingMode.AllPartyMembers:
                        this.selectionState.Targets = this.combatantController.GetFactionCombatants(BattleFaction.PlayerTeam);
                        break;
                    case TargetingMode.AllEnemies:
                        this.selectionState.Targets = this.combatantController.GetFactionCombatants(BattleFaction.EnemyTeam);
                        break;
                }
                this.selectionState.AttackIndex = 0;
                this.selectionState.ItemIndex = -1;
                this.state = BattleInterfaceController.State.Waiting;
                if (this.OnInteractionComplete != null)
                {
                    this.OnInteractionComplete(this.selectionState);
                }
            }
            this.selectedTargetId = -1;
            this.selectionState.TargetingMode = TargetingMode.None;
            this.ResetTargetingSelection();
        }


        private void CompleteMenuGuard()
        {
            if (this.OnInteractionComplete != null)
            {
                this.selectionState.Type = SelectionState.SelectionType.Guard;
                this.selectionState.Targets = null;
                this.selectionState.AttackIndex = -1;
                this.selectionState.ItemIndex = -1;
                this.state = BattleInterfaceController.State.Waiting;
                this.OnInteractionComplete(this.selectionState);
            }
        }

        // Token: 0x0600046B RID: 1131 RVA: 0x0001D458 File Offset: 0x0001B658
        private void CompleteMenuRun()
        {
            if (this.OnInteractionComplete != null)
            {
                this.selectionState.Type = SelectionState.SelectionType.Run;
                this.selectionState.Targets = null;
                this.selectionState.AttackIndex = -1;
                this.selectionState.ItemIndex = -1;
                this.state = BattleInterfaceController.State.Waiting;
                this.OnInteractionComplete(this.selectionState);
            }
        }

        public void BeginPlayerInteraction(CharacterType character)
        {
            int num = 0;
            PlayerCombatant playerCombatant = null;
            foreach (Combatant combatant in this.combatantController.GetFactionCombatants(BattleFaction.PlayerTeam))
            {
                playerCombatant = (PlayerCombatant)combatant;
                if (playerCombatant.Character == character)
                {
                    break;
                }
                num++;
            }
            Combatant firstLiveCombatant = this.combatantController.GetFirstLiveCombatant(BattleFaction.PlayerTeam);
            bool showRun = firstLiveCombatant != null && firstLiveCombatant.ID == playerCombatant.ID;
            this.state = BattleInterfaceController.State.TopLevelSelection;
            bool lockAUX = false;
            foreach (StatusEffectInstance statusEffectInstance in playerCombatant.GetStatusEffects())
            {
                if (statusEffectInstance.Type == StatusEffect.DisableAUX)
                {
                    lockAUX = true;
                }
            }
            this.buttonBar.SetActions(BattleButtonBars.GetActions(character, showRun, lockAUX));
            this.buttonBar.Show(0);
            this.textbox.Hide();
            this.cardBar.SelectedIndex = num;
        }

        public void EndPlayerInteraction()
        {
            this.cardBar.SelectedIndex = -1;
        }

        public void SetActiveCard(int index)
        {
            this.cardBar.SelectedIndex = index;
        }

        public Graphic GetCardGraphic(int index)
        {
            return cardBar.GetCardGraphic(index);// this.cardBar.PopCard(index, height);
        }

        public void PopCard(int index, int height)
        {
            this.cardBar.PopCard(index, height);
        }

        public void SetCardSpring(int index, BattleCard.SpringMode mode, Vector2f amplitude, Vector2f speed, Vector2f decay)
        {
            this.cardBar.SetSpring(index, mode, amplitude, speed, decay);
        }

        public void SetCardGroovy(int index, bool groovy)
        {
            this.cardBar.SetGroovy(index, groovy);
        }

        public void AddCardSpring(int index, Vector2f amplitude, Vector2f speed, Vector2f decay)
        {
            this.cardBar.AddSpring(index, amplitude, speed, decay);
        }
        public void SetCardGlow(int index, BattleCard.GlowType type)
        {
            this.cardBar.SetGlow(index, type);
        }
        public void HideButtonBar()
        {
            this.buttonBar.Hide();
        }

        public void ShowButtonBar()
        {
            this.buttonBar.Show();
        }

        public void ShowMessage(string message, bool useButton)
        {
            this.textbox.Reset(message, useButton);
            this.textbox.Show();
        }

        public void ShowStyledMessage(string message, bool useButton, string style)
        {
            this.textbox.Reset(message, useButton);
            this.textbox.ChangeStyle(style);
            this.textbox.Show();
        }

        public void ResetTextboxStyle()
        {
            this.textbox.ChangeStyle(Settings.WindowStyle);
        }

        public void SetLetterboxing(float letterboxing)
        {
            this.topLetterboxTargetY = -400;
            this.bottomLetterboxTargetY = 180L - (int)(35f * letterboxing);
        }

        public void AddEnemy(int id)
        {
            EnemyCombatant enemyCombatant = (EnemyCombatant)this.combatantController[id];
            this.enemyIDs.Add(id);
            new IndexedColorGraphic(Paths.GRAPHICS_ENEMIES + $"{enemyCombatant.Enemy.SpriteName}.dat", "front", default(Vector2f), 0);
            this.AlignEnemyGraphics();
        }

        public void DoEnemyDeathAnimation(int id)
        {
            this.enemyDeathSound.Play();
            this.graphicModifiers.Add(new GraphicDeathFader(this.enemyGraphics[id], 40));
        }

        public void RemoveAllModifiers()
        {
            this.graphicModifiers.Clear();
        }

        public void RemoveEnemy(int id)
        {
            this.RemoveTalker(this.enemyGraphics[id]);
            this.graphicModifiers.RemoveAll((IGraphicModifier x) => x.Graphic == this.enemyGraphics[id]);
            this.pipeline.Remove(this.enemyGraphics[id]);
            this.enemyGraphics[id].Dispose();
            this.enemyGraphics.Remove(id);
            this.enemyIDs.Remove(id);
            this.AlignEnemyGraphics();
        }

        public void UpdatePlayerCard(int id, int hp, int pp, float meter)
        {
            PlayerCombatant playerCombatant = (PlayerCombatant)this.combatantController[id];
            this.cardBar.SetHP(playerCombatant.PartyIndex, hp);
            this.cardBar.SetPP(playerCombatant.PartyIndex, pp);

            this.cardBar.SetMeter(playerCombatant.PartyIndex, 0.87f);
            //this.cardBar.SetMeter(playerCombatant.PartyIndex, meter);

            // incredibly roundabout way of doing this, instead of just stopping the HP from going below 0 in the statset struct
            // also doesn't work! it causes immortality!
            /*if (playerCombatant.Stats.HP < 0) {
				StatSet newstat = playerCombatant.Stats;
				newstat.HP = 0;
				playerCombatant.SetStats(newstat);
				
			
			}*/

            if (playerCombatant.Stats.HP <= 0)
            {
                // why was this commented??
                playerCombatant.AddStatusEffect(StatusEffect.Unconscious, 500);

                foreach (BackgroundLayer bg in background.Layers)
                {
                    LayerParams param = bg.Parameters;
                    param.Speed += (float)(param.Speed * 0.1);
                    bg.UpdateParameters(param);

                }


                this.cardBar.KillCard(playerCombatant.PartyIndex);
                //KillCard


            }
        }

        public void Update()
        {
            this.textbox.Update();
            foreach (IGraphicModifier graphicModifier in this.graphicModifiers)
            {
                graphicModifier.Update();
            }
            this.graphicModifiers.RemoveAll((IGraphicModifier x) => x.Done);
            foreach (AUXAnimator AUXAnimator in this.AUXAnimators)
            {
                AUXAnimator.Update();
            }
            this.AUXAnimators.RemoveAll((AUXAnimator x) => x.Complete);
            foreach (DamageNumber damageNumber in this.damageNumbers)
            {
                damageNumber.Update();
            }
            if (this.youWon != null)
            {
                this.youWon.Update();
            }
            if (this.groovy != null)
            {
                this.groovy.Update();
            }
            this.comboCircle.Update();
            this.dimmer.Update();
            if (this.topLetterboxY < this.topLetterboxTargetY - 0.5f || this.topLetterboxY > this.topLetterboxTargetY + 0.5f)
            {
                this.topLetterboxY += (this.topLetterboxTargetY - this.topLetterboxY) / 10f;
                this.topLetterbox.Position = new Vector2f(this.topLetterbox.Position.X, (int)this.topLetterboxY);
            }
            else if ((int)this.topLetterboxY != (int)this.topLetterboxTargetY)
            {
                this.topLetterboxY = this.topLetterboxTargetY;
                this.topLetterbox.Position = new Vector2f(this.topLetterbox.Position.X, (int)this.topLetterboxY);
            }
            if (this.bottomLetterboxY > this.bottomLetterboxTargetY + 0.5f || this.bottomLetterboxY < this.bottomLetterboxTargetY - 0.5f)
            {
                this.bottomLetterboxY += (this.bottomLetterboxTargetY - this.bottomLetterboxY) / 10f;
                this.bottomLetterbox.Position = new Vector2f(this.bottomLetterbox.Position.X, (int)this.bottomLetterboxY);
            }
            else if ((int)this.bottomLetterboxY != (int)this.bottomLetterboxTargetY)
            {
                this.bottomLetterboxY = this.bottomLetterboxTargetY;
                this.bottomLetterbox.Position = new Vector2f(this.bottomLetterbox.Position.X, (int)this.bottomLetterboxY);
            }
            if (this.textboxHideFlag)
            {
                this.textbox.Hide();
                this.textboxHideFlag = false;
            }
        }

        public void Draw(RenderTarget target)
        {
            target.Draw(this.topLetterbox);
            target.Draw(this.bottomLetterbox);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    foreach (Graphic graphic in this.enemyGraphics.Values)
                    {
                        graphic.Dispose();
                    }
                    foreach (Graphic graphic2 in this.selectionMarkers.Values)
                    {
                        graphic2.Dispose();
                    }
                    foreach (IGraphicModifier graphicModifier in this.graphicModifiers)
                    {
                        if (graphicModifier is IDisposable)
                        {
                            ((IDisposable)graphicModifier).Dispose();
                        }
                    }
                    this.jingler.Stop();
                    this.jingler.Dispose();
                    this.cardBar.Dispose();
                }
                AudioManager.Instance.Unuse(this.moveBeepX);
                AudioManager.Instance.Unuse(this.moveBeepY);
                AudioManager.Instance.Unuse(this.selectBeep);
                AudioManager.Instance.Unuse(this.cancelBeep);
                AudioManager.Instance.Unuse(this.prePlayerAttack);
                AudioManager.Instance.Unuse(this.preEnemyAttack);
                AudioManager.Instance.Unuse(this.preAUXSound);
                AudioManager.Instance.Unuse(this.talkSound);
                AudioManager.Instance.Unuse(this.enemyDeathSound);
                foreach (KeyValuePair<CharacterType, List<VioletSound>> keyValuePair in this.comboSoundMap)
                {
                    List<VioletSound> value = keyValuePair.Value;
                    foreach (VioletSound sound in value)
                    {
                        AudioManager.Instance.Unuse(sound);
                    }
                }
                AudioManager.Instance.Unuse(this.smashSound);
                AudioManager.Instance.Unuse(this.comboHitA);
                AudioManager.Instance.Unuse(this.comboHitB);
                AudioManager.Instance.Unuse(this.comboSuccess);
                AudioManager.Instance.Unuse(this.groovySound);
                AudioManager.Instance.Unuse(this.reflectSound);
                foreach (VioletSound sound2 in this.winSounds.Values)
                {
                    AudioManager.Instance.Unuse(sound2);
                }
                this.textbox.OnTextboxComplete -= this.TextboxComplete;
                this.textbox.OnTextTrigger -= this.TextTrigger;
                InputManager.Instance.AxisPressed -= this.AxisPressed;
                InputManager.Instance.ButtonPressed -= this.ButtonPressed;
            }
            this.disposed = true;
        }

        private const int TOP_LETTERBOX_HEIGHT = 14;

        // Token: 0x04000622 RID: 1570
        private const int BOTTOM_LETTERBOX_HEIGHT = 35;

        // Token: 0x04000623 RID: 1571
        private const float LETTERBOX_SPEED_FACTOR = 10f;

        // Token: 0x04000624 RID: 1572
        private const int ENEMY_SPACING = 10;

        // Token: 0x04000625 RID: 1573
        private const int ENEMY_DEPTH = 0;

        // Token: 0x04000626 RID: 1574
        private const int ENEMY_TRANSLATE_FRAMES = 10;

        // Token: 0x04000627 RID: 1575
        private const int ENEMY_DEATH_FRAMES = 40;

        // Token: 0x04000628 RID: 1576
        public const int ENEMY_MIDLINE = 78;

        // Token: 0x04000629 RID: 1577
        public const int ENEMY_OFFSET = 12;

        // Token: 0x0400062A RID: 1578
        private bool disposed;

        // Token: 0x0400062B RID: 1579
        public RenderPipeline pipeline;


        // Token: 0x0400062C RID: 1580
        private ActorManager actorManager;

        // Token: 0x0400062D RID: 1581
        private CombatantController combatantController;

        // Token: 0x0400062E RID: 1582
        private Shape topLetterbox;

        // Token: 0x0400062F RID: 1583
        private Shape bottomLetterbox;

        // Token: 0x04000630 RID: 1584
        private float topLetterboxY;

        // Token: 0x04000631 RID: 1585
        private float bottomLetterboxY;

        // Token: 0x04000632 RID: 1586
        private float topLetterboxTargetY;

        // Token: 0x04000633 RID: 1587
        private float bottomLetterboxTargetY;

        // Token: 0x04000634 RID: 1588
        private ButtonBar buttonBar;

        // Token: 0x04000635 RID: 1589
        private SectionedAUXBox AUXMenu;

        // Token: 0x04000636 RID: 1590
        private CardBar cardBar;

        // Token: 0x04000637 RID: 1591
        private Dictionary<int, IndexedColorGraphic> enemyGraphics;

        // Token: 0x04000638 RID: 1592
        private BattleTextbox textbox;

        // Token: 0x04000639 RID: 1593
        private ScreenDimmer dimmer;

        // Token: 0x0400063A RID: 1594
        private ComboAnimator comboCircle;

        // Token: 0x0400063B RID: 1595
        private int selectedTargetId;

        // Token: 0x0400063C RID: 1596
        private int enemySelectIndex;

        // Token: 0x0400063D RID: 1597
        private int partySelectIndex;

        // Token: 0x0400063E RID: 1598
        private List<int> enemyIDs;

        // Token: 0x0400063F RID: 1599
        private List<int> partyIDs;

        // Token: 0x04000640 RID: 1600
        public List<IGraphicModifier> graphicModifiers;



        // Token: 0x04000641 RID: 1601
        private List<AUXAnimator> AUXAnimators;

        // Token: 0x04000642 RID: 1602
        private BattleInterfaceController.State state;

        // Token: 0x04000643 RID: 1603
        private SelectionState selectionState;

        // Token: 0x04000644 RID: 1604
        private Groovy groovy;

        // Token: 0x04000645 RID: 1605
        private VioletSound moveBeepX;

        // Token: 0x04000646 RID: 1606
        private VioletSound moveBeepY;

        // Token: 0x04000647 RID: 1607
        private VioletSound selectBeep;

        // Token: 0x04000648 RID: 1608
        private VioletSound cancelBeep;

        // Token: 0x04000649 RID: 1609
        private VioletSound prePlayerAttack;

        // Token: 0x0400064A RID: 1610
        private VioletSound preEnemyAttack;

        // Token: 0x0400064B RID: 1611
        private VioletSound preAUXSound;

        // Token: 0x0400064C RID: 1612
        private VioletSound talkSound;

        // Token: 0x0400064D RID: 1613
        private VioletSound enemyDeathSound;

        // Token: 0x0400064E RID: 1614
        private VioletSound smashSound;

        // Token: 0x0400064F RID: 1615
        private Dictionary<CharacterType, List<VioletSound>> comboSoundMap;

        // Token: 0x04000650 RID: 1616
        private VioletSound comboHitA;

        // Token: 0x04000651 RID: 1617
        private VioletSound comboHitB;

        // Token: 0x04000652 RID: 1618
        private VioletSound hitSound;

        // Token: 0x04000653 RID: 1619
        private VioletSound comboSuccess;

        // Token: 0x04000654 RID: 1620
        private VioletSound groovySound;

        // Token: 0x04000655 RID: 1621
        private VioletSound reflectSound;

        // Token: 0x04000656 RID: 1622
        private Dictionary<int, VioletSound> winSounds;

        // Token: 0x04000657 RID: 1623
        private YouWon youWon;

        // Token: 0x04000658 RID: 1624
        private LevelUpJingler jingler;

        // Token: 0x04000659 RID: 1625
        private List<DamageNumber> damageNumbers;

        // Token: 0x0400065A RID: 1626
        private Dictionary<Graphic, Graphic> selectionMarkers;

        // Token: 0x0400065B RID: 1627
        private bool textboxHideFlag;

        // Token: 0x0400065C RID: 1628
        private bool isUndoAllowed;

        // Token: 0x0400065D RID: 1629
        private int activeCharacter;

        // Token: 0x020000C7 RID: 199
        private enum State
        {
            // Token: 0x04000666 RID: 1638
            Waiting,
            // Token: 0x04000667 RID: 1639
            TopLevelSelection,
            // Token: 0x04000668 RID: 1640
            AUXTypeSelection,
            AUXAttackSelection,
            // Token: 0x04000669 RID: 1641
            SpecialSelection,
            // Token: 0x0400066A RID: 1642
            ItemSelection,
            // Token: 0x0400066B RID: 1643
            EnemySelection,
            // Token: 0x0400066C RID: 1644
            AllySelection
        }

        public delegate void InteractionCompletionHandler(SelectionState state);

        public delegate void TextboxCompletionHandler();
    }
}
