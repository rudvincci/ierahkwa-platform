namespace Mamey.Constants
{
    public static class Names
    {
        public static readonly string[] LastNames = new[]
        {
            "Smith", "Johnson", "Martinez", "Brown", "Garcia-Lopez", "Rodriguez-Gonzalez", "Davis", "Hernandez-Santiago",
            "Miller", "Martinez-Ruiz", "Martinez-Santos", "Garcia", "Anderson", "Taylor", "Wilson", "Perez-Martinez",
            "Lee", "Walker", "Allen", "Scott", "Carter", "Mitchell", "Sanchez-Rodriguez", "Ramirez-Lopez"
        };
        public static readonly string[] NativeNames = new[]
        {
            "Waya", "Takwila", "Mika", "", "Cheveyo", "Peta", "Tasunke", "Ahyoka", "Chaska", "Hopi",
            "Kiona", "Tiva", "Kosumi", "Sahale", "Awan", "Yuma", "", "Tala", "Nootau", "Misu",
            "Adsila", "Kele", "Kohana", "Takoda"
        };
        public static readonly string[] Nicknames = new[]
        {
            "Ace", "Rocky", "", "Buddy", "Champ", "", "Buzz", "Tex", "Scout", "", "Red", "Jet", "Hawk",
            "Dash", "Taz", "Ziggy", "", "Tank", "Rex", "Flash", "", "Duke", "Rusty", "Shadow"
        };

        public static readonly string[] MaleFirstNames = new[]
        {
            "Liam", "Noah", "James", "Oliver", "Elijah", "William", "Benjamin", "Lucas", "Henry", "Alexander",
            "Michael", "Mason", "Ethan", "Logan", "Jackson", "Sebastian", "Aiden", "Matthew", "Samuel", "David",
            "Joseph", "Daniel", "Jack", "Owen"
        };
        public static readonly string[] MaleMiddleNames = new[]
        {
            "James", "Michael", "Alexander", "Christopher", "David", "Joseph", "Robert", "Edward", "John", "Thomas",
            "William", "Anthony", "Charles", "Andrew", "Matthew", "Steven", "Daniel", "Scott", "Patrick", "Lee",
            "Ryan", "Paul", "Henry", "George"
        };
        public static readonly string[] MaleNicknames = new[]
        {
            "Mike", "Sam", "Max", "Charlie", "Alex", "Nate", "Ben", "Tommy", "Jack", "Tony",
            "Danny", "Billy", "Chris", "Robby", "Andy", "Leo", "Jay", "Eddie", "Frank", "Matt",
            "Joey", "Zack", "Will", "Tim"
        };

        public static readonly string[] FemaleFirstNames = new[]
        {
            "Olivia", "Emma", "Ava", "Sophia", "Isabella", "Mia", "Amelia", "Charlotte", "Harper", "Evelyn",
            "Abigail", "Emily", "Elizabeth", "Sofia", "Avery", "Ella", "Scarlett", "Grace", "Chloe", "Lily",
            "Hannah", "Layla", "Zoe", "Riley"
        };
        public static readonly string[] FemaleMiddleNames = new[]
        {
            "Marie", "Rose", "Grace", "Elizabeth", "Ann", "Mae", "Louise", "Lynn", "Jane", "Marie-Claire",
            "Renee", "Faith", "Hope", "Joy", "Nicole", "Leigh", "Paige", "Kate", "Brooke", "Skye",
            "Ivy", "June", "Claire", "Dawn"
        };
        public static readonly string[] FemaleNicknames = new[]
        {
            "Liz", "Maddie", "Ellie", "Katie", "Nina", "Lulu", "Bella", "Rosie", "Evie", "Daisy",
            "Bea", "Liv", "Tori", "Josie", "Gigi", "Penny", "Annie", "Izzy", "Lola", "Roxy",
            "Addie", "Mia", "Emmy", "Gracie"
        };
#pragma warning disable CS8604 // Possible null reference argument.
        public static string[] FirstNames = MaleFirstNames.Union(FemaleFirstNames).ToArray();

        public static string[] MiddleNames = MaleMiddleNames.Union(FemaleMiddleNames).ToArray();
#pragma warning restore CS8604 // Possible null reference argument.
    }
}

