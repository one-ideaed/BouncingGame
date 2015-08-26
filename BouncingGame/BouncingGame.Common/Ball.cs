using System;
using CocosSharp;

namespace BouncingGame.Common
{
	public class Ball: CCSprite
	{
		private const string BALL_IMAGE = "images/bigball";
		private float velocityX;
		private float velocityY;

		public Ball () : base(BALL_IMAGE)
		{
		}

		public void Move(float frameTimeInSeconds)
		{
			//Regular movement
	//		velocityY += frameTimeInSeconds * -140;
			PositionX += velocityX * frameTimeInSeconds;
			PositionY += velocityY * frameTimeInSeconds;

			//Try bouce from screen edge
			bool isCrashRight = BoundingBoxTransformedToParent.MaxX > VisibleBoundsWorldspace.MaxX && velocityX > 0;
			bool isCrashLeft = BoundingBoxTransformedToParent.MinX < VisibleBoundsWorldspace.MinX&& velocityX < 0; 
			if (isCrashRight || isCrashLeft)
			{
				velocityX*= -1;
				StartRotation ();
			}

		}

		public float VelocityX {
			get {
				return velocityX;
			}
		}

		public bool IsOutOfPitchBottom()
		{
			return BoundingBoxTransformedToParent.MaxY < VisibleBoundsWorldspace.MinY
				? true : false;
		}

		public bool IsOutOfPitchTop()
		{
			return BoundingBoxTransformedToParent.MinY > VisibleBoundsWorldspace.MaxY
				? true : false;
		}

		public float VelocityY {
			get {
				return velocityY;
			}
		}

		public void VerticalBounce()
		{
			velocityY *= -1;
			velocityX = CCRandom.GetRandomFloat (-getMaxSpeedX(), getMaxSpeedX());
			StartRotation ();
		}

		public void StartRotation()
		{
			float angle = (float)(360 / Math.PI * Math.Atan (VelocityX / VelocityY));
			angle += VelocityY < 0 ? 180 : 0;
			Random random = new Random ();
			float duration= (float)random.NextDouble()/2;
			CCRotateTo action = new CCRotateTo (duration, angle);
			AddAction (action);
		}

		private float getMaxSpeedX()
		{
			//need 1.25 second to pass whole pitch horizontally
			return (VisibleBoundsWorldspace.MaxX - VisibleBoundsWorldspace.MinX - ContentSize.Width)/(float)1.25;
		}

		private float getMaxSpeedY()
		{
			//need 1 second to pass whole pitch vertically
			return VisibleBoundsWorldspace.MaxY - VisibleBoundsWorldspace.MinY - ContentSize.Height;
		}

		public void InitialKick()
		{
			Random r = new Random ();
			velocityX = 0;
			velocityY = r.NextDouble() > 0.5 ? getMaxSpeedY() : -getMaxSpeedY();
		    StartRotation ();
		}
	}
}

