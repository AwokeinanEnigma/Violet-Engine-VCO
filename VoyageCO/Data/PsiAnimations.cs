using System;
using System.Collections.Generic;
using Violet.Audio;
using Violet.Graphics;
using Violet.Utility;
using VCO.Battle.AUXAnimation;
using VCO.Battle.UI;
using VCO.AUX;
using SFML.Graphics;
using SFML.System;

namespace VCO.Data
{
	internal class AUXAnimations
	{
		private static AUXElementList GenerateFreezeAlpha()
		{
			List<AUXElement> elements = new List<AUXElement>
			{
				new AUXElement
				{
					Timestamp = 0,
					Animation = new MultipartAnimation(Paths.AUX_GRAPHICS + "freeze_a.sdat", default(Vector2f), 0.5f, 32767),
					Sound = AudioManager.Instance.Use(Paths.SFXBATTLEAUX + "pkFreezeA.wav", AudioType.Sound),
					LockToTargetPosition = true,
					PositionIndex = 0,
					ScreenDarkenColor = new Color?(new Color(0, 0, 0, 128))
				},
				new AUXElement
				{
					Timestamp = 20,
					TargetFlashColor = new Color?(Color.Cyan),
					TargetFlashBlendMode = ColorBlendMode.Screen,
					TargetFlashFrames = 10,
					TargetFlashCount = 1
				},
				new AUXElement
				{
					Timestamp = 50,
					ScreenDarkenColor = new Color?(new Color(0, 0, 0, 0))
				}
			};
			return new AUXElementList(elements);
		}

		private static AUXElementList GenerateBeamAlpha()
		{
			MultipartAnimation animation = new MultipartAnimation(Paths.AUX_GRAPHICS + "beam1.sdat", default(Vector2f), 0.5f, 32767);
			MultipartAnimation multipartAnimation = new MultipartAnimation(Paths.AUX_GRAPHICS + "beam1.sdat", default(Vector2f), 0.5f, 32767);
			multipartAnimation.Scale = new Vector2f(-1f, 1f);
			List<AUXElement> list = new List<AUXElement>();
			list.Add(new AUXElement
			{
				Timestamp = 0,
				Animation = new MultipartAnimation(Paths.AUX_GRAPHICS + "beam2.sdat", new Vector2f(160f, 90f), 0.3f, 32767),
				ScreenDarkenColor = new Color?(new Color(0, 0, 0, 128)),
				Sound = AudioManager.Instance.Use(Paths.SFXBATTLEAUX + "pkBeamA.wav", AudioType.Sound)
			});
			list.Add(new AUXElement
			{
				Timestamp = 50,
				Animation = animation,
				Offset = new Vector2f(-52f, -48f),
				LockToTargetPosition = true,
				PositionIndex = 0
			});
			list.Add(new AUXElement
			{
				Timestamp = 50,
				Animation = multipartAnimation,
				Offset = new Vector2f(52f, -48f),
				LockToTargetPosition = true,
				PositionIndex = 0
			});
			list.Add(new AUXElement
			{
				Timestamp = 80,
				TargetFlashColor = new Color?(Color.Yellow),
				TargetFlashBlendMode = ColorBlendMode.Screen,
				TargetFlashFrames = 20,
				TargetFlashCount = 1
			});
			for (int i = 0; i < 6; i++)
			{
				list.Add(new AUXElement
				{
					Timestamp = 80 + i * 5,
					Animation = new MultipartAnimation(Paths.AUX_GRAPHICS + "beam3.sdat", default(Vector2f), 0.5f, 32767),
					Offset = new Vector2f((float)(i * -8), 0f),
					LockToTargetPosition = true,
					PositionIndex = 0
				});
				list.Add(new AUXElement
				{
					Timestamp = 80 + i * 5,
					Animation = new MultipartAnimation(Paths.AUX_GRAPHICS + "beam3.sdat", default(Vector2f), 0.5f, 32767),
					Offset = new Vector2f((float)(i * 8), 0f),
					LockToTargetPosition = true,
					PositionIndex = 0
				});
			}
			list.Add(new AUXElement
			{
				Timestamp = list[list.Count - 1].Timestamp + 30,
				ScreenDarkenColor = new Color?(new Color(0, 0, 0, 0))
			});
			return new AUXElementList(list);
		}

		private static AUXElementList GenerateHitback()
		{
			Vector2f position = new Vector2f(160f, 90f);
			return new AUXElementList(new List<AUXElement>
			{
				new AUXElement
				{
					Timestamp = 0,
					Animation = new MultipartAnimation(Paths.AUX_GRAPHICS + "comet_reflect.sdat", position, 0.5f, 32767),
					Sound = AudioManager.Instance.Use(Paths.SFXBATTLE + "rocketReflect.wav", AudioType.Sound),
					CardSpringMode = BattleCard.SpringMode.BounceUp,
					CardSpringAmplitude = new Vector2f(0f, 4f),
					CardSpringSpeed = new Vector2f(0f, 0.2f),
					CardSpringDecay = new Vector2f(0f, 0.5f)
				}
			});
		}

		private static List<AUXElement> GenerateThrow()
		{
			Vector2f position = new Vector2f(160f, 90f);
			return new List<AUXElement>
			{
				new AUXElement
				{
					Timestamp = 0,
					Animation = new MultipartAnimation(Paths.AUX_GRAPHICS + "comet.sdat", position, 0.5f, 32767),
					Sound = AudioManager.Instance.Use(Paths.SFXBATTLE + "rocket.wav", AudioType.Sound)
				}
			};
		}

		private static List<AUXElement> GenerateExplosion(int startTimestamp)
		{
			Vector2f v = new Vector2f(160f, 90f);
			List<AUXElement> list = new List<AUXElement>();
			list.Add(new AUXElement
			{
				Timestamp = startTimestamp,
				ScreenDarkenColor = new Color?(Color.Cyan),
				ScreenDarkenDepth = new int?(0),
				Sound = AudioManager.Instance.Use(Paths.SFXBATTLE + "explosion.wav", AudioType.Sound)
			});
			int num = 98;
			int[] array = new int[]
			{
				1,
				2,
				3,
				2,
				1
			};
			int num2 = 180 / (array.Length + 1);
			for (int i = 0; i < array.Length; i++)
			{
				int num3 = array[i];
				int num4 = (num3 - 1) * (num + 20);
				int num5 = num4 / 2;
				for (int j = 0; j < num3; j++)
				{
					Vector2f vector2f = v + new Vector2f((float)(-(float)num5 + (num + 20) * j), -v.Y + (float)(num2 * (i + 1)));
					int num6 = (int)(VectorMath.Magnitude(v - vector2f) / 10f);
					list.Add(new AUXElement
					{
						Timestamp = startTimestamp + num6,
						Animation = new MultipartAnimation(Paths.AUX_GRAPHICS + "comet_boom.sdat", vector2f, 0.4f, 32767)
					});
				}
			}
			list.Add(new AUXElement
			{
				Timestamp = list[list.Count - 1].Timestamp + 10,
				ScreenDarkenColor = new Color?(new Color(0, 0, 0, 0)),
				ScreenDarkenDepth = new int?(0)
			});
			return list;
		}

		private static AUXElementList GenerateComet()
		{
			List<AUXElement> list = new List<AUXElement>();
			list.AddRange(AUXAnimations.GenerateThrow());
			list.AddRange(AUXAnimations.GenerateExplosion(40));
			return new AUXElementList(list);
		}

		private static AUXElementList GenerateFireAlpha()
		{
			List<AUXElement> elements = new List<AUXElement>
			{
				new AUXElement
				{
					Timestamp = 0,
					Animation = new MultipartAnimation(Paths.AUX_GRAPHICS + "fire_a.sdat", new Vector2f(160f, 90f), 0.4f, 32767),
					Sound = AudioManager.Instance.Use(Paths.SFXBATTLEAUX + "pkFireA.wav", AudioType.Sound),
					ScreenDarkenColor = new Color?(new Color(0, 0, 0, 128))
				},
				new AUXElement
				{
					Timestamp = 40,
					TargetFlashColor = new Color?(Color.Red),
					TargetFlashBlendMode = ColorBlendMode.Screen,
					TargetFlashFrames = 10,
					TargetFlashCount = 1
				},
				new AUXElement
				{
					Timestamp = 80,
					ScreenDarkenColor = new Color?(new Color(0, 0, 0, 0))
				}
			};
			return new AUXElementList(elements);
		}




		public static AUXElementList Get(int AUXId)
		{
			switch (AUXId)
			{
			case 1:
				return AUXAnimations.GenerateFreezeAlpha();
			case 2:
				return AUXAnimations.GenerateBeamAlpha();
			case 3:
				return AUXAnimations.GenerateComet();
			case 4:
				return new AUXElementList(AUXAnimations.GenerateThrow());
			case 5:
				return AUXAnimations.GenerateHitback();
			case 6:
				return new AUXElementList(AUXAnimations.GenerateExplosion(0));
			case 7:
				return AUXAnimations.GenerateFireAlpha();
			default:
				return AUXAnimations.GenerateHitback();
			}
		}

		public static AUXElementList Get(IAUX AUX)
		{
			return Get(1);
			//return AUX;
		}
	}
}
