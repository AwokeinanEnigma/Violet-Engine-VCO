using VCO.AUX;
using VCO.Battle.Combatants;

namespace VCO.Battle.UI
{
    internal struct SelectionState
    {
        public SelectionState.SelectionType Type;

        public Combatant[] Targets;
        public TargetingMode TargetingMode;
        public int AttackIndex;

        public int ItemIndex;

        public SOMETHING.AUXBase AUX;
        public IAUX Wrapper;

        public int AUXLevel;

        public enum SelectionType
        {
            Bash,
            AUX,
            Talk,
            Items,
            Guard,
            Run,
            Undo
        }
    }
}

namespace VCO.Battle
{
    public enum TargetingMode
    {
        None,
        PartyMember,
        AllPartyMembers,
        Enemy,
        AllEnemies,
        Random
    }
}

