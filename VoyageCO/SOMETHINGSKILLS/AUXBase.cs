using VCO.Battle;
using VCO.Battle.Actions;
using VCO.Battle.Combatants;

namespace VCO.SOMETHING
{
    public abstract class AUXBase
    {
        internal abstract AUX.IAUX identifier { get; }

        public abstract int AUCost { get; }
        public abstract TargetingMode TargetMode { get; }
        public abstract int[] Symbols { get; }
        public abstract string QualifiedName { get; }
        public abstract string Key { get; }

        internal abstract void Initialize(PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, Combatant[] targets, int level);
        internal abstract void Animate(PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, Combatant[] targets, int level);
        internal abstract void Act(Combatant[] combatants, PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, int level);
        internal abstract void Finish(Combatant[] combatants, PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, int level);
        internal abstract void ShowUnavaliableMessage(PlayerCombatant combatant, BattleInterfaceController interfaceController);
        internal abstract void ScaleToLevel(PlayerCombatant combatant, int level);
        internal void Update(PlayerCombatant combantant, BattleInterfaceController interfaceController, PlayerAUXAction action, Combatant[] targets, int level)
        {
            // do nothing
        }

        internal virtual bool GetAvailiability(PlayerCombatant combantant, BattleInterfaceController interfaceController)
        {
            if (combantant.Stats.PP < AUCost)
            {
                return false;
            }
            return true;
        }
    }
}
