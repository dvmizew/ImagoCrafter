namespace ImagoCrafter.Processing.Kernels;

public class SharpenKernel : ConvolutionKernel
{
    public SharpenKernel(float strength = 0.5f) : base(5)
    {
        // Enhanced Unsharp Mask kernel
        // Center weight is calculated to maintain zero-sum property
        float corner = -strength * 0.1f;  // diagonal elements
        float edge = -strength * 0.2f;    // edge elements
        float center = -(4 * corner + 4 * edge); // center element to maintain zero sum

        float[,] kernel = {
            { corner,  edge,   -strength*0.1f,  edge,    corner },
            { edge,    0.0f,   -strength*0.2f,  0.0f,    edge },
            { -strength*0.1f, -strength*0.2f, 1.0f + center, -strength*0.2f, -strength*0.1f },
            { edge,    0.0f,   -strength*0.2f,  0.0f,    edge },
            { corner,  edge,   -strength*0.1f,  edge,    corner }
        };

        // Apply strength-based normalization
        float sum = 0;
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                if (i != 2 || j != 2) // exclude center
                    sum += kernel[i, j];

        // Ensure the kernel preserves brightness while enhancing edges
        kernel[2, 2] = 1.0f - sum;
        
        // Set the kernel with a slight brightness boost
        SetKernel(kernel, 1.05f); // 5% brightness boost to compensate for edge enhancement
    }
}