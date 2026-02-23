namespace Mamey.Algorithms;

public static class SearchAlgorithms
{
    /// <summary>
    /// Performs a recursive Binary Search on a sorted array.
    /// Complexity: O(log n).
    /// Preferred for its readability and simplicity, especially in scenarios where stack depth is not a concern.
    /// </summary>
    /// <typeparam name="T">Type of elements in the array, must implement IComparable.</typeparam>
    /// <param name="array">The sorted array to search in.</param>
    /// <param name="key">The key to search for.</param>
    /// <param name="left">The starting index of the search range.</param>
    /// <param name="right">The ending index of the search range.</param>
    /// <returns>Index of the key if found, otherwise -1.</returns>
    public static int BinarySearch<T>(T[] array, T key, int left, int right) where T : IComparable<T>
    {
        if (right < left)
            return -1;

        int mid = left + (right - left) / 2;
        int result = key.CompareTo(array[mid]);

        if (result == 0)
            return mid;

        if (result > 0)
            return BinarySearch(array, key, mid + 1, right);
        else
            return BinarySearch(array, key, left, mid - 1);
    }
    /// <summary>
    /// Performs Linear Search on a potentially sorted array with an early exit.
    /// Complexity: O(n) in the worst case, potentially better if the array is partially sorted.
    /// Ideal for small or unsorted datasets, or when there's a high chance of finding the key early.
    /// </summary>
    /// <typeparam name="T">Type of elements in the array, must implement IComparable.</typeparam>
    /// <param name="array">The array to search in.</param>
    /// <param name="key">The key to search for.</param>
    /// <returns>Index of the key if found, otherwise -1.</returns>
    public static int LinearSearchSorted<T>(T[] array, T key) where T : IComparable<T>
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(key))
                return i;
            if (array[i].CompareTo(key) > 0)
                break; // Early exit if current element is greater than the key
        }
        return -1;
    }
    /// <summary>
    /// Performs Breadth-First Search (BFS) on a graph with path tracking.
    /// Complexity: O(V + E) for graph with V vertices and E edges.
    /// Ideal for finding the shortest path in unweighted graphs.
    /// </summary>
    /// <param name="graph">The graph represented as an adjacency list.</param>
    /// <param name="start">The starting node index.</param>
    /// <returns>Dictionary mapping each node to its parent in the BFS tree.</returns>
    public static Dictionary<int, int> BreadthFirstSearchWithPath(Dictionary<int, List<int>> graph, int start)
    {
        var visited = new HashSet<int>();
        var queue = new Queue<int>();
        var paths = new Dictionary<int, int>(); // Tracks the parent of each node

        queue.Enqueue(start);
        visited.Add(start);
        paths[start] = -1; // Start node has no parent

        while (queue.Count > 0)
        {
            int node = queue.Dequeue();

            foreach (var neighbor in graph[node])
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    paths[neighbor] = node; // Set the parent of the neighbor
                }
            }
        }

        return paths;
    }
    /// <summary>
    /// Performs a recursive Depth-First Search (DFS) on a graph.
    /// Complexity: O(V + E) for graph with V vertices and E edges.
    /// Ideal for situations where we need to explore as far as possible along each branch before backtracking.
    /// </summary>
    /// <param name="graph">The graph represented as an adjacency list.</param>
    /// <param name="start">The starting node index.</param>
    /// <param name="visited">Keeps track of visited nodes.</param>
    /// <returns>List of nodes in the order they were visited.</returns>
    public static List<int> DepthFirstSearchRecursive(Dictionary<int, List<int>> graph, int start, HashSet<int>? visited = null)
    {
        if (visited == null)
            visited = new HashSet<int>();

        visited.Add(start);

        // This list will record the visitation order
        var visitOrder = new List<int> { start };

        foreach (var neighbor in graph[start])
        {
            if (!visited.Contains(neighbor))
            {
                visitOrder.AddRange(DepthFirstSearchRecursive(graph, neighbor, visited));
            }
        }
        return visitOrder;
    }
}
