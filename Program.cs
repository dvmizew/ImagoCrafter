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
            default:
                Console.WriteLine($"Unknown command: {command}");
                PrintUsage();
                break;
        }
    }
}