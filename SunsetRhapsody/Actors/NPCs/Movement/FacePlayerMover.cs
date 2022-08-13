﻿using System;
using Violet.Graphics;
using Violet.Utility;
using SFML.System;

namespace SunsetRhapsody.Actors.NPCs.Movement
{
	internal class FacePlayerMover : Mover
	{
		public FacePlayerMover()
		{
			this.oldDirection = -1;
		}

		public override bool GetNextMove(ref Vector2f position, ref Vector2f velocity, ref int direction)
		{
			this.oldDirection = direction;
			if (ViewManager.Instance.FollowActor != null)
			{
				direction = VectorMath.VectorToDirection(ViewManager.Instance.FollowActor.Position - position);
			}
			this.changed = (this.oldDirection != direction);
			return this.changed;
		}

		private int oldDirection;

		private bool changed;
	}
}
