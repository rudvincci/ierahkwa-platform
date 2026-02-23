namespace Mamey.ApplicationName.BlazorWasm.Services;

public static class MockService
{

    private static List<string> GetRandomItems(string[] items, Random random, int maxCount = 3)
    {
        var selectedItems = new List<string>();
        int count = random.Next(1, maxCount + 1);
        for (int i = 0; i < count; i++)
        {
            selectedItems.Add(items[random.Next(items.Length)]);
        }
        return selectedItems;
    }
}