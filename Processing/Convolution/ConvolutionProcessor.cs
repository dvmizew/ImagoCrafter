namespace ImagoCrafter.Processing.Convolution;

using ImagoCrafter.Core;
using ImagoCrafter.Processing.Kernels;

public class ConvolutionProcessor : IImageProcessor
{
    protected ConvolutionKernel _kernel;

    public ConvolutionProcessor(ConvolutionKernel kernel)
    {
        _kernel = kernel;
    }

    public virtual Image Process(Image input)
    {
        Image output = new Image(input.Width, input.Height, input.Channels);
        int kernelRadius = _kernel.Size / 2;

        for (int y = kernelRadius; y < input.Height - kernelRadius; y++)
        {
            for (int x = kernelRadius; x < input.Width - kernelRadius; x++)
            {
                for (int c = 0; c < input.Channels; c++)
                {
                    float sum = 0;
                    
                    for (int ky = -kernelRadius; ky <= kernelRadius; ky++)
                    {
                        for (int kx = -kernelRadius; kx <= kernelRadius; kx++)
                        {
                            int px = x + kx;
                            int py = y + ky;
                            float kernelValue = _kernel.Matrix[ky + kernelRadius, kx + kernelRadius];
                            sum += input.GetPixelComponent(px, py, c) * kernelValue;
                        }
                    }

                    sum = sum * _kernel.Factor + _kernel.Bias;
                    byte result = (byte)Math.Clamp(sum, 0, 255);
                    output.SetPixelComponent(x, y, c, result);
                }
            }
        }

        return output;
    }

    public virtual void Configure(Dictionary<string, object> parameters)
    {
        // Base implementation
    }
}