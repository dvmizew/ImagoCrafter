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
            var image = ImageLoader.Load(inputFile);
            Image? result = null;

            switch (command)
            {
                case "blur":
                    float sigma = 1.0f;

                    if (args.Length > 2)
                    {
                        sigma = float.Parse(args[2]);
                    }
                    
                    var blurProcessor = new GaussianBlurProcessor(sigma);
                    result = blurProcessor.Process(image);
                    break;

                case "vignette":
                    float strength = 0.5f;
                    float radius = 1.0f;

                    if (args.Length > 2)
                        strength = float.Parse(args[2]);
                    if (args.Length > 3)
                        radius = float.Parse(args[3]);

                    var vignetteProcessor = new VignetteProcessor(strength, radius);
                    result = vignetteProcessor.Process(image);
                    break;

                case "resize":
                    if (args.Length < 4)
                    {
                        Console.WriteLine("Error: Width and height parameters are required for resize.");
                        PrintUsage("resize");
                        return;
                    }
                    
                    if (!int.TryParse(args[2], out int width) || !int.TryParse(args[3], out int height))
                    {
                        Console.WriteLine("Error: Width and height must be valid integers.");
                        return;
                    }

                    var resizeProcessor = new ResizeProcessor(width, height);
                    result = resizeProcessor.Process(image);
                    break;

                default:
                    Console.WriteLine($"Unknown command: {command}");
                    PrintUsage();
                    return;
            }

            if (result != null)
            {
                string outputFile = Path.ChangeExtension(inputFile, ".processed" + Path.GetExtension(inputFile));
                ImageLoader.Save(result, outputFile);
                Console.WriteLine($"Image saved to: {outputFile}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void PrintUsage(string? command = null)
    {
        if (command == null)
        {
            Console.WriteLine("Usage: ImagoCrafter <command> <input-file> [options]");
            Console.WriteLine("\nAvailable commands:");
            Console.WriteLine("  blur       - Apply Gaussian blur effect");
            Console.WriteLine("  vignette   - Apply vignette (edge darkening) effect");
            Console.WriteLine("  resize     - Resize image to specified dimensions");
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
                Console.WriteLine("\nParameters:");
                Console.WriteLine("  width       - Target width in pixels");
                Console.WriteLine("  height      - Target height in pixels");
                Console.WriteLine("               Example: ImagoCrafter resize image.jpg 800 600");
                break;
            default:
                Console.WriteLine($"Unknown command: {command}");
                PrintUsage();
                break;
        }
    }
}