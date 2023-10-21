/*
 * 3. Write a program, which multiplies two matrices and uses class Parallel.
 * a. Implement logic of MatricesMultiplierParallel.cs
 *    Make sure that all the tests within MultiThreading.Task3.MatrixMultiplier.Tests.csproj run successfully.
 * b. Create a test inside MultiThreading.Task3.MatrixMultiplier.Tests.csproj to check which multiplier runs faster.
 *    Find out the size which makes parallel multiplication more effective than the regular one.
 */

using System;
using MultiThreading.Task3.MatrixMultiplier.Matrices;
using MultiThreading.Task3.MatrixMultiplier.Multipliers;

namespace MultiThreading.Task3.MatrixMultiplier
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("3. Multiplies two matrices and uses class Parallel.");
            Console.WriteLine();

            byte[] matrixSize = {3, 7, 10, 15, 50, 100 };
            foreach (var matrix in matrixSize)
            {
                CreateAndProcessMatrices(matrix);
            }
            Console.ReadLine();
        }

        private static void CreateAndProcessMatrices(byte sizeOfMatrix)
        {
            Console.WriteLine("Multiplying...");
            var firstMatrix = new Matrix(sizeOfMatrix, sizeOfMatrix, true);
            var secondMatrix = new Matrix(sizeOfMatrix, sizeOfMatrix, true);

            IMatrix resultMatrix = new MatricesMultiplier().Multiply(firstMatrix, secondMatrix);
            IMatrix resultMatrixParallel = new MatricesMultiplierParallel().Multiply(firstMatrix, secondMatrix);

            Console.WriteLine($"firstMatrix size '{sizeOfMatrix}':");
            firstMatrix.Print();
            Console.WriteLine($"secondMatrix size '{sizeOfMatrix}':");
            secondMatrix.Print();

            Console.WriteLine($"resultMatrix size '{sizeOfMatrix}':");
            resultMatrix.Print();
            Console.WriteLine($"resultMatrixParallel size '{sizeOfMatrix}':");
            resultMatrixParallel.Print();
        }
    }
}
