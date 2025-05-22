using ImagoCrafter.Core;
using ImagoCrafter.Processing.Kernels;

namespace ImagoCrafter.Processing.Filters;

public class UnsharpMaskProcessor : IImageProcessor
{
    private float _strength;
    private float _radius;
    private float _threshold;

    public UnsharpMaskProcessor(float strength = 0.5f, float radius = 1.0f, float threshold = 0.0f)
    {
        _strength = strength;
        _radius = radius;
        _threshold = threshold;
    }

    public Image Process(Image input)
    {
        // Create a blurred version using Gaussian blur
        var blurProcessor = new GaussianBlurProcessor(_radius);
        var blurred = blurProcessor.Process(input);

        // Create output image
        var output = new Image(input.Width, input.Height, input.Channels);

        // Apply unsharp mask formula: Sharp = Original + (Original - Blur) * amount
        Parallel.For(0, input.Height, y =>
        {
            for (int x = 0; x < input.Width; x++)
            {
                for (int c = 0; c < input.Channels; c++)
                {
                    float original = input.GetPixelComponentF(x, y, c);
                    float blur = blurred.GetPixelComponentF(x, y, c);
                    float difference = original - blur;

                    // Apply threshold to prevent sharpening noise
                    if (Math.Abs(difference) > _threshold)
                    {
                        float sharpened = original + (difference * _strength);
                        output.SetPixelComponentF(x, y, c, Math.Clamp(sharpened, 0.0f, 1.0f));
                    }
                    else
                    {
                        output.SetPixelComponentF(x, y, c, original);
                    }
                }
            }
        });

        return output;
    }

    public void Configure(Dictionary<string, object> parameters)
    {
        if (parameters.TryGetValue("strength", out var s) && s is float strength)
            _strength = strength;
        if (parameters.TryGetValue("radius", out var r) && r is float radius)
            _radius = radius;
        if (parameters.TryGetValue("threshold", out var t) && t is float threshold)
            _threshold = threshold;
    }
}