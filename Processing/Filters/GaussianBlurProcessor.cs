using ImagoCrafter.Core;

namespace ImagoCrafter.Processing.Filters;

public class GaussianBlurProcessor : IImageProcessor
{
    private float _sigma;
    private int _kernelSize;
    private float[] _kernel1D = Array.Empty<float>();

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
        int radius = _kernelSize / 2;

        Parallel.For(0, input.Height, y =>
        {
            for (int x = 0; x < input.Width; x++)
            {
                for (int c = 0; c < input.Channels; c++)
                {
                    float sum = 0;
                    for (int k = -radius; k <= radius; k++)
                    {
                        int px = x + k;
                        float sourceValue = input.GetPixelComponentSafe(px, y, c);
                        sum += sourceValue * _kernel1D[k + radius];
                    }
                    temp.SetPixelComponentF(x, y, c, sum);
                }
            }
        });

        Parallel.For(0, output.Height, y =>
        {
            for (int x = 0; x < output.Width; x++)
            {
                for (int c = 0; c < output.Channels; c++)
                {
                    float sum = 0;
                    for (int k = -radius; k <= radius; k++)
                    {
                        int py = y + k;
                        float sourceValue = temp.GetPixelComponentSafe(x, py, c);
                        sum += sourceValue * _kernel1D[k + radius];
                    }
                    output.SetPixelComponentF(x, y, c, sum);
                }
            }
        });

        return output;
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