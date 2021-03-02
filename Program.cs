using System;
using System.Threading;
using System.Threading.Tasks;

namespace matrix
{
    internal static class Program
    {
        private static void Main()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.CursorVisible = false;
            var m = new Matrix(100, 100, 10, 25);

            for (var i = 0; i < 1000; i++)
            {
                Task.Factory.StartNew(m.SpawnFallingSymbols);
            }

            Thread.Sleep(1_000_000);
        }
    }

    internal class Matrix
    {
        private int Width { get; set; }

        private int Speed { get; set; }
        private int Length { get; set; }

        private int Height { get; set; }

        public Matrix(int height, int width, int speed, int length)
        {
            Width = width;
            Speed = speed;
            Length = length;
            Height = height;
        }

        private readonly object _cursorLocker = new();
        private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private static readonly Random Rand = new();

        private static string GetString()
        {
            return $"{Letters[Rand.Next(0, 35)]}";
        }

        public void SpawnFallingSymbols()
        {
            var r = new Random();
            var length = r.Next(10, Length);
            var startColumn = r.Next(0, Width);

            for (var startRow = 2; startRow < Height; startRow++)
            {
                lock (_cursorLocker)
                {
                    // remove first symbol
                    Console.SetCursorPosition(startColumn, startRow - 1);
                    Console.Write(" ");
                    Console.SetCursorPosition(startColumn - 1, startRow - 1);
                }

                // Thread.Sleep(100);
                Print(length, startRow, startColumn);
            }
        }

        private void Print(int length, int startRow, int startColumn)
        {
            var endRow = length + startRow;
            int i;
            for (i = startRow; i < endRow - 2; i++)
            {
                lock (_cursorLocker)
                {
                    Console.SetCursorPosition(startColumn, i);
                    Console.Write(GetString());
                }

                Thread.Sleep(Speed);
            }

            lock (_cursorLocker)
            {
                Console.SetCursorPosition(startColumn, i);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(GetString());
            }

            Thread.Sleep(Speed);

            lock (_cursorLocker)
            {
                Console.SetCursorPosition(startColumn, i + 1);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(GetString());

                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }

            Thread.Sleep(Speed);
        }
    }
}

/*
 * public void Капелька()
        {
            var r = new Random();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            var length = 2;//r.Next(3, Length);
            var startColumn = r.Next(0, Width);

            for (var startRow = 2; startRow < 50; startRow++)
            {
                lock (_cursorLocker)
                {
                    // remove first symbol
                    Console.CursorTop = startRow - 1;
                    Console.CursorLeft = startColumn;

                    Console.Write(" ");
                    Console.CursorLeft--;

                    Print(length, startRow);
                }
            }
        }

        private static void Print(int length, int startRow)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            var endRow = length + startRow;
            for (var i = startRow; i < endRow - 2; i++)
            {
                Console.CursorTop = i;
                Console.Write("*");
                Console.CursorLeft--;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("*");
            Console.CursorLeft--;
            Console.CursorTop++;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("*");
            Console.CursorLeft--;
            Console.CursorTop++;
        }
 */