namespace ImagoCrafter.Processing.Kernels;

public class GaussianKernel : ConvolutionKernel
{
    private float _sigma;

    public GaussianKernel(int size, float sigma = 1.0f) : base(size)
    {
        _sigma = sigma;
        GenerateKernel();
    }

    private void GenerateKernel()
    {
        int radius = _size / 2;
        float sigmaSqr = _sigma * _sigma;
        float twoSigmaSqr = 2 * sigmaSqr;

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                float exponent = -(x * x + y * y) / twoSigmaSqr;
                _kernel[y + radius, x + radius] = (float)Math.Exp(exponent) / (float)(Math.PI * twoSigmaSqr);
            }
        }

        Normalize();
    }
}