namespace Mamey.Algorithms;

public class TreeTrieNode<T>
{
    public Dictionary<string, TreeTrieNode<T>> Children { get; private set; }
    public T Value { get; set; } // Generic value stored in the node
    public bool IsEnd { get; set; } // Marks if the node is the end of a valid entry

    public TreeTrieNode()
    {
        Children = new Dictionary<string, TreeTrieNode<T>>();
        IsEnd = false;
    }
}

public class TreeTrie<T>
{
    private TreeTrieNode<T> root;

    public TreeTrie()
    {
        root = new TreeTrieNode<T>();
    }

    // Insert or update a hierarchical key with a value
    public bool Upsert(string key, T value)
    {
        var parts = key.Split('.'); // Split by dot (can be customized)
        TreeTrieNode<T> current = root;

        foreach (var part in parts)
        {
            if (!current.Children.ContainsKey(part))
            {
                current.Children[part] = new TreeTrieNode<T>();
            }
            current = current.Children[part];
        }

        current.IsEnd = true; // Mark the node as a valid end node
        current.Value = value; // Upsert the value
        return true; // Upsert always succeeds
    }

    // Delete a key from the TreeTrie
    public bool Delete(string key)
    {
        var parts = key.Split('.');
        return DeleteHelper(root, parts, 0);
    }

    // Helper function for recursive delete
    private bool DeleteHelper(TreeTrieNode<T> current, string[] parts, int index)
    {
        if (index == parts.Length)
        {
            if (!current.IsEnd)
            {
                return false; // Key does not exist
            }

            current.IsEnd = false; // Unmark this as an end node
            return current.Children.Count == 0; // Return true if it has no children (ready to delete)
        }

        string part = parts[index];
        if (!current.Children.ContainsKey(part))
        {
            return false; // Key does not exist
        }

        bool shouldDeleteCurrentNode = DeleteHelper(current.Children[part], parts, index + 1);

        // If true is returned and the current node has no other children, delete it
        if (shouldDeleteCurrentNode)
        {
            current.Children.Remove(part);
            return current.Children.Count == 0 && !current.IsEnd;
        }

        return false;
    }

    // Insert a new hierarchical key into the TreeTrie
    public bool Insert(string key, T value)
    {
        var parts = key.Split('.');
        TreeTrieNode<T> current = root;

        foreach (var part in parts)
        {
            if (!current.Children.ContainsKey(part))
            {
                current.Children[part] = new TreeTrieNode<T>();
            }
            current = current.Children[part];
        }

        if (current.IsEnd)
        {
            return false; // Key already exists
        }

        current.IsEnd = true;
        current.Value = value;
        return true; // Successfully inserted
    }

    // Search for a key and return the associated value
    public bool Search(string key, out T value)
    {
        var parts = key.Split('.');
        TreeTrieNode<T> current = root;

        foreach (var part in parts)
        {
            if (!current.Children.ContainsKey(part))
            {
                value = default(T);
                return false;
            }
            current = current.Children[part];
        }

        if (current.IsEnd)
        {
            value = current.Value;
            return true;
        }

        value = default(T);
        return false;
    }

    // Check if a key is available
    public bool IsAvailable(string key)
    {
        var parts = key.Split('.');
        TreeTrieNode<T> current = root;

        foreach (var part in parts)
        {
            if (!current.Children.ContainsKey(part))
            {
                return true; // Key is available
            }
            current = current.Children[part];
        }

        return !current.IsEnd; // Available if it's not a valid end node
    }

    // Display the entire hierarchy
    public void Display()
    {
        DisplayHelper(root, "");
    }

    private void DisplayHelper(TreeTrieNode<T> node, string prefix)
    {
        foreach (var part in node.Children.Keys)
        {
            var newPrefix = string.IsNullOrEmpty(prefix) ? part : $"{prefix}.{part}";
            Console.WriteLine(newPrefix);
            DisplayHelper(node.Children[part], newPrefix);
        }
    }
}
