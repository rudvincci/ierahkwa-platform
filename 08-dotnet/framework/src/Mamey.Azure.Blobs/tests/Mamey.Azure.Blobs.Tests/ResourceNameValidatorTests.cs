namespace Mamey.Azure.Blobs.Tests;

using Mamey.Azure.Blobs;

public class ResourceNameValidatorTests
{
    private readonly ResourceNameValidator _validator = new ResourceNameValidator();

    [Theory]
    [InlineData("valid-container-name", true)]
    [InlineData("invalid--name", false)]
    [InlineData("1invalid-name", false)]
    [InlineData("containername", true)]
    [InlineData("invalidname!", false)]
    public void ValidateContainerNameTests(string containerName, bool expected)
    {
        var result = _validator.IsValidContainerName(containerName);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("valid-directory", true)]
    [InlineData("invalid-directory/", false)]
    [InlineData("invalid-directory.", false)]
    [InlineData("valid_directory_name", true)]
    public void ValidateDirectoryNameTests(string directoryName, bool expected)
    {
        var result = _validator.IsValidDirectoryName(directoryName);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("valid-blob-name", true)]
    [InlineData("invalid/blob/name/", false)]
    [InlineData("invalid-blob-name.", false)]
    [InlineData("valid_blob_name", true)]
    [InlineData("invalid\uE000name", false)]
    public void ValidateBlobNameTests(string blobName, bool expected)
    {
        var result = _validator.IsValidBlobName(blobName);
        Assert.Equal(expected, result);
    }
}
