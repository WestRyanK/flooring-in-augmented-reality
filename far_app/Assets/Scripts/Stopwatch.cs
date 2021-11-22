using System;

public class Stopwatch : IDisposable
{
    public float Duration => _stopwatch.ElapsedMilliseconds;
    private System.Diagnostics.Stopwatch _stopwatch;

    public Stopwatch()
    {
        _stopwatch = new System.Diagnostics.Stopwatch();
        _stopwatch.Start();
    }

    public void Dispose()
    {
        _stopwatch.Stop();
    }
}