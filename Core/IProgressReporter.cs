namespace ImagoCrafter.Core;

public interface IProgressReporter
{
    void Report(int percentage, string? message = null);
}
