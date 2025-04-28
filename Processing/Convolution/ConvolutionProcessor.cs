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

        for (int y = 0; y < input.Height; y++)
        {
            for (int x = 0; x < input.Width; x++)
            {
                for (int c = 0; c < input.Channels; c++)
                {
                    float sum = 0;
                    
                    for (int ky = -kernelRadius; ky <= kernelRadius; ky++)
                    {
                        for (int kx = -kernelRadius; kx <= kernelRadius; kx++)
                        {
                            int px = Math.Clamp(x + kx, 0, input.Width - 1);
                            int py = Math.Clamp(y + ky, 0, input.Height - 1);
                            float kernelValue = _kernel.Matrix[ky + kernelRadius, kx + kernelRadius];
                            sum += input.GetPixelComponent(px, py, c) * kernelValue;
                        }
                    }

                    output.SetPixelComponent(x, y, c, (byte)Math.Clamp(sum * _kernel.Factor + _kernel.Bias, 0, 255));
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