# ImagoCrafter

ImagoCrafter is an advanced image processing application built in C# on .NET 8.0, providing a comprehensive suite of filters and processing techniques accessible via a command-line interface.

## Available Commands

- **blur** - Apply Gaussian blur effect
- **vignette** - Apply vignette (edge darkening) effect  
- **resize** - Resize image to specified dimensions or percentage
- **sharpen** - Apply unsharp mask sharpening effect

## Command Usage

```bash
# Gaussian blur
ImagoCrafter blur image.jpg 2.5

# Vignette effect
ImagoCrafter vignette image.jpg 0.7 1.2

# Resize by percentage or absolute dimensions
ImagoCrafter resize image.jpg 50%
ImagoCrafter resize image.jpg 800 600

# Unsharp mask sharpening
ImagoCrafter sharpen image.jpg 1.0 1.5 0.01
```

For detailed usage of each command, use: `ImagoCrafter <command> --help`

## Documentation
All implementation details and code examples are available in the PDF generated from the LaTeX source:

- `Documentation/ImagoCrafter.pdf`