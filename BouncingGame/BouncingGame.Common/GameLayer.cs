using System;
using System.Collections.Generic;
using CocosSharp;
using BouncingGame.Common;


namespace BouncingGame
{
	public class GameLayer : CCLayer
	{
		private CCSprite background;
		private Platform playerPlatform;
		private Platform enemyPlatform;
		private Ball ball;
		private CCSprite playerMarking;
		private CCSprite enemyMarking;
		private CCLabel scoreLabel;
		//private CCSprite logo1;
		//private CCSprite logo2;

		private CCSize screenSize;

		private int playerScore;
		private int enemyScore;
		private int ticks;
		private States state = States.Preparation;
		//private int GAME_DURATION = 60;
		private int FINISH_SCORE = 5;

		enum States {Preparation, Action, Results};
	
		public GameLayer (CCSize screenSize, CCClipMode clipmode) : base (screenSize, clipmode)
		{
			this.screenSize = screenSize;
			Init ();
			Schedule (RunGameLogic);
		}

		void Init()
		{
			CCRect r = new CCRect (0,0, screenSize.Width,screenSize.Height);
			background = new CCSprite("images/bggrass", r);
			background.MaximizeTextureRect ();
			playerMarking = new CCSprite ("images/bgmarking");
		    enemyMarking = new CCSprite ("images/bgmarking");
			//logo1 = new CCSprite ("images/logopong");
			//logo2 = new CCSprite ("images/logoganymede");
			playerPlatform = new Platform (true);
			enemyPlatform = new Platform (false);
			ball = new Ball();
			string scoreText = playerScore.ToString () + " : " + enemyScore.ToString (); 
			scoreLabel = new CCLabel( scoreText, "fonts/arial", 100);

			Layout();

			AddChild (background);
			AddChild (playerMarking);
			AddChild (enemyMarking);
			//AddChild (logo1);
			//AddChild (logo2);
			AddChild (scoreLabel);
			AddChild (playerPlatform);
			AddChild (enemyPlatform);
			AddChild (ball);
		}

		private void Layout()
		{
			background.PositionX = screenSize.Center.X;
			background.PositionY = screenSize.Center.Y;

			playerMarking.Opacity = 100;
			playerMarking.PositionX = screenSize.Center.X;
			playerMarking.PositionY = playerMarking.ContentSize.Height/2;

			enemyMarking.FlipY = true;
			enemyMarking.Opacity = 100;
			enemyMarking.PositionX = screenSize.Center.X;
			enemyMarking.PositionY = screenSize.Height - enemyMarking.ContentSize.Height/2;
			/*
			logo1.PositionX = logo1.ContentSize.Center.X;
			logo1.PositionY = logo1.ContentSize.Center.Y;

			logo2.PositionX = screenSize.Width - logo2.ContentSize.Center.X;
			logo2.PositionY = screenSize.Height - logo2.ContentSize.Center.Y;
			*/
			playerPlatform.PositionX = screenSize.Center.X;
			playerPlatform.PositionY = screenSize.Height / 15;

			scoreLabel.Opacity = 180;
			scoreLabel.PositionX = screenSize.Center.X;
			scoreLabel.PositionY = screenSize.Center.Y;
			scoreLabel.AnchorPoint = CCPoint.AnchorMiddle;

			LayoutActiveObjects();
		}

		private void LayoutActiveObjects()
		{
			enemyPlatform.PositionX = screenSize.Center.X;
			enemyPlatform.PositionY = screenSize.Height - screenSize.Height / 15;

			ball.PositionX = screenSize.Center.X;
			ball.PositionY = screenSize.Center.Y;
		}

		void RunGameLogic(float frameTimeInSeconds)
		{
			ticks++;;
			switch (state) {
			case States.Preparation:
				//take rest for 1 sec
				if (ticks > 1/frameTimeInSeconds) {
					state = States.Action;
					ball.InitialKick ();
				}
				break;
			case States.Action:
				ball.Move (frameTimeInSeconds);
				enemyPlatform.Move (ball, frameTimeInSeconds);

				if (enemyPlatform.IsCollision (ball) || playerPlatform.IsCollision (ball)) {
					ball.VerticalBounce ();
				}

				bool outTop = ball.IsOutOfPitchTop ();
				bool outBottom = ball.IsOutOfPitchBottom ();
				if (outTop || outBottom) {
					ticks = 0;
					state = States.Preparation;

					LayoutActiveObjects ();
					ball.StopAllActions ();
					ball.Rotation = 0;

					playerScore += outTop ? 1 : 0;
					enemyScore += outBottom ? 1 : 0;

					if (playerScore >= FINISH_SCORE || enemyScore >= FINISH_SCORE) {
						state = States.Results;
						ball.Visible = false;
						AnnounceGameResults ();
					} else {
						UpdateScoreLabel ();
					}
				}
				break;

			case States.Results:
				// show result for 5 secs
				if (ticks > 5/frameTimeInSeconds) {
					state = States.Preparation;
					ticks = 0;
					playerScore = 0;
					enemyScore = 0;
					ball.Visible = true;
					UpdateScoreLabel ();
				}
				break;
			}
		}

		private void AnnounceGameResults()
		{
			if (playerScore > enemyScore)
				scoreLabel.Text = "Golden medal!";
			else if (playerScore < enemyScore)
				scoreLabel.Text = "Silver medal!";
			else
				//impossible case for today
				scoreLabel.Text = "Friendship won!";
		}

		private void UpdateScoreLabel()
		{
			scoreLabel.Text = playerScore.ToString () + " : " + enemyScore.ToString ();
		}

		protected override void AddedToScene ()
		{
			base.AddedToScene ();
			// Register for touch events
			var touchListener = new CCEventListenerTouchAllAtOnce ();
			touchListener.OnTouchesEnded = OnTouchesEnded;

			touchListener.OnTouchesMoved = HandleTouchesMoved;
			AddEventListener (touchListener, this);
		}

		void HandleTouchesMoved (System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{
			//Unfortunatelly framework isn't ready to reflect movement correctly in case we reassing Action each tick
			if (ticks%2==0 )return;

			// we only care about the first touch:
			var locationOnScreen = touches [0].Location;

			CCPoint point = new CCPoint ();
			point.X = locationOnScreen.X;
			point.Y = Math.Min (locationOnScreen.Y, playerMarking.BoundingBoxTransformedToParent.MaxY);
			float duration = playerPlatform.TimeToGet (point);
			CCMoveTo action = new CCMoveTo (duration,point);
			playerPlatform.StopAllActions ();
			playerPlatform.AddAction (action);
		}

		void OnTouchesEnded (List<CCTouch> touches, CCEvent touchEvent)
		{
			if (touches.Count > 0)
			{
				//TODO
			}
		}
	}
}