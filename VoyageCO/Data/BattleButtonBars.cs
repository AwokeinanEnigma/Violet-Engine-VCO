using System.Collections.Generic;
using VCO.AUX;
using VCO.Battle.UI;

namespace VCO.Data
{
    internal static class BattleButtonBars
    {
        public static ButtonBar.Action[] GetActions(CharacterType character, bool showRun, bool lockedAUX = false)
        {
            List<ButtonBar.Action> list = new List<ButtonBar.Action>
            {
                ButtonBar.Action.Bash
            };
            if (character != CharacterType.Floyd)
            {
                if (character != CharacterType.Zack && AUXManager.Instance.CharacterHasAUX(character))
                {
                    list.Add(ButtonBar.Action.AUX);
                }
            }
            else
            {
                list.Add(ButtonBar.Action.Talk);
            }
            if (lockedAUX == true)
            {
                list.Remove(ButtonBar.Action.AUX);
            }

            list.Add(ButtonBar.Action.Items);
            list.Add(ButtonBar.Action.Guard);
            if (showRun)
            {
                list.Add(ButtonBar.Action.Run);
            }
            return list.ToArray();
        }
    }
}
