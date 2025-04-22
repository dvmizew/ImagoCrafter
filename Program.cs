using ImagoCrafter.Core;
using ImagoCrafter.Processing.Filters;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("ImagoCrafter - Image Processing Tool");
        Console.WriteLine("-----------------------------------");

        if (args.Length < 2)
        {
            PrintUsage();
            return;
        }

        string command = args[0].ToLower();
        string inputFile = args[1];

        try
        {
            var image = ImageLoader.Load(inputFile);
            Image? result = null;

            switch (command)
            {
                case "blur":
                    var blurProcessor = new GaussianBlurProcessor();
                    if (args.Length > 2)
                    {
                        float sigma = float.Parse(args[2]);
                        blurProcessor.Configure(new Dictionary<string, object> { { "sigma", sigma } });
                    }
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

    static void PrintUsage()
    {
        Console.WriteLine("Usage: ImagoCrafter <command> <input-file> [options]");
        Console.WriteLine("\nAvailable commands:");
        Console.WriteLine("  blur       - Apply blur effect [sigma]");
    }
}