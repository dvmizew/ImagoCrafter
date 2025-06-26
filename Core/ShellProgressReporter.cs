namespace ImagoCrafter.Core;

using ShellProgressBar;

public class ShellProgressReporter : IProgressReporter, IDisposable
{
    private readonly ProgressBar _progressBar;
    private bool _disposed = false;

    public ShellProgressReporter(string initialMessage = "Processing...")
    {
        var options = new ProgressBarOptions
        {
            ProgressCharacter = '█',
            ProgressBarOnBottom = true,
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkGreen,
            CollapseWhenFinished = false
        };

        _progressBar = new ProgressBar(100, initialMessage, options);
    }

    public void Report(int percentage, string? message = null)
    {
        if (_disposed) return;
        
        percentage = Math.Clamp(percentage, 0, 100);
        _progressBar.Tick(percentage, message ?? "Processing...");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _progressBar?.Dispose();
            _disposed = true;
        }
    }
}
