@echo off
REM Check if pdflatex is in PATH
where pdflatex >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo Error: pdflatex is not installed or not in PATH.
    echo Please install MiKTeX or TeX Live.
    exit /b 1
)

REM Navigate to Documentation directory
cd %~dp0Documentation

REM Run pdflatex twice to resolve references
pdflatex ImagoCrafter.tex
pdflatex ImagoCrafter.tex

echo PDF generated at: Documentation\ImagoCrafter.pdf
pause