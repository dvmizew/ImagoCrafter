namespace ImagoCrafter.Processing.Filters;

using ImagoCrafter.Core;
using ImagoCrafter.Processing.Convolution;
using ImagoCrafter.Processing.Kernels;

public class GaussianBlurProcessor : ConvolutionProcessor
{
    private float _sigma;
    private int _kernelSize;

    public GaussianBlurProcessor(int kernelSize = 5, float sigma = 1.0f) 
        : base(new GaussianKernel(kernelSize, sigma))
    {
        _sigma = sigma;
        _kernelSize = kernelSize;
    }

    public override void Configure(Dictionary<string, object> parameters)
    {
        if (parameters.TryGetValue("sigma", out var sigmaObj) && sigmaObj is float sigma)
        {
            _sigma = sigma;
        }
        if (parameters.TryGetValue("kernelSize", out var sizeObj) && sizeObj is int size)
        {
            _kernelSize = size;
        }

        _kernel = new GaussianKernel(_kernelSize, _sigma);
    }
}