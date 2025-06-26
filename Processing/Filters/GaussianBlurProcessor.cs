using ImagoCrafter.Core;

namespace ImagoCrafter.Processing.Filters;

public class GaussianBlurProcessor : IImageProcessor
{
    private float _sigma;
    private int _kernelSize;
    private float[] _kernel1D = [];

    public GaussianBlurProcessor(float sigma = 1.0f)
    {
        Configure(new Dictionary<string, object> { { "sigma", sigma } });
    }

    private void Generate1DKernel()
    {
        int radius = _kernelSize / 2;
        _kernel1D = new float[_kernelSize];
        float sigmaSqr = _sigma * _sigma;
        float twoSigmaSqr = 2 * sigmaSqr;

        float sum = 0;
        for (int x = -radius; x <= radius; x++)
        {
            float value = MathF.Exp(-(x * x) / twoSigmaSqr);
            _kernel1D[x + radius] = value;
            sum += value;
        }

        // normalize
        for (int i = 0; i < _kernelSize; i++)
        {
            _kernel1D[i] /= sum;
        }
    }

    public Image Process(Image input)
    {
        var temp = new Image(input.Width, input.Height, input.Channels);
        var output = new Image(input.Width, input.Height, input.Channels);

        // horizontal blur
        ApplyKernel(input, temp, true);
        
        // vertical blur
        ApplyKernel(temp, output, false);

        return output;
    }

    private void ApplyKernel(Image source, Image target, bool horizontal)
    {
        int radius = _kernelSize / 2;
        
        Parallel.For(0, source.Height, y =>
        {
            for (int x = 0; x < source.Width; x++)
            {
                for (int c = 0; c < source.Channels; c++)
                {
                    float sum = 0;
                    for (int k = -radius; k <= radius; k++)
                    {
                        int px = horizontal ? x + k : x;
                        int py = horizontal ? y : y + k;
                        float sourceValue = source.GetPixelComponentSafe(px, py, c);
                        sum += sourceValue * _kernel1D[k + radius];
                    }
                    target.SetPixelComponentF(x, y, c, sum);
                }
            }
        });
    }

    public void Configure(Dictionary<string, object> parameters)
    {
        if (parameters.TryGetValue("sigma", out var sigmaObj) && sigmaObj is float sigma)
        {
            _sigma = sigma;
            _kernelSize = CalculateKernelSize(sigma);
            Generate1DKernel();
        }
    }

    private static int CalculateKernelSize(float sigma)
    {
        int size = (int)(6 * sigma);
        return Math.Max(3, size | 1);
    }
}