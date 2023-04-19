using Sandbox;

public static class MyExtension
{
    [Event.Tick]
    public static void FrameHook()
    {
        DebugOverlay.ScreenText( "Extension Is Working!", Game.IsServer ? 1 : 4, 0.1f );
    }
}
