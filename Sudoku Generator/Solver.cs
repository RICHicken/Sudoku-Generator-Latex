using System;
using System.Collections.Generic;
namespace Sudoku_Solver
{
    class Solver
    {
        private const int GRID_LENGTH = 9;
        private const int CHUNK_LENGTH = 3;

        private static Random r = new Random();
        public static int numOfSols = 0;
        private static int[,] filledGrid = new int[GRID_LENGTH, GRID_LENGTH];

        private static List<int> possibilities = new List<int>(9);
        private static List<Point> empty = new List<Point>();



        public static int[,] solution;


        public static void RandomSolve(int[,] board) 
        {
            filledGrid = board;
            RandomSolve();

            solution = filledGrid;
        }
        private static bool RandomSolve()
        {
            GetEmptySlots();    //get all the empty slots (not necessary for just solving a puzzle, but this is mainly for the puzzle generator)
            if (empty.Count == 0)
            {
                return true;
            }

            Point nextEmpty = empty[0]; //get the first empty slot

            List<int> possibles = CheckSlotPossibilities(nextEmpty.x, nextEmpty.y);

            while (possibles.Count > 0)
            {
                int i = possibles[r.Next(possibles.Count)];
                possibles.Remove(i);

                filledGrid[nextEmpty.y, nextEmpty.x] = i;   //set this grid to one of the possible values

                if (RandomSolve())    //recursively call this until we filled the entire puzzle or if we reached a "dead end" (cannot put anymore values)
                {
                    return true;
                }

                filledGrid[nextEmpty.y, nextEmpty.x] = 0;   //if we reached a dead end, remove the value in this slot
            }


            return false;
        }
        public static void Setup()
        {
            for (int i = 1; i <= GRID_LENGTH; i++)
            {
                possibilities.Add(i);
            }
        }
        public static void Solve(int[,] board)
        {
            numOfSols = 0;

            empty = new List<Point>();
            filledGrid = (int[,]) board.Clone();

            Solve();
    }

        private static void Solve()
        {
            for (int y = 0; y < GRID_LENGTH; y++)
            {
                for (int x = 0; x < GRID_LENGTH; x++)
                {
                    if (filledGrid[y, x] == 0)
                    {
                        for (int n = 1; n < 10; n++)
                        {
                            if (Possible(x, y, n))
                            {
                                filledGrid[y, x] = n;
                                Solve();

                                if(numOfSols > 1)
                                {
                                    return;
                                }

                                filledGrid[y, x] = 0;
                            }
                        }
                        return;
                    }

                }
            }

            solution = filledGrid;
            numOfSols++;
        }

        /*
         * returns all possible values for filledGrid[y,x]
         * Precondition: filledGrid[y,x] == 0 (its empty)
         */
        private static List<int> CheckSlotPossibilities(int x, int y)
        {
            List<int> possible = new List<int>(possibilities.AsReadOnly());

            //checking row and column of slot
            for (int i = 0; i < GRID_LENGTH; i++)
            {
                possible.Remove(filledGrid[y, i]);
                possible.Remove(filledGrid[i, x]);
            }

            //checking chunk of slot
            Point topLeft = GetChunkStart(x, y);
            for (int i = topLeft.x; i < topLeft.x + CHUNK_LENGTH; i++)
            {
                for (int j = topLeft.y; j < topLeft.y + CHUNK_LENGTH; j++)
                {
                    possible.Remove(filledGrid[j, i]);
                }
            }


            return possible;
        }


        private static bool Possible(int x, int y, int n)
        {
            //check row and column
            for (int i = 0; i < GRID_LENGTH; i++)
            {
                if (filledGrid[y, i] == n || filledGrid[i, x] == n)
                {
                    return false;
                }
            }


                //check square chunk
                Point topLeft = GetChunkStart(x, y);
            for (int i = topLeft.x; i < topLeft.x + CHUNK_LENGTH; i++)
            {
                for (int j = topLeft.y; j < topLeft.y + CHUNK_LENGTH; j++)
                {
                   if(filledGrid[j, i] == n)
                    {
                        return false;
                    }
                }
            }


            return true;
        }



        //populates empty with empty slots (if slot == 0)
        private static void GetEmptySlots()
        {
            empty.Clear();

            for (int x = 0; x < GRID_LENGTH; x++)
            {
                for (int y = 0; y < GRID_LENGTH; y++)
                {
                    if (filledGrid[y, x] == 0) //if empty
                    {
                        empty.Add(new Point(x, y));
                    }
                }
            }
        }

        //populates empty with empty slots (if slot == 0)
        public static List<Point> GetEmptySlots(int[,] board)
        {
            List<Point> empties = new List<Point>();

            for (int x = 0; x < GRID_LENGTH; x++)
            {
                for (int y = 0; y < GRID_LENGTH; y++)
                {
                    if (board[y, x] == 0) //if empty
                    {
                        empties.Add(new Point(x, y));
                    }
                }
            }

            return empties;
        }

        //prints the grid in a square
        private static void PrintGrid()
        {
            string row;
            for (int x = 0; x < GRID_LENGTH; x++)
            {
                row = "";
                for (int y = 0; y < GRID_LENGTH; y++)
                {
                    row += filledGrid[x, y] + "  ";

                    if((y+1) % 3 == 0)
                    {
                        row += " ";
                    }

                }
                Console.WriteLine(row);
                if((x + 1) % 3 == 0)
                {
                    Console.WriteLine();
                }
            }
        }

        //prints the grid as an array
        private static void PrintGridAsArray()
        {
            string row;
            Console.Write("{");
            for (int x = 0; x < GRID_LENGTH; x++)
            {

                row = (x != 0) ? "\n{  " : "{ ";

                for (int y = 0; y < GRID_LENGTH; y++)
                {
                    row += filledGrid[x, y] + ", ";
                }

                    Console.Write(row.Substring(0, row.Length - 2) + " }");
                if (x != GRID_LENGTH - 1)
                {
                    Console.Write(",");
                }
            }

            Console.WriteLine("}");
            
        }

        //returns the top left corner position of the chunk of (x, y)
        private static Point GetChunkStart(int x, int y)
        {
            return new Point((x / 3) * 3, (y / 3) * 3);
        }

        public class Point
        {
            public int x;
            public int y;
            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public override string ToString()
            {
                return "(" + x + ", " + y + ")";
            }

            public override bool Equals(object obj)
            {
                Point p = (Point)obj;
                return (x == p.x) && (x == p.y);
            }
        }
    }
}
