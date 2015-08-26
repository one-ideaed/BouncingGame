using System;
using CocosSharp;

namespace BouncingGame.Common
{
	public class Platform : CCSprite
	{
		private const string PLAYER_PLATFORM = "images/platformblue";
		private const string ENEMY_PLATFORM = "images/platformred";

		private float velocityX;
		private float velocityY;

		private bool isPlayer;

		public Platform (bool isPlayer) : base(isPlayer ? PLAYER_PLATFORM: ENEMY_PLATFORM)
		{
			this.isPlayer = isPlayer;
		}

		public void Move(Ball ball, float frameTimeInSeconds)
		{
			if (isPlayer) return;

			float a = ball.PositionX - PositionX;
			velocityX = Math.Sign(a) * Math.Min (GetMaxSpeedX(), Math.Abs(a/frameTimeInSeconds));
			PositionX += velocityX * frameTimeInSeconds;

			a = VisibleBoundsWorldspace.MaxY - PositionY - ContentSize.Height;
			int directionY = -Math.Sign(ball.VelocityY);
			velocityY = Math.Min(directionY * GetMaxSpeedY(), a/frameTimeInSeconds);
			PositionY += velocityY * frameTimeInSeconds; 
		}
			
		public bool IsCollision(Ball ball)
		{
			return ball.BoundingBoxTransformedToParent.IntersectsRect (BoundingBoxTransformedToParent)
				&& Math.Sign(ball.VelocityY) == Math.Sign(PositionY-ball.PositionY);
		}

		private float GetMaxSpeedX()
		{
			//Enemy 2 seconds to pass screen horizontally. We need only 0.3 second :)
			float x = VisibleBoundsWorldspace.MaxX - VisibleBoundsWorldspace.MinX - ContentSize.Width;
			return (float)(isPlayer ? x/0.3 : x/2);
		}

		private float GetMaxSpeedY()
		{
			//Enemy 4 seconds to pass screen horizontally. Player has the same speed in both dimentions.
			float y = VisibleBoundsWorldspace.MaxX - VisibleBoundsWorldspace.MinX - ContentSize.Width;
			return isPlayer ? GetMaxSpeedX() : y/4;
		}

		public float TimeToGet(CCPoint point)
		{
			float durationX = Math.Abs(point.X-PositionX)/GetMaxSpeedX();
			float durationY = Math.Abs(point.Y-PositionY)/GetMaxSpeedY();
			return Math.Max (durationX, durationY);
		}
	}
}

