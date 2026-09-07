namespace EchoForest.Core;

/// <summary>Engine-neutral RGBA color used by pure cutscene steps.</summary>
public readonly record struct CutsceneColor(float Red, float Green, float Blue, float Alpha)
{
    public static CutsceneColor Clear => new(0f, 0f, 0f, 0f);
    public static CutsceneColor Black => new(0f, 0f, 0f, 1f);
    public static CutsceneColor White => new(1f, 1f, 1f, 1f);
}

/// <summary>Engine-neutral 2D world position used by pure cutscene steps.</summary>
public readonly record struct CutscenePosition(float X, float Y);