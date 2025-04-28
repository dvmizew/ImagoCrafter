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
        float twoSigmaSqr = 2 * _sigma * _sigma;
        float constant = 1.0f / (MathF.PI * twoSigmaSqr);

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                float distSqr = x * x + y * y;
                _kernel[y + radius, x + radius] = constant * MathF.Exp(-distSqr / twoSigmaSqr);
            }
        }

        Normalize();
        _factor = 1.0f;
    }
}