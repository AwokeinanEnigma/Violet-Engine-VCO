using System;
using VCO.Battle.Combatants;

namespace VCO.Battle.Actions
{
    internal abstract class BattleAction : IComparable
    {
        public static BattleAction GetInstance(ActionParams aparams)
        {
            return (BattleAction)Activator.CreateInstance(aparams.actionType, new object[]
            {
                aparams
            });
        }

        public event BattleAction.ActionCompleteHandler OnActionComplete;

        public int Priority => this.priority;

        public bool Complete => this.complete;

        public Combatant Sender => this.sender;

        public BattleAction(ActionParams aparams)
        {
            this.controller = aparams.controller;
            this.priority = aparams.priority;
            this.sender = aparams.sender;
            this.targets = aparams.targets;
        }

        public void Update()
        {
            this.UpdateAction();
            if (this.complete && !this.onCompleteFired && this.OnActionComplete != null)
            {
                this.OnActionComplete(this);
                this.onCompleteFired = true;
            }
        }

        protected virtual void UpdateAction()
        {
        }

        public int CompareTo(object obj)
        {
            if (obj is BattleAction)
            {
                BattleAction battleAction = (BattleAction)obj;
                return battleAction.priority - this.priority;
            }
            throw new ArgumentException("Cannot compare BattleAction to object not of type BattleAction.");
        }

        protected BattleController controller;

        protected int priority;

        protected bool complete;

        protected Combatant sender;

        protected Combatant[] targets;

        private bool onCompleteFired;

        public delegate void ActionCompleteHandler(BattleAction action);
    }
}
