using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VCO.Data;
using Violet.GUI;

namespace VCO.Scripts.Text
{
    internal class TextProcessor
    {
        private const char NEWLINE_CHAR = '\n';
        private const char BULLET_CHAR = '@';
        private const char SPACE_CHAR = ' ';
        private const int PAUSE_CHAR_DURATION = 10;
        private const int BULLET_PAUSE_DURATION = 30;
        private const string ENCLOSING_CMD_REGEX = "\\[([a-zA-Z][a-zA-Z0-9]*):?(\\b[^\\]]*)\\](.*?)\\[\\/\\1\\]";
        private const string SINGLE_CMD_REGEX = "\\[([a-zA-Z][a-zA-Z0-9]*):?(\\b[^\\]]*)\\]";
        private const string CMD_PAUSE = "p";
        private const string CMD_CHARNAME = "cn";
        private const string CMD_TRIGGER = "t";
        private const string CMD_TRAVIS = "travis";
        private const string CMD_FLOYD = "floyd";
        private const string CMD_MERYL = "meryl";
        private const string CMD_LEO = "leo";
        private const string CMD_ZACK = "zack";
        private const string CMD_RENEE = "renee";
        private static readonly char[] PAUSE_CHARS = new char[2]
        {
      ',',
      '?'
        };

        public static TextBlock Process(FontData font, string text, int frameWidth)
        {
            List<string> stringList = new List<string>();
            Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
            List<ITextCommand> commands = new List<ITextCommand>();

            StringBuilder stringBuilder = new StringBuilder(text ?? "").Replace("\r", "");
            MatchCollection matchCollection = Regex.Matches(stringBuilder.ToString(), "\\[([a-zA-Z][a-zA-Z0-9]*):?(\\b[^\\]]*)\\](.*?)\\[\\/\\1\\]");
 
            int matchIndex = 0;

            foreach (Match match in matchCollection)
            {
                string preMatch = match.Groups[1].Value;
                string midMatch = match.Groups[2].Value;
                string postMatch = match.Groups[3].Value;
                
                stringBuilder = stringBuilder.Remove(match.Index, match.Length);
                stringBuilder = stringBuilder.Insert(match.Index, postMatch);
            }
            foreach (Match match in Regex.Matches(stringBuilder.ToString(), "\\[([a-zA-Z][a-zA-Z0-9]*):?(\\b[^\\]]*)\\]"))
            {
                string firstMatch = match.Groups[1].Value;
                string secondMatch = match.Groups[2].Value;
                
                string[] sourceArray = secondMatch.Split(',');
                
                for (int index = 0; index < sourceArray.Length; ++index) { 
                    sourceArray[index] = sourceArray[index].Trim();
                }

                int beforeMatchIndex = match.Index - matchIndex;
                
                TextProcessor.OffsetCommandPositions(commands, beforeMatchIndex, match.Length);
                stringBuilder = stringBuilder.Remove(beforeMatchIndex, match.Length);

                matchIndex += match.Length;
                
                switch (firstMatch)
                {
                    // pause 
                    case "p":
                        int pauseCommandDuration;
                        int.TryParse(secondMatch, out pauseCommandDuration);
                        commands.Add(new TextPause(beforeMatchIndex, pauseCommandDuration));
                        continue;

                    // character name
                    case "cn":
                        CharacterType result2;
                        Enum.TryParse<CharacterType>(secondMatch, out result2);
                        string name1 = CharacterNames.GetName(result2);
                        stringBuilder = stringBuilder.Insert(beforeMatchIndex, name1);
                        matchIndex -= name1.Length;
                        continue;

                    // text trigger
                    case "t":
                        int triggerResult;
                        int.TryParse(sourceArray[0], out triggerResult);
                        string[] strArray = new string[sourceArray.Length - 1];
                        Array.Copy(sourceArray, 1, strArray, 0, strArray.Length);
                        commands.Add(new TextTrigger(beforeMatchIndex, triggerResult, strArray));
                        continue;

                    // oddity character names
                    // todo: remove later
                    case "travis":
                        string travis = CharacterNames.GetName(CharacterType.Travis);
                        stringBuilder = stringBuilder.Insert(beforeMatchIndex, travis);
                        matchIndex -= travis.Length;
                        continue;
                    case "floyd":
                        string floyd = CharacterNames.GetName(CharacterType.Floyd);
                        stringBuilder = stringBuilder.Insert(beforeMatchIndex, floyd);
                        matchIndex -= floyd.Length;
                        continue;
                    case "meryl":
                        string meryl = CharacterNames.GetName(CharacterType.Meryl);
                        stringBuilder = stringBuilder.Insert(beforeMatchIndex, meryl);
                        matchIndex -= meryl.Length;
                        continue;
                    case "leo":
                        string name5 = CharacterNames.GetName(CharacterType.Leo);
                        stringBuilder = stringBuilder.Insert(beforeMatchIndex, name5);
                        matchIndex -= name5.Length;
                        continue;
                    case "zack":
                        string name6 = CharacterNames.GetName(CharacterType.Zack);
                        stringBuilder = stringBuilder.Insert(beforeMatchIndex, name6);
                        matchIndex -= name6.Length;
                        continue;
                    case "renee":
                        string name7 = CharacterNames.GetName(CharacterType.Renee);
                        stringBuilder = stringBuilder.Insert(beforeMatchIndex, name7);
                        matchIndex -= name7.Length;
                        continue;
                    default:
                        continue;
                }
            }
            SFML.Graphics.Text text1 = new SFML.Graphics.Text(stringBuilder.ToString().Replace('@'.ToString(), string.Empty).Replace('\n'.ToString(), string.Empty), font.Font, font.Size);
            float num3 = 0.0f;
            float num4 = text1.FindCharacterPos(0U).X;
            int num5 = 0;
            int startIndex = 0;
            int num6 = 0;
            bool flag1 = false;
            bool flag2 = false;
            for (int index = 0; index < stringBuilder.Length; ++index)
            {
                char ch = stringBuilder[index];
                if (((IEnumerable<char>)TextProcessor.PAUSE_CHARS).Contains<char>(ch))
                {
                    commands.Add(new TextPause(index + 1, 10));
                }
                else
                {
                    switch (ch)
                    {
                        case '\n':
                            num6 = index;
                            stringBuilder = stringBuilder.Remove(index, 1);
                            TextProcessor.OffsetCommandPositions(commands, index, -1);
                            --index;
                            flag1 = true;
                            break;
                        case ' ':
                            num5 = index;
                            break;
                        case '@':
                            num6 = index;
                            stringBuilder = stringBuilder.Remove(index, 1);
                            TextProcessor.OffsetCommandPositions(commands, index, -1);
                            --index;
                            flag1 = index > startIndex;
                            if (flag2)
                                commands.Add(new TextWait(index));
                            dictionary.Add(stringList.Count + (flag1 ? 1 : 0), true);
                            flag2 = true;
                            continue;
                    }
                }
                float x = text1.FindCharacterPos((uint)index).X;
                float num7 = x - num4;
                num3 += num7;
                num4 = x;
                if (num3 > (double)frameWidth)
                {
                    num6 = num5;
                    flag1 = true;
                }
                if (flag1)
                {
                    string str = stringBuilder.ToString(startIndex, num6 - startIndex);
                    stringList.Add(str);
                    num3 = 0.0f;
                    int num8 = 0;
                    int length = stringBuilder.Length;
                    while (num6 + num8 < length && stringBuilder[num6 + num8] == ' ')
                        ++num8;
                    startIndex = num6 + num8;
                    flag1 = false;
                }
            }
            string str4 = stringBuilder.ToString(startIndex, stringBuilder.Length - startIndex);
            if (str4.Length > 0)
                stringList.Add(str4);
            List<TextLine> lines = new List<TextLine>();
            int startPosition = 0;
            for (int index = 0; index < stringList.Count; ++index)
            {
                bool bullet = false;
                if (dictionary.ContainsKey(index))
                    bullet = dictionary[index];
                string text2 = stringList[index];
                lines.Add(new TextLine(bullet, TextProcessor.GetCommandRange(commands, startPosition, startPosition + text2.Length), text2));
                startPosition += text2.Length;
            }
            return new TextBlock(lines);
        }

        private static ITextCommand[] GetCommandRange(
          List<ITextCommand> commands,
          int startPosition,
          int endPosition)
        {
            List<ITextCommand> textCommandList = new List<ITextCommand>();
            foreach (ITextCommand command in commands)
            {
                if (command.Position >= startPosition && command.Position < endPosition)
                    textCommandList.Add(command);
            }
            return textCommandList.ToArray();
        }

        private static void OffsetCommandPositions(
          List<ITextCommand> commands,
          int afterPosition,
          int offset)
        {
            foreach (ITextCommand command in commands)
            {
                if (command.Position > afterPosition)
                    command.Position += offset;
            }
        }
    }
}
