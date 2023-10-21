/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        private const int MaxThreads = 10;
        private static Semaphore _semaphore = new Semaphore(0, MaxThreads);

        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            Console.WriteLine();

            Console.WriteLine("Using Thread class:");
            Thread thread = new Thread(ThreadFunction);
            thread.Start(MaxThreads);
            thread.Join();

            Console.WriteLine();

            Console.WriteLine("Using ThreadPool:");
            ThreadPool.QueueUserWorkItem(ThreadPoolFunction, MaxThreads);
            _semaphore.WaitOne();

            Console.ReadLine();
        }

        static void ThreadFunction(object state)
        {
            int number = (int)state;

            Console.WriteLine($"Thread number: {number}");

            if (number > 1)
            {
                Thread thread = new Thread(ThreadFunction);
                thread.Start(number - 1);
                thread.Join();
            }
        }

        static void ThreadPoolFunction(object state)
        {
            int number = (int)state;

            Console.WriteLine($"Thread number: {number}");

            if (number > 1)
            {
                ThreadPool.QueueUserWorkItem(ThreadPoolFunction, number - 1);
            }
            else
            {
                _semaphore.Release();
            }
        }
    }
}
