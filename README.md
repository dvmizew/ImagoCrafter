# ImagoCrafter

ImagoCrafter is a image processing application built in C# on .NET 9.0, providing a suite of filters and processing techniques accessible via a command-line interface.

## Downloads

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/dvmizew/ImagoCrafter?style=flat-square&color=blue)](https://github.com/dvmizew/ImagoCrafter/releases/latest)
[![GitHub downloads](https://img.shields.io/github/downloads/dvmizew/ImagoCrafter/total?style=flat-square&color=green)](https://github.com/dvmizew/ImagoCrafter/releases)
[![GitHub stars](https://img.shields.io/github/stars/dvmizew/ImagoCrafter?style=flat-square&color=yellow)](https://github.com/dvmizew/ImagoCrafter/stargazers)
[![GitHub license](https://img.shields.io/github/license/dvmizew/ImagoCrafter?style=flat-square&color=orange)](https://github.com/dvmizew/ImagoCrafter/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple?style=flat-square)](https://dotnet.microsoft.com/download/dotnet/8.0)

**[Download the latest version from Releases page](https://github.com/dvmizew/ImagoCrafter/releases)**

## Build from Source

```bash
git clone https://github.com/dvmizew/ImagoCrafter.git
cd ImagoCrafter

dotnet build
dotnet run <command> <arguments>
```

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