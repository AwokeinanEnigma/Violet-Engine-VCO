using System;
using SunsetRhapsody.Battle.Combatants;
using SunsetRhapsody.Psi;

namespace SunsetRhapsody.Battle.UI
{
	internal struct SelectionState
	{
		public SelectionState.SelectionType Type;

		public Combatant[] Targets;
        public TargetingMode TargetingMode;
		public int AttackIndex;

		public int ItemIndex;

		public SOMETHING.AUXBase Psi;
		public IPsi Wrapper;

		public int PsiLevel;

		public enum SelectionType
		{
			Bash,
			PSI,
			Talk,
			Items,
			Guard,
			Run,
			Undo
		}
	}
}

namespace SunsetRhapsody.Battle
{
    // Token: 0x02000016 RID: 22
    public enum TargetingMode
    {
        // Token: 0x040000ED RID: 237
        None,
        // Token: 0x040000EE RID: 238
        PartyMember,
        // Token: 0x040000EF RID: 239
        AllPartyMembers,
        // Token: 0x040000F0 RID: 240
        Enemy,
        // Token: 0x040000F1 RID: 241
        AllEnemies,
        // Token: 0x040000F2 RID: 242
        Random
    }
}

