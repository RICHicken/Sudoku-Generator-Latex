@echo off
set /p amount="number of puzzles: "
set /p inname="name of file: "

set name=%inname%_Sheet

mkdir .\pdfs\%inname%

for /l %%A in (1, 1, %amount%) do (
start /wait "" ".\Core Files\Sudoku Generator.exe" n %name%_%%A RICHicken 0 notseparate dontwait
start /wait "" pdflatex .\output\%name%_%%A.tex
move %name%_%%A.aux output
move %name%_%%A.log output
move %name%_%%A.pdf .\pdfs\%inname%
)
set loc=.\pdfs\%inname%

echo @echo off > %loc%\combine.bat

echo mkdir .\temp >> %loc%\combine.bat
echo set file=combine.tex >> %loc%\combine.bat
echo. >> %loc%\combine.bat
echo echo \documentclass{article} ^> %%file%% >> %loc%\combine.bat
echo echo \usepackage{pdfpages} ^>^> %%file%% >> %loc%\combine.bat
echo echo. ^>^> %%file%% >> %loc%\combine.bat
echo echo \begin{document} ^>^> %%file%% >> %loc%\combine.bat
echo. >> %loc%\combine.bat
echo. >> %loc%\combine.bat

for /l %%A in (1, 1, %amount%) do (
echo echo \includepdf[pages=-]{%name%_%%A} ^>^> %%file%% >> %loc%\combine.bat
)

echo. >> %loc%\combine.bat
echo. >> %loc%\combine.bat
echo echo \end{document} ^>^> %%file%% >> %loc%\combine.bat
echo. >> %loc%\combine.bat
echo start /wait "" pdflatex %%file%% >> %loc%\combine.bat
echo. >> %loc%\combine.bat
echo move combine.tex .\temp >> %loc%\combine.bat
echo move combine.aux .\temp >> %loc%\combine.bat
echo move combine.log .\temp >> %loc%\combine.bat
echo move combine.pdf >> %loc%\combine.bat