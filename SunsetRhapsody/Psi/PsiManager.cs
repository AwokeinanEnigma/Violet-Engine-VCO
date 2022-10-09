using System;
using System.Collections.Generic;
using System.Linq;
using fNbt;
using SunsetRhapsody.Data;
using Rufini.Strings;

namespace SunsetRhapsody.AUX
{
	public sealed class AUXManager
	{
		public static AUXManager Instance
		{
			get
			{
				if (AUXManager.instance == null)
				{
					AUXManager.instance = new AUXManager();
				}
				return AUXManager.instance;
			}
		}

		private AUXManager()
		{
			NbtFile nbtFile = new NbtFile(AUXManager.AUX_FILE);
			NbtCompound rootTag = nbtFile.RootTag;
			this.offensive = this.InitializeAUXList<OffenseAUX>();
			this.defensive = this.InitializeAUXList<DefensiveAUX>();
			this.assistive = this.InitializeAUXList<AssistiveAUX>();
			this.other = this.InitializeAUXList<OtherAUX>();
			this.LoadOffenseAUX(rootTag.Get<NbtCompound>("offense"));
			Console.WriteLine("it loaded??");
			this.LoadDefenseAUX(rootTag.Get<NbtCompound>("defense"));
			Console.WriteLine("it loaded??");
			this.LoadAssistAUX(rootTag.Get<NbtCompound>("assist"));
			Console.WriteLine("it loaded??");
			this.LoadOtherAUX(rootTag.Get<NbtCompound>("other"));
			Console.WriteLine("it loaded??");
		}

		private Dictionary<CharacterType, List<T>> InitializeAUXList<T>() where T : IAUX
		{
			return new Dictionary<CharacterType, List<T>>
			{
				{
					CharacterType.Floyd,
					new List<T>()
				},
				{
					CharacterType.Leo,
					new List<T>()
				},
				{
					CharacterType.Meryl,
					new List<T>()
				},
				{
					CharacterType.Travis,
					new List<T>()
				},
				{
					CharacterType.Zack,
					new List<T>()
				},
				{
					CharacterType.Renee,
					new List<T>()
				}
			};
		}

		internal bool CharacterCanUseAUXType(CharacterType playerCharacter, AUXType AUXType)
		{
			switch (AUXType)
			{
			case AUXType.Offense:
				return this.offensive.ContainsKey(playerCharacter);
			case AUXType.Defense:
				return this.defensive.ContainsKey(playerCharacter);
			case AUXType.Assist:
				return this.assistive.ContainsKey(playerCharacter);
			case AUXType.Other:
				return this.other.ContainsKey(playerCharacter);
			default:
				Console.WriteLine("AUX Type {0} is not supported", AUXType);
				throw new NotSupportedException();
			}
		}

		private void LoadOffenseAUX(NbtCompound offenseTag)
		{
			if (offenseTag != null)
			{

				Console.WriteLine("huh1");
				//OffenseAUX AUX = new OffenseAUX(new SOMETHING.Debug());
				OffenseAUX AUX1 = new OffenseAUX(new SOMETHING.Beam());
				OffenseAUX AUX2 = new OffenseAUX(new SOMETHING.Fire());
				OffenseAUX AUX3 = new OffenseAUX(new SOMETHING.Freeze());
				OffenseAUX AUX4 = new OffenseAUX(new SOMETHING.Test());
				Console.WriteLine($"Offensive8 AUX: {AUX2.aux.QualifiedName} " + Environment.NewLine);
				//this.AddAUXToCharacters<OffenseAUX>(this.offensive, AUX, new List<string>() { "meryl", "travis", "leo" });
				this.AddAUXToCharacters<OffenseAUX>(this.offensive, AUX1, new List<string>() { "sean", "travis", "leo" });
				this.AddAUXToCharacters<OffenseAUX>(this.offensive, AUX2, new List<string>() { "sean", "travis", "leo" });
				this.AddAUXToCharacters<OffenseAUX>(this.offensive, AUX3, new List<string>() { "sean", "travis", "leo" });
				this.AddAUXToCharacters<OffenseAUX>(this.offensive, AUX4, new List<string>() { "sean", "travis", "leo" });
				/*
				using (IEnumerator<NbtTag> enumerator = offenseTag.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						/*
						NbtTag nbtTag = enumerator.Current;
						NbtCompound nbtCompound = (NbtCompound)nbtTag;
						string stringValue = nbtCompound.Get<NbtString>("key").StringValue;
						NbtIntArray[] source = nbtCompound.Get<NbtList>("effect").ToArray<NbtIntArray>();
						int[][] effect = (from s in source
						select s.Value).ToArray<int[]>();

					}
					return;
				}*/
			}
			Console.WriteLine("Could not load OffenseAUX. Is the offense tag present?");
		}

		private void LoadDefenseAUX(NbtCompound defenseTag)
		{
			if (defenseTag != null)
			{
				Console.WriteLine("huh1");
				//OffenseAUX AUX = new OffenseAUX(new SOMETHING.Debug());
				//Console.WriteLine($"Offensive8 AUX: {AUX.aux.QualifiedName} " + Environment.NewLine);
			}
			Console.WriteLine("Could not load DefenseAUX. Is the defense tag present?");
		}

		private void LoadAssistAUX(NbtCompound assistTag)
		{
			if (assistTag != null)
			{
				AssistiveAUX AUX3 = new AssistiveAUX(new SOMETHING.LifeUp());
				Console.WriteLine($"Assistive AUX: {AUX3.aux.QualifiedName} " + Environment.NewLine);
				this.AddAUXToCharacters<AssistiveAUX>(this.assistive, AUX3, new List<string>() { "travis" });

				AssistiveAUX AUX2 = new AssistiveAUX(new SOMETHING.Telepathy());
				Console.WriteLine($"Assistive AUX: {AUX2.aux.QualifiedName} " + Environment.NewLine);
				this.AddAUXToCharacters<AssistiveAUX>(this.assistive, AUX2, new List<string>() { "travis", "sean", "leo" });
			}
			Console.WriteLine("Could not load AssistAUX. Is the assist tag present?");
		}

		private void LoadOtherAUX(NbtCompound otherTag)
		{
			Console.WriteLine("huh1");
			OtherAUX AUX = new OtherAUX(new SOMETHING.Debug());
			OtherAUX AUX1 = new OtherAUX(new SOMETHING.Sacrifice());
			//this.AddAUXToCharacters<OtherAUX>(this.other, AUX, new List<string>() { "travis", "Sean", "leo" });
			this.AddAUXToCharacters<OtherAUX>(this.other, AUX1, new List<string>() { "travis", "sean", "leo" });
			Console.WriteLine($"OtherAUX AUX: {AUX.aux.QualifiedName} " + Environment.NewLine);
			if (otherTag != null)
			{
			}
			Console.WriteLine("Could not load OtherAUX. Is the other tag present?");
		}

		private IEnumerable<string> LoadUsersAttribute(NbtCompound ability)
		{
			return from s in ability.Get<NbtList>("users").ToArray<NbtString>()
			select s.Value;
		}

		private int[] LoadPPAttribute(NbtCompound ability)
		{
			return ability.Get<NbtIntArray>("pp").Value;
		}

		private int[] LoadLevelsAttribute(NbtCompound ability)
		{
			return ability.Get<NbtIntArray>("levels").Value;
		}

		private float LoadSpecialAttribute(NbtCompound ability)
		{
			return ability.Get<NbtFloat>("special").Value;
		}

		private string LoadTargetAttribute(NbtCompound ability)
		{
			return ability.Get<NbtString>("target").Value;
		}

		private int LoadAnimationAttribute(NbtCompound ability)
		{
			return ability.Get<NbtInt>("anim").Value;
		}

		private void AddAUXToCharacters<T>(Dictionary<CharacterType, List<T>> dictionary, T AUX, IEnumerable<string> characters) where T : IAUX
		{
			foreach (string text in characters)
			{
				string a;
				if ((a = text) != null)
				{
					if (a == "floyd")
					{
						dictionary[CharacterType.Floyd].Add(AUX);
						continue;
					}
					if (a == "leo")
					{
						dictionary[CharacterType.Leo].Add(AUX);
						continue;
					}
					if (a == "sean")
					{
						dictionary[CharacterType.Meryl].Add(AUX);
						continue;
					}
					if (a == "travis")
					{
						dictionary[CharacterType.Travis].Add(AUX);
						continue;
					}
					if (a == "zack")
					{
						dictionary[CharacterType.Zack].Add(AUX);
						continue;
					}
				}
				Console.WriteLine("Tried to add AUX {0} to invalid character {1}", AUX.aux.QualifiedName, text);
			}
		}

		internal IEnumerable<OffenseAUX> GetCharacterOffenseAUX(CharacterType playerCharacter)
		{
			return this.offensive[playerCharacter];
		}

		internal IEnumerable<DefensiveAUX> GetCharacterDefenseAUX(CharacterType playerCharacter)
		{
			return this.defensive[playerCharacter];
		}

		internal IEnumerable<AssistiveAUX> GetCharacterAssistAUX(CharacterType playerCharacter)
		{
			return this.assistive[playerCharacter];
		}

		internal IEnumerable<OtherAUX> GetCharacterOtherAUX(CharacterType playerCharacter)
		{
			return this.other[playerCharacter];
		}

		internal bool CharacterHasAUX(CharacterType playerCharacter)
		{
            switch (playerCharacter)
            {
                case CharacterType.Travis:
                    return true;
                case CharacterType.Dog:
                    return false;
                case CharacterType.Leo:
                    return true;
                case CharacterType.Floyd:
                    return false;
                case CharacterType.Renee:
                    return true;
                case CharacterType.Zack:
                    return false;
                case CharacterType.Meryl:
                    return true;
					default:
                        return false;
            }

            /*bool flag = this.offensive[playerCharacter].Count > 0;
			bool flag2 = this.defensive[playerCharacter].Count > 0;
			bool flag3 = this.assistive[playerCharacter].Count > 0;
			bool flag4 = this.other[playerCharacter].Count > 0;*/
			//return flag || flag2 || flag3 || flag4;
		}

		private static AUXManager instance;

		private static readonly string AUX_FILE = Paths.AUXFILES + "psi.dat";

		private readonly Dictionary<CharacterType, List<OffenseAUX>> offensive;

		private readonly Dictionary<CharacterType, List<DefensiveAUX>> defensive;

		private readonly Dictionary<CharacterType, List<AssistiveAUX>> assistive;

		private readonly Dictionary<CharacterType, List<OtherAUX>> other;
	}
}
