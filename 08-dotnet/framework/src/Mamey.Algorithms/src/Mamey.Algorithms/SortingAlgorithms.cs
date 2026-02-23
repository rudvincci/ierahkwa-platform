namespace Mamey.Algorithms;

public static class SortingAlgorithms
{
    /// <summary>
    /// Performs Quick Sort on an array. Best for average-case performance, not stable.
    /// Complexity: Average O(n log n), Worst O(n^2).
    /// Good for large datasets except when the pivot consistently ends up being the smallest or largest element.
    /// </summary>
    /// <typeparam name="T">Type of elements in the array, must implement IComparable.</typeparam>
    /// <param name="array">The array to be sorted.</param>
    /// <param name="left">The starting index of the array segment to sort.</param>
    /// <param name="right">The ending index of the array segment to sort.</param>
    public static void QuickSort<T>(T[] array, int left, int right) where T : IComparable<T>
    {
        if (right - left <= 10)
        {
            InsertionSort(array, left, right); // Switch to Insertion Sort for small arrays
        }
        else if (left < right)
        {
            int pivot = RandomizedPartition(array, left, right);
            QuickSort(array, left, pivot - 1);
            QuickSort(array, pivot + 1, right);
        }
    }
    
    /// <summary>
    /// Performs Merge Sort on an array. Stable and good for large datasets.
    /// Complexity: Always O(n log n).
    /// Preferred when stability is required and for linked list sorting due to its inherent recursive nature.
    /// </summary>
    /// <typeparam name="T">Type of elements in the array, must implement IComparable.</typeparam>
    /// <param name="array">The array to be sorted.</param>
    /// <param name="left">The starting index of the array segment to sort.</param>
    /// <param name="right">The ending index of the array segment to sort.</param>
    public static void MergeSort<T>(T[] array, int left, int right) where T : IComparable<T>
    {
        if (right - left <= 10)
        {
            InsertionSort(array, left, right); // Use Insertion Sort for small subarrays
        }
        else if (left < right)
        {
            int middle = left + (right - left) / 2;
            MergeSort(array, left, middle);
            MergeSort(array, middle + 1, right);
            Merge(array, left, middle, right);
        }
    }

    /// <summary>
    /// Performs Bubble Sort on an array. Simple but inefficient for large datasets.
    /// Complexity: Average and Worst O(n^2).
    /// Best used for small datasets or when the array is almost sorted, as it can terminate early.
    /// </summary>
    /// <typeparam name="T">Type of elements in the array, must implement IComparable.</typeparam>
    /// <param name="array">The array to be sorted.</param>
    public static void BubbleSort<T>(T[] array) where T : IComparable<T>
    {
        bool swapped;
        for (int i = 0; i < array.Length - 1; i++)
        {
            swapped = false;
            for (int j = 0; j < array.Length - i - 1; j++)
            {
                if (array[j].CompareTo(array[j + 1]) > 0)
                {
                    Swap(array, j, j + 1);
                    swapped = true;
                }
            }
            if (!swapped) break; // No swaps means the array is already sorted
        }
    }

    /// <summary>
    /// Performs Insertion Sort on an array. Efficient for small datasets and nearly sorted arrays.
    /// Complexity: Average and Worst O(n^2), Best O(n) for nearly sorted data.
    /// Ideal for small or partially sorted datasets due to its low overhead.
    /// </summary>
    /// <typeparam name="T">Type of elements in the array, must implement IComparable.</typeparam>
    /// <param name="array">The array to be sorted.</param>
    public static void InsertionSort<T>(T[] array, int left, int right) where T : IComparable<T>
    {
        for (int i = left + 1; i <= right; i++)
        {
            T key = array[i];
            int j = i - 1;

            // Find the position to insert using binary search
            int insertPosition = BinarySearchInsertPosition(array, key, left, j);

            while (j >= insertPosition)
            {
                array[j + 1] = array[j];
                j--;
            }
            array[j + 1] = key;
        }
    }

    /// <summary>
    /// Performs Selection Sort on an array. Simple and not stable.
    /// Complexity: O(n^2) regardless of the initial order of elements.
    /// Suitable for small arrays where simplicity is preferred and memory write is a costly operation.
    /// </summary>
    /// <typeparam name="T">Type of elements in the array, must implement IComparable.</typeparam>
    /// <param name="array">The array to be sorted.</param>
    public static void SelectionSort<T>(T[] array) where T : IComparable<T>
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            int minIndex = i;
            for (int j = i + 1; j < array.Length; j++)
            {
                if (array[j].CompareTo(array[minIndex]) < 0)
                {
                    minIndex = j;
                }
            }
            Swap(array, minIndex, i);
        }
    }

    /// <summary>
    /// Performs Heap Sort on an array. Efficient and not stable.
    /// Complexity: O(n log n).
    /// Suitable for sorting large datasets where a stable sort is not required. Often used in systems with limited memory resources.
    /// </summary>
    /// <typeparam name="T">Type of elements in the array, must implement IComparable.</typeparam>
    /// <param name="array">The array to be sorted.</param>
    public static void HeapSort<T>(T[] array) where T : IComparable<T>
    {
        int n = array.Length;

        // Build a heap
        for (int i = n / 2 - 1; i >= 0; i--)
            Heapify(array, n, i);

        // Extract elements from heap one by one
        for (int i = n - 1; i > 0; i--)
        {
            Swap(array, 0, i);
            Heapify(array, i, 0);
        }
    }

    /// <summary>
    /// Performs Shell Sort on an array. A variation of Insertion Sort with better performance.
    /// Complexity: Depends on gap sequence, worst-case O(n^2), can be much less depending on the gap sequence.
    /// Good for medium-sized arrays and benefits from its adaptive nature, especially with a good choice of gap sequence.
    /// </summary>
    /// <typeparam name="T">Type of elements in the array, must implement IComparable.</typeparam>
    /// <param name="array">The array to be sorted.</param>
    public static void ShellSort<T>(T[] array) where T : IComparable<T>
    {
        int n = array.Length;
        for (int gap = n / 2; gap > 0; gap /= 2)
        {
            for (int i = gap; i < n; i++)
            {
                T temp = array[i];
                int j;
                for (j = i; j >= gap && array[j - gap].CompareTo(temp) > 0; j -= gap)
                {
                    array[j] = array[j - gap];
                }
                array[j] = temp;
            }
        }
    }

    /// <summary>
    /// Performs Radix Sort on an array of integers. Non-comparative and stable.
    /// Complexity: O(nk) for n keys which have k digits.
    /// Best used for sorting integers, especially when the range of array elements is not significantly greater than the number of elements.
    /// </summary>
    /// <param name="array">The array of integers to be sorted.</param>
    public static void RadixSort(int[] array)
    {
        int max = FindMax(array);
        int[] aux = new int[array.Length]; // Single auxiliary array for all digit sorts

        for (int exp = 1; max / exp > 0; exp *= 10)
        {
            CountSort(array, aux, exp);
        }
    }

    /// <summary>
    /// Performs Radix Sort on an array of floating-point numbers.
    /// Complexity: O(nk) for n keys which have k digits.
    /// Suitable for datasets where floating-point numbers can be normalized without losing precision.
    /// </summary>
    /// <param name="array">The array of floating-point numbers to be sorted.</param>
    public static void RadixSortFloats(float[] array)
    {
        // Determine the multiplication factor to normalize decimal numbers
        float max = FindMaxFloat(array);
        float factor = (float)Math.Pow(10, Math.Ceiling(Math.Log10(max)));

        // Normalize the numbers and convert them to integers
        int[] intArray = new int[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            intArray[i] = (int)(array[i] * factor);
        }

        // Apply Radix Sort on the integer array
        RadixSort(intArray);

        // Convert back to the original float array
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (float)intArray[i] / factor;
        }
    }

    /// <summary>
    /// Performs Radix Sort on an array of strings. Non-comparative.
    /// Complexity: O(nk) for n strings with maximum length k.
    /// Ideal for sorting strings when all strings are of the same length or when varying lengths are properly handled.
    /// </summary>
    /// <param name="array">The array of strings to be sorted.</param>
    public static void RadixSortStrings(string[] array)
    {
        int maxLen = FindMaxLength(array);

        for (int exp = maxLen - 1; exp >= 0; exp--)
        {
            CountSortString(array, exp);
        }
    }

    /// <summary>
    /// Performs Radix Sort on an array of double numbers.
    /// Complexity: O(nk) for n keys which have k digits.
    /// Effective for datasets where double values can be normalized without significant precision loss.
    /// </summary>
    /// <param name="array">The array of double numbers to be sorted.</param>
    public static void RadixSortDoubles(double[] array)
    {
        // Determine the multiplication factor to normalize decimal numbers
        double max = FindMaxDouble(array);
        double factor = Math.Pow(10, Math.Ceiling(Math.Log10(max)));

        // Normalize the numbers and convert them to integers
        long[] intArray = new long[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            intArray[i] = (long)(array[i] * factor);
        }

        // Apply Radix Sort on the integer array
        RadixSortLong(intArray);

        // Convert back to the original double array
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (double)intArray[i] / factor;
        }
    }

    /// <summary>
    /// Performs Radix Sort on an array of long integers. Non-comparative and stable.
    /// Complexity: O(nk) for n keys which have k digits.
    /// Best used for sorting long integers when the range of array elements is not significantly greater than the number of elements.
    /// </summary>
    /// <param name="array">The array of long integers to be sorted.</param>
    public static void RadixSortLong(long[] array)
    {
        long max = FindMaxLong(array);

        for (long exp = 1; max / exp > 0; exp *= 10)
        {
            CountSortLong(array, exp);
        }
    }

    #region HelperMethods
    private static int RandomizedPartition<T>(T[] array, int left, int right) where T : IComparable<T>
    {
        Random random = new Random();
        int randomPivotIndex = left + random.Next(right - left);
        Swap(array, randomPivotIndex, right); // Swap random pivot with the last element
        return Partition(array, left, right);
    }
    private static int Partition<T>(T[] array, int left, int right) where T : IComparable<T>
    {
        T pivot = array[right];
        int i = left - 1;

        for (int j = left; j < right; j++)
        {
            if (array[j].CompareTo(pivot) < 0)
            {
                i++;
                Swap(array, i, j);
            }
        }

        Swap(array, i + 1, right);
        return i + 1;
    }
    private static void Swap<T>(T[] array, int index1, int index2)
    {
        T temp = array[index1];
        array[index1] = array[index2];
        array[index2] = temp;
    }
    private static int BinarySearchInsertPosition<T>(T[] array, T key, int left, int right) where T : IComparable<T>
    {
        while (left <= right)
        {
            int middle = left + (right - left) / 2;
            if (array[middle].CompareTo(key) < 0)
                left = middle + 1;
            else
                right = middle - 1;
        }
        return left;
    }
    private static void Merge<T>(T[] array, int left, int middle, int right) where T : IComparable<T>
    {
        int n1 = middle - left + 1;
        int n2 = right - middle;
        T[] L = new T[n1];
        T[] R = new T[n2];

        Array.Copy(array, left, L, 0, n1);
        Array.Copy(array, middle + 1, R, 0, n2);

        int i = 0, j = 0, k = left;
        while (i < n1 && j < n2)
        {
            if (L[i].CompareTo(R[j]) <= 0)
            {
                array[k] = L[i];
                i++;
            }
            else
            {
                array[k] = R[j];
                j++;
            }
            k++;
        }

        while (i < n1)
        {
            array[k] = L[i];
            i++;
            k++;
        }

        while (j < n2)
        {
            array[k] = R[j];
            j++;
            k++;
        }
    }
    private static void Heapify<T>(T[] array, int n, int i) where T : IComparable<T>
    {
        int largest = i;
        int left = 2 * i + 1;
        int right = 2 * i + 2;

        if (left < n && array[left].CompareTo(array[largest]) > 0)
            largest = left;

        if (right < n && array[right].CompareTo(array[largest]) > 0)
            largest = right;

        if (largest != i)
        {
            Swap(array, i, largest);
            Heapify(array, n, largest);
        }
    }
    private static void CountSort(int[] array, int[] aux, int exp)
    {
        int n = array.Length;
        int[] count = new int[10]; // There are 10 possible digits (0-9)

        Array.Clear(count, 0, count.Length);

        for (int i = 0; i < n; i++)
        {
            count[(array[i] / exp) % 10]++;
        }

        for (int i = 1; i < 10; i++)
        {
            count[i] += count[i - 1];
        }

        for (int i = n - 1; i >= 0; i--)
        {
            int index = (array[i] / exp) % 10;
            aux[count[index] - 1] = array[i];
            count[index]--;
        }

        Array.Copy(aux, array, n); // Copy back to the original array
    }
    private static int FindMax(int[] array)
    {
        int max = array[0];
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] > max)
            {
                max = array[i];
            }
        }
        return max;
    }
    private static float FindMaxFloat(float[] array)
    {
        float max = array[0];
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] > max)
            {
                max = array[i];
            }
        }
        return max;
    }
    private static void CountSortString(string[] array, int exp)
    {
        int n = array.Length;
        string[] output = new string[n];
        int[] count = new int[256]; // ASCII character set

        Array.Clear(count, 0, count.Length);

        foreach (string str in array)
        {
            int index = exp < str.Length ? (int)str[exp] : 0;
            count[index]++;
        }

        for (int i = 1; i < 256; i++)
        {
            count[i] += count[i - 1];
        }

        for (int i = n - 1; i >= 0; i--)
        {
            int index = exp < array[i].Length ? (int)array[i][exp] : 0;
            output[count[index] - 1] = array[i];
            count[index]--;
        }

        Array.Copy(output, array, n);
    }
    private static int FindMaxLength(string[] array)
    {
        int maxLength = 0;
        foreach (string str in array)
        {
            if (str.Length > maxLength)
            {
                maxLength = str.Length;
            }
        }
        return maxLength;
    }
    private static double FindMaxDouble(double[] array)
    {
        double max = array[0];
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] > max)
            {
                max = array[i];
            }
        }
        return max;
    }
    private static void CountSortLong(long[] array, long exp)
    {
        int n = array.Length;
        long[] output = new long[n];
        int[] count = new int[10];

        Array.Clear(count, 0, count.Length);

        for (int i = 0; i < n; i++)
        {
            count[(int)((array[i] / exp) % 10)]++;
        }

        for (int i = 1; i < 10; i++)
        {
            count[i] += count[i - 1];
        }

        for (int i = n - 1; i >= 0; i--)
        {
            int index = (int)((array[i] / exp) % 10);
            output[count[index] - 1] = array[i];
            count[index]--;
        }

        Array.Copy(output, array, n);
    }
    private static long FindMaxLong(long[] array)
    {
        long max = array[0];
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] > max)
            {
                max = array[i];
            }
        }
        return max;
    }
    #endregion
}
