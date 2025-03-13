internal class Program
{
    // Directions: Up, Right, Down, Left (clockwise)
    private static (int dx, int dy)[] directions = { (-1, 0), (0, 1), (1, 0), (0, -1) };

    // Simulate the guard's path and return the visited positions and directions
    public static HashSet<(int, int, int)> SimulateGuard(string[] map, int startX, int startY, int startDirection)
    {
        HashSet<(int, int, int)> visited = new();
        int guardX = startX, guardY = startY, facing = startDirection;
        int rows = map.Length, cols = map[0].Length;

        visited.Add((guardX, guardY, facing)); // Add the starting position and direction

        while (true)
        {
            // Calculate the next position the guard will move to
            int nextX = guardX + directions[facing].dx;
            int nextY = guardY + directions[facing].dy;

            // Check if the guard is out of bounds
            if (nextX < 0 || nextX >= rows || nextY < 0 || nextY >= cols)
            {
                // If the guard moves out of bounds, the simulation ends
                break;
            }

            // Check if there's an obstruction in the next position
            if (map[nextX][nextY] == '#')
            {
                // Turn right (90 degrees) if there's an obstruction
                facing = (facing + 1) % 4;
            }
            else
            {
                // Move forward if no obstruction
                guardX = nextX;
                guardY = nextY;

                // Add the current position and direction to the visited set
                if (!visited.Add((guardX, guardY, facing)))
                {
                    break; // Stuck, loop detected
                }
            }
        }

        return visited;
    }

    // Function to calculate how many valid positions for the new obstruction
    public static int GetValidObstructionPositions(string[] input)
    {
        int rows = input.Length;
        int cols = input[0].Length;

        // Find the guard's initial position and facing direction
        int guardX = 0, guardY = 0, facing = 0;
        for (int i = 0; i < rows; i++)
        {
            int col = input[i].IndexOf('^');
            if (col != -1)
            {
                guardX = i;
                guardY = col;
                facing = 0; // Initially facing up
                break;
            }
        }

        // Simulate the original patrol path and mark all visited positions and directions
        HashSet<(int, int, int)> visitedPositions = SimulateGuard(input, guardX, guardY, facing);

        // Now check every potential position for a new obstruction
        int validPositionsCount = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // Don't place an obstruction where the guard starts or where there's already an obstruction
                if ((i == guardX && j == guardY) || input[i][j] == '#')
                    continue;

                // Create a modified map with the potential obstruction
                char[] modifiedRow = input[i].ToCharArray();
                modifiedRow[j] = '#';
                string[] modifiedMap = (string[])input.Clone();
                modifiedMap[i] = new string(modifiedRow);

                // Simulate the guard's movement with the new obstruction
                HashSet<(int, int, int)> modifiedVisited = SimulateGuard(modifiedMap, guardX, guardY, facing);

                // If the modified path causes a loop (i.e., fewer positions are visited), count the position
                if (modifiedVisited.Count < visitedPositions.Count)
                {
                    validPositionsCount++;
                }
            }
        }

        return validPositionsCount;
    }

    private static void Main()
    {
        // Sample input map
        string[] input =
        {
            "....#.....",
            ".........#",
            "..........",
            "..#.......",
            ".......#..",
            "..........",
            ".#..^.....",
            "........#.",
            "#.........",
            "......#..."
        };

        // Get the number of valid positions for the new obstruction
        int result = GetValidObstructionPositions(input);

        // Output the result
        Console.WriteLine($"Number of valid positions for the new obstruction: {result}");

        // Wait for user input before closing
        Console.ReadLine();
    }
}