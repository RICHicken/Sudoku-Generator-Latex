@echo off
set /p name="name of file: "
start /wait "" ".\Core Files\Sudoku Generator.exe" n %name% RICHicken 0 separate dontwait
start /wait "" pdflatex .\output\%name%.tex
start /wait "" pdflatex .\output\%name%_sol.tex

mkdir .\pdfs\%name%

move %name%.aux output
move %name%.log output
move %name%.pdf .\pdfs\%name%

move %name%_sol.aux output
move %name%_sol.log output
move %name%_sol.pdf .\pdfs\%name%