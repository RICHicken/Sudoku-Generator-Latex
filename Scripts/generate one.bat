@echo off
set /p name="name of file: "
start /wait "" ".\Core Files\Sudoku Generator.exe" n %name% RICHicken 0 notseparate dontwait
start /wait "" pdflatex .\output\%name%.tex
move %name%.aux output
move %name%.log output

if not exist ".\pdfs" mkdir .\pdfs
move %name%.pdf pdfs