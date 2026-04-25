namespace EchoForest.Core;

/// <summary>
/// In-memory <see cref="IDisplayServer"/> used in NUnit tests.
/// Records every call so tests can assert on applied values without the Godot runtime.
/// </summary>
public sealed class MockDisplayServer : IDisplayServer
{
    public WindowMode AppliedWindowMode { get; private set; } = WindowMode.Windowed;
    public bool AppliedVSync { get; private set; }
    public int AppliedFpsLimit { get; private set; }
    public int AppliedMonitorIndex { get; private set; }
    public float AppliedBrightness { get; private set; } = 100f;
    public float AppliedGamma { get; private set; } = 100f;

    /// <summary>Configurable screen count returned by <see cref="GetScreenCount"/>.</summary>
    public int ScreenCount { get; set; } = 1;

    public void ApplyWindowMode(WindowMode mode) => AppliedWindowMode = mode;
    public void ApplyVSync(bool enabled) => AppliedVSync = enabled;
    public void ApplyFpsLimit(int fps) => AppliedFpsLimit = fps;
    public void ApplyMonitor(int index) => AppliedMonitorIndex = index;
    public void ApplyBrightness(float value) => AppliedBrightness = value;
    public void ApplyGamma(float value) => AppliedGamma = value;
    public int GetScreenCount() => ScreenCount;
}
