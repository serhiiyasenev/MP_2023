namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            int[][] nodes = [[5], [7, 3], [6, 8, 10], [12, 9, 13, 16]];
            var result = Recommend(nodes);
            Console.WriteLine($"Maximum path sum is: {result}");
            Console.ReadKey();
        }

        static int Recommend(int[][] nodes)
        {
            if (nodes == null || nodes.Length == 0) return 0;

            int[] maxSum = (int[])nodes[nodes.Length - 1].Clone();

            for (int layer = nodes.Length - 2; layer >= 0; layer--)
            {
                for (int i = 0; i <= layer; i++)
                {
                    maxSum[i] = nodes[layer][i] + Math.Max(maxSum[i], maxSum[i + 1]);
                }
            }

            return maxSum[0];
        }
    }
}
