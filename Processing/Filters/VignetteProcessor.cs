using ImagoCrafter.Core;

namespace ImagoCrafter.Processing.Filters;

public class VignetteProcessor : IImageProcessor
{
    private float _strength;
    private float _radius;

    public VignetteProcessor(float strength = 0.5f, float radius = 1.0f)
    {
        _strength = strength;
        _radius = radius;
    }

    public Image Process(Image input)
    {
        var output = new Image(input.Width, input.Height, input.Channels);
        float centerX = input.Width / 2.0f;
        float centerY = input.Height / 2.0f;
        float maxDistance = MathF.Sqrt(centerX * centerX + centerY * centerY);

        Parallel.For(0, input.Height, y =>
        {
            for (int x = 0; x < input.Width; x++)
            {
                float dx = (x - centerX) / centerX;
                float dy = (y - centerY) / centerY;
                float distance = MathF.Sqrt(dx * dx + dy * dy);
                float vignette = 1.0f - MathF.Min(1.0f, distance / _radius);
                vignette = 1.0f - (_strength * (1.0f - vignette * vignette));

                for (int c = 0; c < input.Channels; c++)
                {
                    float value = input.GetPixelComponentF(x, y, c) * vignette;
                    output.SetPixelComponentF(x, y, c, value);
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
    }
}