using ImagoCrafter.Core;
using ImagoCrafter.Processing.Filters;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("ImagoCrafter - Image Processing Tool");
        Console.WriteLine("-----------------------------------");

        if (args.Length == 0 || args[0] == "--help")
        {
            PrintUsage();
            return;
        }

        string command = args[0].ToLower();
        
        if (args.Length == 2 && args[1] == "--help")
        {
            PrintUsage(command);
            return;
        }

        if (args.Length < 2)
        {
            PrintUsage();
            return;
        }

        string inputFile = args[1];

        try
        {
            using var progressReporter = new ShellProgressReporter("Loading image...");
            var image = ImageLoader.Load(inputFile, progressReporter);
            
            progressReporter.Report(0, "Starting image processing...");
            ProcessImage(command, image, args, progressReporter);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void ProcessImage(string command, Image image, string[] args, IProgressReporter? progressReporter = null)
    {
        Image? result;

        switch (command)
        {
            case "blur":
                progressReporter?.Report(0, "Starting blur operation...");
                float sigma = args.Length > 2 ? ParseFloat(args[2], 1.0f) : 1.0f;                    
                result = new GaussianBlurProcessor(sigma).Process(image);
                progressReporter?.Report(100, "Blur operation completed");
                break;

            case "vignette":
                progressReporter?.Report(0, "Starting vignette operation...");
                float strength = args.Length > 2 ? ParseFloat(args[2], 0.5f) : 0.5f;
                float radius = args.Length > 3 ? ParseFloat(args[3], 1.0f) : 1.0f;
                result = new VignetteProcessor(strength, radius).Process(image);
                progressReporter?.Report(100, "Vignette operation completed");
                break;

            case "resize":
                progressReporter?.Report(0, "Starting resize operation...");
                if (args.Length < 3)
                {
                    Console.WriteLine("Error: Either percentage or width/height parameters are required for resize.");
                    PrintUsage("resize");
                    return;
                }

                int width, height;
                
                // percentage
                if (args[2].EndsWith("%"))
                {
                    if (args.Length != 3)
                    {
                        Console.WriteLine("Error: When using percentage, provide only one value.");
                        PrintUsage("resize");
                        return;
                    }

                    string percentStr = args[2].TrimEnd('%');
                    if (!float.TryParse(percentStr, out float percent))
                    {
                        Console.WriteLine("Error: Invalid percentage value.");
                        return;
                    }

                    if (percent <= 0)
                    {
                        Console.WriteLine("Error: Percentage must be positive.");
                        return;
                    }

                    width = (int)(image.Width * (percent / 100));
                    height = (int)(image.Height * (percent / 100));
                }
                else
                {
                    // absolute dimensions logic
                    if (args.Length < 4)
                    {
                        Console.WriteLine("Error: Width and height parameters are required for absolute resize.");
                        PrintUsage("resize");
                        return;
                    }

                    if (!int.TryParse(args[2], out width) || !int.TryParse(args[3], out height))
                    {
                        Console.WriteLine("Error: Width and height must be valid integers.");
                        return;
                    }

                    if (width <= 0 || height <= 0)
                    {
                        Console.WriteLine("Error: Width and height must be positive values.");
                        return;
                    }
                }

                result = new ResizeProcessor(width, height).Process(image);
                progressReporter?.Report(100, "Resize operation completed");
                break;

            case "sharpen":
                progressReporter?.Report(0, "Starting sharpen operation...");
                float sharpStrength = args.Length > 2 ? ParseFloat(args[2], 0.5f) : 0.5f;
                float sharpRadius = args.Length > 3 ? ParseFloat(args[3], 1.0f) : 1.0f;
                float threshold = args.Length > 4 ? ParseFloat(args[4], 0.0f) : 0.0f;
                result = new UnsharpMaskProcessor(sharpStrength, sharpRadius, threshold).Process(image);
                progressReporter?.Report(100, "Sharpen operation completed");
                break;

            default:
                Console.WriteLine($"Unknown command: {command}");
                PrintUsage();
                return;
        }

        if (result != null)
        {
            string outputFile = Path.ChangeExtension(args[1], ".processed" + Path.GetExtension(args[1]));
            Console.WriteLine();
            ImageLoader.Save(result, outputFile, progressReporter);
            Console.WriteLine($"Image saved to: {outputFile}");
        }
    }
    
    private static float ParseFloat(string value, float defaultValue)
    {
        return float.TryParse(value, out float result) ? result : defaultValue;
    }

    static void PrintUsage(string? command = null)
    {
        if (command == null)
        {
            Console.WriteLine("Usage: ImagoCrafter <command> <input-file> [options]");
            Console.WriteLine("\nAvailable commands:");
            Console.WriteLine("  blur       - Apply Gaussian blur effect");
            Console.WriteLine("  vignette   - Apply vignette (edge darkening) effect");
            Console.WriteLine("  resize     - Resize image to specified dimensions or percentage");
            Console.WriteLine("  sharpen    - Apply unsharp mask sharpening effect");
            Console.WriteLine("\nFor command-specific options, use: ImagoCrafter <command> --help");
            return;
        }

        switch (command.ToLower())
        {
            case "blur":
                Console.WriteLine("Usage: ImagoCrafter blur <input-file> [sigma]");
                Console.WriteLine("\nOptions:");
                Console.WriteLine("  sigma      - Blur intensity (default: 1.0).");
                Console.WriteLine("               Example: ImagoCrafter blur image.jpg 2.5");
                break;
            case "vignette":
                Console.WriteLine("Usage: ImagoCrafter vignette <input-file> [strength] [radius]");
                Console.WriteLine("\nOptions:");
                Console.WriteLine("  strength   - Vignette strength from 0.0 to 1.0 (default: 0.5)");
                Console.WriteLine("  radius     - Vignette radius, larger values affect more of the image (default: 1.0)");
                Console.WriteLine("               Example: ImagoCrafter vignette image.jpg 0.7 1.2");
                break;
            case "resize":
                Console.WriteLine("Usage: ImagoCrafter resize <input-file> <width> <height>");
                Console.WriteLine("       ImagoCrafter resize <input-file> <percentage>%");
                Console.WriteLine("\nParameters:");
                Console.WriteLine("  width       - Target width in pixels");
                Console.WriteLine("  height      - Target height in pixels");
                Console.WriteLine("  percentage  - Resize image by percentage (e.g., 50%)");
                Console.WriteLine("               Example: ImagoCrafter resize image.jpg 800 600");
                Console.WriteLine("               Example: ImagoCrafter resize image.jpg 50%");
                break;
            case "sharpen":
                Console.WriteLine("Usage: ImagoCrafter sharpen <input-file> [strength] [radius] [threshold]");
                Console.WriteLine("\nOptions:");
                Console.WriteLine("  strength    - Sharpening strength from 0.0 to 2.0 (default: 0.5)");
                Console.WriteLine("  radius      - Blur radius for unsharp mask (default: 1.0)");
                Console.WriteLine("  threshold   - Threshold to prevent noise amplification (default: 0.0)");
                Console.WriteLine("               Example: ImagoCrafter sharpen image.jpg 1.0 1.5 0.01");
                break;
            default:
                Console.WriteLine($"Unknown command: {command}");
                PrintUsage();
                break;
        }
    }
}