using Mamey.Types;

namespace Mamey.Tests.Types;

public class NameTests
{
    [Fact]
    public void Name_Constructor_ShouldInitializeCorrectly()
    {
        var name = new Name("John", "Doe");

        Assert.Equal("John", name.FirstName);
        Assert.Equal("Doe", name.LastName);
        Assert.Null(name.MiddleName);
        Assert.Null(name.Nickname);
    }

    [Fact]
    public void Name_Constructor_ShouldThrowArgumentException_ForInvalidFirstName()
    {
        Assert.Throws<ArgumentException>(() => new Name(null, "Doe"));
        Assert.Throws<ArgumentException>(() => new Name("", "Doe"));
        Assert.Throws<ArgumentException>(() => new Name("   ", "Doe"));
    }

    [Fact]
    public void Name_Constructor_ShouldThrowArgumentException_ForInvalidLastName()
    {
        Assert.Throws<ArgumentException>(() => new Name("John", null));
        Assert.Throws<ArgumentException>(() => new Name("John", ""));
        Assert.Throws<ArgumentException>(() => new Name("John", "   "));
    }

    [Fact]
    public void Name_FullName_ShouldReturnCorrectFullName()
    {
        var name = new Name("John", "Doe", "William");

        Assert.Equal("John William Doe", name.FullName);
    }

    [Fact]
    public void Name_GivenNames_ShouldReturnCorrectGivenNames()
    {
        var name = new Name("John", "Doe", "William");

        Assert.Equal("John William", name.GivenNames);
    }

    [Fact]
    public void Name_ShortFullName_ShouldReturnCorrectShortFullName()
    {
        var name = new Name("John", "Doe");

        Assert.Equal("John Doe", name.ShortFullName);
    }

    [Fact]
    public void Name_Initials_ShouldReturnCorrectInitials()
    {
        var name = new Name("John", "Doe", "William");

        Assert.Equal("J.W.D.", name.Initials);
    }

    [Fact]
    public void Name_Equals_ShouldReturnTrueForEqualNames()
    {
        var name1 = new Name("John", "Doe", "William");
        var name2 = new Name("John", "Doe", "William");

        Assert.True(name1.Equals(name2));
    }

    [Fact]
    public void Name_Equals_ShouldReturnFalseForDifferentNames()
    {
        var name1 = new Name("John", "Doe", "William");
        var name2 = new Name("Jane", "Doe", "William");

        Assert.False(name1.Equals(name2));
    }

    [Fact]
    public void Name_Clone_ShouldReturnExactCopy()
    {
        var name1 = new Name("John", "Doe", "William", "Johnny");
        var name2 = (Name)name1.Clone();

        Assert.Equal(name1, name2);
        Assert.NotSame(name1, name2);
    }

    [Fact]
    public void Name_CompareTo_ShouldReturnCorrectOrder()
    {
        var name1 = new Name("John", "Doe");
        var name2 = new Name("Jane", "Doe");
        var name3 = new Name("John", "Smith");

        Assert.True(name1.CompareTo(name2) > 0);
        Assert.True(name2.CompareTo(name1) < 0);
        Assert.True(name1.CompareTo(name3) < 0);
    }


    
}