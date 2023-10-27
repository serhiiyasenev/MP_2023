/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Threading;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        private static List<int> sharedCollection = new List<int>();
        private static readonly object lockObject = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            Thread addingThread = new Thread(AddElements);
            Thread printingThread = new Thread(PrintElements);

            addingThread.Start();
            printingThread.Start();

            addingThread.Join();
            printingThread.Join();

            Console.ReadLine();
        }

        private static void AddElements()
        {
            for (int i = 1; i <= 10; i++)
            {
                lock (lockObject)
                {
                    sharedCollection.Add(i);
                    // Signal the other thread to continue
                    Monitor.Pulse(lockObject);
                }
                Console.WriteLine($"AddElement '{i}'");
            }
        }

        private static void PrintElements()
        {
            for (int i = 0; i < 10; i++)
            {
                lock (lockObject)
                {
                    while (sharedCollection.Count <= i)
                    {
                        // Wait for the element to be added
                        Monitor.Wait(lockObject);
                    }
                    Console.WriteLine($"Elements in collection after addition {i + 1}: {string.Join(", ", sharedCollection)}");
                }
            }
        }
    }
}
