#!/bin/bash

# Check if pdflatex is installed
if ! command -v pdflatex &> /dev/null; then
    echo "Error: pdflatex is not installed. Please install TeX Live or MiKTeX."
    exit 1
fi

# Navigate to the Documentation directory
cd "$(dirname "$0")/Documentation"

# Run pdflatex twice to resolve references
pdflatex ImagoCrafter.tex
pdflatex ImagoCrafter.tex

echo "PDF generated at: Documentation/ImagoCrafter.pdf"