/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
    class Program
    {
        private static readonly Random Random = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
            Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
            Console.WriteLine("First Task – creates an array of 10 random integer.");
            Console.WriteLine("Second Task – multiplies this array with another random integer.");
            Console.WriteLine("Third Task – sorts this array by ascending.");
            Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
            Console.WriteLine();

            Task<int[]> firstTask = Task.Factory.StartNew(CreateRandomArray);
            Task<int[]> secondTask = firstTask.ContinueWith(t => MultiplyArray(t.Result));
            Task<int[]> thirdTask = secondTask.ContinueWith(t => SortArray(t.Result));
            Task fourthTask = thirdTask.ContinueWith(t => CalculateAverage(t.Result));

            Console.ReadLine();
        }

        static int[] CreateRandomArray()
        {
            var array = new int[10];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Random.Next(0, 100);
            }
            Console.WriteLine($"Created array: {string.Join(", ", array)}");
            return array;
        }

        static int[] MultiplyArray(int[] array)
        {
            int multiplier = Random.Next(1, 10);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] *= multiplier;
            }
            Console.WriteLine($"Array after multiplication by {multiplier}: {string.Join(", ", array)}");
            return array;
        }

        static int[] SortArray(int[] array)
        {
            Array.Sort(array);
            Console.WriteLine($"Sorted array: {string.Join(", ", array)}");
            return array;
        }

        static void CalculateAverage(int[] array)
        {
            double average = array.Average();
            Console.WriteLine($"Average value: {average}");
        }
    }
}