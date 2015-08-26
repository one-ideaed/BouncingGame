using System;
using CocosSharp;

namespace BouncingGame
{
	public class GameAppDelegate : CCApplicationDelegate
	{
		public override void ApplicationDidFinishLaunching (CCApplication application, CCWindow mainWindow)
		{
			application.PreferMultiSampling = false;
			application.ContentRootDirectory = "Content";
			application.ContentSearchPaths.Add ("animations");
			application.ContentSearchPaths.Add ("fonts");
			application.ContentSearchPaths.Add ("sounds");
	
			CCSize winSize = mainWindow.WindowSizeInPixels;
			CCScene.SetDefaultDesignResolution (winSize.Width, winSize.Height, CCSceneResolutionPolicy.ShowAll);
            
			CCScene scene = new CCScene (mainWindow);

			BouncingGame.GameLayer gameLayer = new BouncingGame.GameLayer (winSize,CCClipMode.None);

			scene.AddChild (gameLayer);

			mainWindow.RunWithScene (scene);
		}

		public override void ApplicationDidEnterBackground (CCApplication application)
		{
			application.Paused = true;
		}

		public override void ApplicationWillEnterForeground (CCApplication application)
		{
			application.Paused = false;
		}
	}
}
