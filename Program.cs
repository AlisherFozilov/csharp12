using System;
using System.Threading;
using System.Threading.Tasks;

namespace matrix
{
    internal static class Program
    {
        private static void Main()
        {
            Multithreading.UsingThreads();
            Multithreading.UsingTasks();
            Multithreading.UsingParallel();
            
            return;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.CursorVisible = false;
            var width = 100;
            var m = new Matrix(100, width, 20, 25);
            var r = new Random();
            for (var i = 0; i < 100; i++)
            {
                var startColumn = r.Next(0, width);
                Task.Factory.StartNew(() => m.SpawnFallingSymbols(startColumn));
                Task.Factory.StartNew(() => m.SpawnFallingSymbols(startColumn + 1));
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(3000);
                    Task.Factory.StartNew(() => m.SpawnFallingSymbols(startColumn));
                });
            }

            // return;
            Thread.Sleep(1_000_000);
        }
    }

    internal class Matrix
    {
        private int Width { get; set; }

        private int Speed { get; set; } // lower - faster
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

        public void SpawnFallingSymbols(int startColumn)
        {
            var r = new Random();
            var length = r.Next(10, Length);

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

    static class Multithreading
    {
        // Многопоточное программирование
        public static void UsingThreads()
        {
            var t = new Thread(() =>
            {
                Thread.Sleep(10);
                Console.WriteLine("Hello threads");
            });
            t.Start();
            Console.WriteLine("waiting for a thread to complete");
            t.Join();
            Console.WriteLine("finished");
        }

        // Асинхронное программирование
        public static void UsingTasks()
        {
            var t = new Task(() =>
            {
                Thread.Sleep(10);
                Console.WriteLine("Hello task");
            });
            t.Start();
            Console.WriteLine("waiting for a task to complete");
            t.Wait();
            Console.WriteLine("finished");
        }

        // Параллельное программирование
        public static void UsingParallel()
        {
            var sum = 0;
            Parallel.For(1, 100, num => Interlocked.Add(ref sum, num));
            Console.WriteLine(sum);
        }
    }
}