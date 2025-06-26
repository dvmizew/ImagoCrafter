namespace ImagoCrafter.Processing.Kernels;

public class ConvolutionKernel(int size)
{
    protected float[,] _kernel = new float[size, size];
    protected int _size = size;
    protected float _factor = 1.0f;
    protected float _bias = 0.0f;

    public float[,] Matrix => _kernel;
    public float Factor => _factor;
    public float Bias => _bias;
    public int Size => _size;

    public void SetKernel(float[,] kernel, float factor = 1.0f, float bias = 0.0f)
    {
        _kernel = kernel;
        _factor = factor;
        _bias = bias;
    }

    protected void Normalize()
    {
        float sum = _kernel.Cast<float>().Sum();
        if (sum != 0)
        {
            for (int i = 0; i < _size; i++)
                for (int j = 0; j < _size; j++)
                    _kernel[i, j] /= sum;
        }
    }
}