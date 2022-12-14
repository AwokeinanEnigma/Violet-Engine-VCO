using System;
using VCO.Battle.Combatants;
using VCO.Battle.EnemyAI;
using VCO.Data;
using VCO.Data.Enemies;

namespace VCO.Battle.Actions
{
    internal class EnemyDecisionAction : DecisionAction
    {
        public EnemyDecisionAction(ActionParams aparams, EnemyData data) : base(aparams)
        {
            this.enemyType = (this.sender as EnemyCombatant).Enemy;
            Console.WriteLine("Created another decision!");
            switch (enemyType.AIName)
            {
                case "TravisMustDie":
                    aicontrol = new TravisMustDieAI(controller, sender, data);
                    break;
                case "BoilerPlateAI":
                    aicontrol = new BoilerplateAI(controller, sender, data);
                    break;
                case "FrooguAI":
                    aicontrol = new FrooguAI(controller, Sender, data);
                    break;
                case "hermitcanai":
                    aicontrol = new HermitCanAI(controller, Sender, data);
                    break;
                case "infestedai":
                    aicontrol = new InfestedLegsAI(controller, Sender, data);
                    break;
                case "demiurgeai":
                    aicontrol = new DemiurgeFilamentAI(controller, Sender, data);
                    break;
                default:
                    aicontrol = new BoilerplateAI(controller, Sender, data);
                    break;
            }

            //this.aicontrol = new RandomAI(this.controller, this.sender);
        }

        public void UpdateActionParams(ActionParams aparams, EnemyData data)
        {
            this.aparams = aparams;
            this.controller = aparams.controller;
            this.priority = aparams.priority;
            this.sender = aparams.sender;
            this.targets = aparams.targets;
            this.enemyType = data;
        }

        protected override void UpdateAction()
        {
            base.UpdateAction();
            bool flag = false;
            StatusEffectInstance[] statusEffects = this.sender.GetStatusEffects();
            if (statusEffects.Length > 0)
            {
                foreach (StatusEffectInstance statusEffectInstance in statusEffects)
                {
                    Type type = StatusEffectActions.Get(statusEffectInstance.Type);
                    if (type != null)
                    {
                        ActionParams aparams = new ActionParams
                        {
                            actionType = type,
                            controller = this.controller,
                            sender = this.sender,
                            priority = this.sender.Stats.Speed,
                            targets = this.sender.SavedTargets,
                            data = new object[]
                            {
                                statusEffectInstance
                            }
                        };
                        this.controller.AddAction(BattleAction.GetInstance(aparams));
                        flag |= (statusEffectInstance.Type == StatusEffect.Talking);
                    }
                }
            }
            if (!flag)
            {
                Combatant[] factionCombatants = this.controller.CombatantController.GetFactionCombatants(BattleFaction.PlayerTeam);
                BattleAction action = this.aicontrol.GetAction(this.sender.Stats.Speed, factionCombatants);
                this.controller.AddAction(action);
                Console.WriteLine($"added action: action is {action}");
            }

            this.complete = true;
        }

        private EnemyData enemyType;
        private ActionParams aparams;
        private readonly IEnemyAI aicontrol;
    }
}
