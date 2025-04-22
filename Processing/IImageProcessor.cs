namespace ImagoCrafter.Processing;

using ImagoCrafter.Core;

public interface IImageProcessor
{
    Image Process(Image input);
    void Configure(Dictionary<string, object> parameters);
}