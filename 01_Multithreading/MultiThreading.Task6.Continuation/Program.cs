/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");

            // a. Continuation task should be executed regardless of the result of the parent task.
            var taskA = Task.Run(() =>
            {
                Console.WriteLine("Task A started");
            }).ContinueWith(prevTask =>
            {
                Console.WriteLine("Continuation for Task A (executed regardless of the result)");
            });

            // b. Continuation task should be executed when the parent task finished without success.
            var taskB = Task.Run(() =>
            {
                Console.WriteLine("Task B started");
                throw new Exception("Task B exception");
            }).ContinueWith(prevTask =>
            {
                Console.WriteLine("Continuation for Task B (executed due to task failure)");
            }, TaskContinuationOptions.OnlyOnFaulted);

            // c. Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.
            var taskC = Task.Run(() =>
            {
                Console.WriteLine("Task C started");
                throw new Exception("Task C exception");
            }).ContinueWith(prevTask =>
            {
                Console.WriteLine("Continuation for Task C (executed on the same thread due to task failure)");
            }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

            // d. Continuation task should be executed outside of the thread pool when the parent task would be cancelled.
            var cts = new CancellationTokenSource();
            var taskD = Task.Run(() =>
            {
                Console.WriteLine("Task D started");
                cts.Token.ThrowIfCancellationRequested();
            }, cts.Token).ContinueWith(prevTask =>
            {
                Console.WriteLine("Continuation for Task D (executed outside of the thread pool due to task cancellation)");
            }, TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.LongRunning);

            cts.Cancel();

            // Wait for tasks to complete before ending the program.
            try
            {
                Task.WaitAll(taskA, taskB, taskC, taskD);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Catch the aggregate exceptions for the purpose of this demo.");
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Console.WriteLine();
            }

            Console.WriteLine("All tasks completed");
            Console.ReadLine();
        }
    }
}