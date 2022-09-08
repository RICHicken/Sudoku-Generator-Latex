using System;
using Sudoku_Solver;
using System.Collections.Generic;
using System.IO;

namespace Sudoku_Generator
{
    class Generator
    {
        private const int GRID_LENGTH = 9;
        private const double MAX_SIZE = 1.346;
        private const int MIN_SIZE = 1;
        private static double fontSize;
        private static string printName;
        private static string author;
        static void Main(string[] args)
        {
            Random r = new Random();
            List<Solver.Point> untouchedSlots = new List<Solver.Point>();
            int[,] board = new int[GRID_LENGTH, GRID_LENGTH];
            int[,] solution;
            bool symmetry;
            bool separate;
            string name;


            // Process Args (these have to be in a specific order for now)

            // Symmetry 
            if (args.Length > 0)
            {
                symmetry = (args[0] != "n");
            }
            else
            {
                Console.WriteLine("Do you want symmetry? (y/n)");
                ConsoleKey response;
                do
                {
                    response = Console.ReadKey().Key;

                    symmetry = (response.Equals(ConsoleKey.Y));

                } while (response != ConsoleKey.Y && response != ConsoleKey.N);
            }


            // Title of Puzzle
            string input = "";
            if (args.Length > 1)
            {
                name = args[1];
            }
            else
            {
                do
                {
                    Console.WriteLine("\nWrite name of the File");
                    input = Console.ReadLine();
                } while (input.Length == 0);

                name = input;
            }
            printName = name.Replace('_', ' ');

            // Author
            input = "";

            if (args.Length > 2)
            {
                input = args[2];
            }
            else
            {
                Console.WriteLine("\nName of Author");
                input = Console.ReadLine();
            }

            if (input.Length == 0)
            {
                author = "";
            }
            else
            {
                author = "Made By: " + input;
            }

            // Font Size
            input = "";
            if (args.Length > 3)
            {
                if (!double.TryParse(args[3], out fontSize))
                {
                    fontSize = 0;
                }
            }
            else
            {
                do
                {
                    Console.WriteLine("\nType size (leave blank for default)");
                    string temp = Console.ReadLine();
                    input = (temp == "") ? "0" : temp;
                } while (!double.TryParse(input, out fontSize));
            }


            if (fontSize == 0)
            {
                fontSize = MAX_SIZE;
            }
            else if (fontSize > MAX_SIZE)
            {
                Console.WriteLine($"Size too big, clamping to {MAX_SIZE}");
                fontSize = MAX_SIZE;
            }
            else if (fontSize < MIN_SIZE)
            {
                Console.WriteLine($"Size too small, clamping to {MIN_SIZE}");
                fontSize = MIN_SIZE;
            }


            // Generate the board 
            Console.WriteLine("\nGenerating...");

            Solver.Setup();
            Solver.RandomSolve(board);                      //finds a random solved (completed) board
            board = (int[,])Solver.solution.Clone();        //copying the solution
            solution = (int[,])Solver.solution.Clone();    //copying the solution again to display later




            // Now we want to remove some values

            // Populate untouchedSlots with all slots in board

            for (int x = 0; x < GRID_LENGTH; x++)
            {
                for (int y = 0; y < GRID_LENGTH; y++)
                {

                    untouchedSlots.Add(new Solver.Point(x, y));
                }
            }

            /*
             * for each untouched slot, we want to remove a value and see if we still have 1 solution
             *      keep the number removed if we do have only 1 solution
             *      place the number back if we have more than 1 solution
             */
            while (untouchedSlots.Count > 0)
            {
                int rand = r.Next(untouchedSlots.Count);    //pick a random untouched slot
                Solver.Point p = untouchedSlots[rand];
                untouchedSlots.RemoveAt(rand);

                int numSave = board[p.y, p.x];              //save the number stored at that slot

                //remove the number at position
                board[p.y, p.x] = 0;                        //remove the number at that position
                Solver.Solve(board);            //find all solutions

                if (Solver.numOfSols > 1)                    //if there is more than 1 solution, put the number back
                {
                    board[p.y, p.x] = numSave;
                }

            }

            // Make it symmetrical
            if (symmetry)
            {
                for (int x = 0; x < GRID_LENGTH; x++)
                {
                    for (int y = 0; y < GRID_LENGTH; y++)
                    {
                        if (board[y, x] != 0)
                        {
                            board[8 - y, 8 - x] = solution[8 - y, 8 - x];
                        }
                    }
                }
            }


            // Check if user wants the puzzle and solution as separate pages
            if (args.Length > 4)
            {
                separate = (args[4] == "separate");
            }
            else
            {
                Console.WriteLine("Do you want puzzle and solution as separate pages? (y/n)");
                ConsoleKey response;
                do
                {
                    response = Console.ReadKey().Key;

                    separate = (response.Equals(ConsoleKey.Y));

                } while (response != ConsoleKey.Y && response != ConsoleKey.N);
            }

            // Write to file
            
            char slash = Path.DirectorySeparatorChar;
            if (!Directory.Exists(Directory.GetCurrentDirectory() + slash + "output"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + slash + "output");
            }

            if (separate)
            {
                StreamWriter puzzle = new StreamWriter(Directory.GetCurrentDirectory() + $"{slash}output{slash}{name}.tex");
                StreamWriter sol = new StreamWriter(Directory.GetCurrentDirectory() + $"{slash}output{slash}{name}_sol.tex");
                SaveSeparateLatex(board, solution, puzzle, sol);
            }
            else
            {
                StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + $"{slash}output{slash}{name}.tex");
                SaveLatex(board, solution, sw);
            }




            // Print puzzle and solution (if dontwait was not said through console args)

            if (!(args.Length > 5 && args[5] == "dontwait"))
            {
                Console.WriteLine("\nPUZZLE:");
                PrintGridNoZeroes(board);

                Console.WriteLine("\n\nSOLUTION");
                PrintGrid(solution);

                Console.WriteLine("Press any key to close...");
                Console.ReadKey();
            }
        }

        public static void PrintGrid(int[,] board)
        {
            string row;
            Console.WriteLine("|----------|----------|----------|");
            for (int x = 0; x < GRID_LENGTH; x++)
            {
                row = "| ";
                for (int y = 0; y < GRID_LENGTH; y++)
                {
                    row += board[x, y] + "  ";

                    if ((y + 1) % 3 == 0)
                    {
                        row += "| ";
                    }

                }
                Console.WriteLine(row);
                if ((x + 1) % 3 == 0)
                {
                    Console.WriteLine("|----------|----------|----------|");
                }
            }
        }

        public static void PrintGridNoZeroes(int[,] board)
        {
            string row;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("|----------|----------|----------|");
            for (int x = 0; x < GRID_LENGTH; x++)
            {
                row = "| ";
                for (int y = 0; y < GRID_LENGTH; y++)
                {

                    string num = (board[x, y] == 0) ? "•" : board[x, y].ToString();

                    row += num + "  ";

                    if ((y + 1) % 3 == 0)
                    {
                        row += "| ";
                    }

                }
                Console.WriteLine(row);
                if ((x + 1) % 3 == 0)
                {
                    Console.WriteLine("|----------|----------|----------|");
                }
            }
        }

        public static void SaveLatex(int[,] board, int[,] solution, StreamWriter sw)
        {
            sw.WriteLine("\\documentclass{article}" +
                              "\n\\usepackage[utf8]{inputenc}" +
                              "\n\\usepackage{setspace}" +
                              "\n\\usepackage{tikz}" +
                              "\n\\usetikzlibrary{matrix}" +
                              "\n\\pagenumbering{gobble}");

            sw.WriteLine($"\\newcommand \\size{{{fontSize}}}");

            sw.WriteLine("\n\n" +
                                "\\begin{document}" +
                                "\n\t\\doublespacing" +
                                "\n\t\\begin{center}" +
                                $"\n\t\\huge{{Sudoku Puzzle: {printName}}}\\\\" +
                                $"\n\t\\large{{{author}}}\\\\" +
                                "\n\t\\today\\\\" +
                                "\n\t\\vspace{0.5in}");

            sw.WriteLine("\t\t\\begin{tikzpicture}" +
                                "\n\t\t\t\\draw[line width = 1, scale = \\size] (0, 0) grid (9, 9);\n");

            sw.Flush();

            for (int x = 9; x >= 1; x--)
            {
                for (int y = 9; y >= 1; y--)
                {
                    int cell = board[9 - x, y - 1];
                    string outCell = (cell == 0) ? "" : cell.ToString();
                    sw.WriteLine($"\t\t\t\t\\node at ({y} * \\size - \\size / 2, {x} * \\size - \\size / 2) {{{outCell}}};");
                }

            }
            sw.Flush();

            sw.WriteLine("\n\t\t\t\\draw[line width = 2, scale = 3*\\size] (0, 0) grid (3, 3);" +
                                "\n\t\t\\end{tikzpicture}" +
                                "\n\t\\end{center}" +
                                "\n\\newpage");


            sw.WriteLine("\n\n" +
                                "\n\t\\begin{center}" +
                                $"\n\t\\huge{{Solution for {printName}}}\\\\" +
                                $"\n\t\\large{{{author}}}\\\\" +
                                "\n\t\\today\\\\" +
                                "\n\t\\vspace{0.5in}");

            sw.WriteLine("\t\t\\begin{tikzpicture}" +
                    "\n\t\t\t\\draw[line width = 1, scale = \\size] (0, 0) grid (9, 9);\n");

            sw.Flush();

            for (int x = 9; x >= 1; x--)
            {
                for (int y = 9; y >= 1; y--)
                {
                    int cell = solution[9 - x, y - 1];
                    string outCell = (cell == 0) ? "" : cell.ToString();
                    sw.WriteLine($"\t\t\t\t\\node at ({y} * \\size - \\size / 2, {x} * \\size - \\size / 2) {{{outCell}}};");
                }

            }

            sw.Flush();

            sw.WriteLine("\n\t\t\t\\draw[line width = 2, scale = 3*\\size] (0, 0) grid (3, 3);" +
                                "\n\t\t\\end{tikzpicture}");

            sw.Flush();
            sw.WriteLine("\t\\end{center}");

            sw.Flush();

            sw.WriteLine("\\end{document}");

            sw.Close();
        }

        public static void SaveSeparateLatex(int[,] board, int[,] solution, StreamWriter puzzle, StreamWriter sol)
        {
            puzzle.WriteLine("\\documentclass{article}" +
                              "\n\\usepackage[utf8]{inputenc}" +
                              "\n\\usepackage{setspace}" +
                              "\n\\usepackage{tikz}" +
                              "\n\\usetikzlibrary{matrix}" +
                              "\n\\pagenumbering{gobble}");

            puzzle.WriteLine($"\\newcommand \\size{{{fontSize}}}");

            puzzle.WriteLine("\n\n" +
                                "\\begin{document}" +
                                "\n\t\\doublespacing" +
                                "\n\t\\begin{center}" +
                                $"\n\t\\huge{{Sudoku Puzzle: {printName}}}\\\\" +
                                $"\n\t\\large{{{author}}}\\\\" +
                                "\n\t\\today\\\\" +
                                "\n\t\\vspace{0.5in}");

            puzzle.WriteLine("\t\t\\begin{tikzpicture}" +
                                "\n\t\t\t\\draw[line width = 1, scale = \\size] (0, 0) grid (9, 9);\n");

            puzzle.Flush();

            for (int x = 9; x >= 1; x--)
            {
                for (int y = 9; y >= 1; y--)
                {
                    int cell = board[9 - x, y - 1];
                    string outCell = (cell == 0) ? "" : cell.ToString();
                    puzzle.WriteLine($"\t\t\t\t\\node at ({y} * \\size - \\size / 2, {x} * \\size - \\size / 2) {{{outCell}}};");
                }

            }
            puzzle.Flush();

            puzzle.WriteLine("\n\t\t\t\\draw[line width = 2, scale = 3*\\size] (0, 0) grid (3, 3);" +
                                "\n\t\t\\end{tikzpicture}" +
                                "\n\t\\end{center}" +
                                "\n\\end{document}");
            puzzle.Close();

            sol.WriteLine("\\documentclass{article}" +
                  "\n\\usepackage[utf8]{inputenc}" +
                  "\n\\usepackage{setspace}" +
                  "\n\\usepackage{tikz}" +
                  "\n\\usetikzlibrary{matrix}" +
                  "\n\\pagenumbering{gobble}");

            sol.WriteLine($"\\newcommand \\size{{{fontSize}}}");

            sol.WriteLine("\n\n" +
                                "\\begin{document}" +
                                "\n\t\\doublespacing");
            sol.Flush();

            sol.WriteLine("\n\n" +
                                "\n\t\\begin{center}" +
                                $"\n\t\\huge{{Solution for {printName}}}\\\\" +
                                $"\n\t\\large{{{author}}}\\\\" +
                                "\n\t\\today\\\\" +
                                "\n\t\\vspace{0.5in}");

            sol.WriteLine("\t\t\\begin{tikzpicture}" +
                    "\n\t\t\t\\draw[line width = 1, scale = \\size] (0, 0) grid (9, 9);\n");

            sol.Flush();

            for (int x = 9; x >= 1; x--)
            {
                for (int y = 9; y >= 1; y--)
                {
                    int cell = solution[9 - x, y - 1];
                    string outCell = (cell == 0) ? "" : cell.ToString();
                    sol.WriteLine($"\t\t\t\t\\node at ({y} * \\size - \\size / 2, {x} * \\size - \\size / 2) {{{outCell}}};");
                }

            }

            sol.Flush();

            sol.WriteLine("\n\t\t\t\\draw[line width = 2, scale = 3*\\size] (0, 0) grid (3, 3);" +
                                "\n\t\t\\end{tikzpicture}");

            sol.Flush();
            sol.WriteLine("\t\\end{center}");

            sol.Flush();

            sol.WriteLine("\\end{document}");

            sol.Close();
        }

        public static string GetLatex(int[,] board, int[,] solution)
        {
            string output;
            output = ("\\documentclass{article}" +
                              "\n\\usepackage[utf8]{inputenc}" +
                              "\n\\usepackage{setspace}" +
                              "\n\\usepackage{tikz}" +
                              "\n\\usetikzlibrary{matrix}");

            output += ($"\n\\newcommand \\size{{{fontSize}}}");

            output += ("\n\n\n" +
                                "\\begin{document}" +
                                "\n\t\\doublespacing" +
                                "\n\t\\begin{center}" +
                                "\n\t\\huge{Sudoku Puzzle}\\\\" +
                                $"\n\t\\large{{{author}}}\\\\" +
                                "\n\t\\today\\\\" +
                                "\n\t\\vspace{0.5in}");

            output += ("\n\t\t\\begin{tikzpicture}" +
                                "\n\t\t\t\\draw[line width = 1, scale = \\size] (0, 0) grid (9, 9);\n");

            for (int x = 9; x >= 1; x--)
            {
                for (int y = 9; y >= 1; y--)
                {
                    int cell = board[9 - x, y - 1];
                    string outCell = (cell == 0) ? "" : cell.ToString();
                    output += ($"\n\t\t\t\t\\node at ({y} * \\size - \\size / 2, {x} * \\size - \\size / 2) {{{outCell}}};");
                }

            }

            output += ("\n\n\t\t\t\\draw[line width = 2, scale = 3*\\size] (0, 0) grid (3, 3);" +
                                "\n\t\t\\end{tikzpicture}" +
                                "\n\t\\end{center}" +
                                "\n\\newpage");


            output += ("\n\n\n" +
                                "\n\t\\begin{center}" +
                                "\n\t\\huge{Sudoku Puzzle Solution}\\\\" +
                                $"\n\t\\large{{{author}}}\\\\" +
                                "\n\t\\today\\\\" +
                                "\n\t\\vspace{0.5in}");

            output += ("\n\t\t\\begin{tikzpicture}" +
                    "\n\t\t\t\\draw[line width = 1, scale = \\size] (0, 0) grid (9, 9);\n");

            for (int x = 9; x >= 1; x--)
            {
                for (int y = 9; y >= 1; y--)
                {
                    int cell = solution[9 - x, y - 1];
                    string outCell = (cell == 0) ? "" : cell.ToString();
                    output += ($"\n\t\t\t\t\\node at ({y} * \\size - \\size / 2, {x} * \\size - \\size / 2) {{{outCell}}};");
                }

            }

            output += ("\n\n\t\t\t\\draw[line width = 2, scale = 3*\\size] (0, 0) grid (3, 3);" +
                                "\n\t\t\\end{tikzpicture}" +
                                "\n\t\\end{center}");

            output += ("\n\\end{document}");

            return output;
        }
    }



}
