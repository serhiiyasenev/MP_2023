/*
* Study the code of this application to calculate the sum of integers from 0 to N, and then
* change the application code so that the following requirements are met:
* 1. The calculation must be performed asynchronously.
* 2. N is set by the user from the console. The user has the right to make a new boundary in the calculation process,
* which should lead to the restart of the calculation.
* 3. When restarting the calculation, the application should continue working without any failures.
*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Task1.CancellationTokens;

internal class Program
{
    private static CancellationTokenSource _cts = new CancellationTokenSource();
    private static Task<long> _currentCalculationTask;

    /// <summary>
    /// The Main method should not be changed at all.
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        Console.WriteLine("Mentoring program L2. Async/await.V1. Task 1");
        Console.WriteLine("Calculating the sum of integers from 0 to N.");
        Console.WriteLine("Use 'q' key to exit...");
        Console.WriteLine();

        Console.WriteLine("Enter N: ");

        var input = Console.ReadLine();
        while (input.Trim().ToUpper() != "Q")
        {
            if (int.TryParse(input, out var n))
            {
                CalculateSum(n);
            }
            else
            {
                Console.WriteLine($"Invalid integer: '{input}'. Please try again.");
                Console.WriteLine("Enter N: ");
            }

            input = Console.ReadLine();
        }

        Console.WriteLine("Press any key to continue");
        Console.ReadLine();
    }

    private static async void CalculateSum(int n)
    {
        _cts.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            await Console.Out.WriteLineAsync($"The task for {n} started... Enter N to cancel the request:");
            _currentCalculationTask = Task.Run(() => Calculator.Calculate(n, _cts.Token));
            var sum = await _currentCalculationTask;
            await Console.Out.WriteLineAsync($"Sum for {n} = {sum}.");
        }
        catch (OperationCanceledException)
        {
            await Console.Out.WriteLineAsync($"Sum for {n} cancelled...");
        }
        catch (OverflowException e)
        {
            await Console.Out.WriteLineAsync(e.Message);
        }
        finally
        {
            await Console.Out.WriteLineAsync();
            await Console.Out.WriteLineAsync("Enter N: ");
        }
    }
}
