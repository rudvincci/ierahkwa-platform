using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using Mamey.Persistence.Minio.Exceptions;
using Xunit;

namespace Mamey.Persistence.Minio.Tests.Exceptions;

/// <summary>
/// Unit tests for custom exception classes.
/// </summary>
public class ExceptionTests
{
    #region MinioServiceException Tests

    [Fact]
    public void MinioServiceException_Constructor_SetsProperties()
    {
        // Arrange
        var message = "Test Minio service exception";
        var errorCode = typeof(MinioServiceException).Name.Underscore().Replace("_exception", string.Empty);
        var innerException = new Exception("Inner exception");

        // Act
        var exception = new MinioServiceException(message, errorCode,innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.Code.Should().Be(errorCode);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void MinioServiceException_ConstructorWithMessageOnly_SetsMessage()
    {
        // Arrange
        var message = "Test Minio service exception";
        var errorCode = "minio_service_error";

        // Act
        var exception = new MinioServiceException(message, errorCode);

        // Assert
        exception.Message.Should().Be(message);
        exception.Code.Should().Be(errorCode);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void MinioServiceException_DefaultConstructor_SetsDefaultMessage()
    {
        // Arrange
        var message = "Minio service error";
        var errorCode = "minio_service_error";

        // Act
        var exception = new MinioServiceException(message, errorCode);

        // Assert
        exception.Message.Should().NotBeNullOrEmpty();
        exception.Code.Should().NotBeNullOrEmpty();
        exception.InnerException.Should().BeNull();
    }

    // Commented out - BinaryFormatter is obsolete in .NET 9
    // [Fact]
    // public void MinioServiceException_Serialization_WorksCorrectly()
    // {
    //     // Arrange
    //     var originalException = new MinioServiceException("Test message", "minio_error", new Exception("Inner"));
    //     var formatter = new BinaryFormatter();
    //     var stream = new MemoryStream();
    //
    //     // Act
    //     formatter.Serialize(stream, originalException);
    //     stream.Position = 0;
    //     var deserializedException = (MinioServiceException)formatter.Deserialize(stream);
    //
    //     // Assert
    //     deserializedException.Message.Should().Be(originalException.Message);
    //     deserializedException.InnerException.Should().NotBeNull();
    //     deserializedException.InnerException!.Message.Should().Be("Inner");
    // }

    #endregion

    #region BucketOperationException Tests

    [Fact]
    public void BucketOperationException_Constructor_SetsBucketNameAndOperation()
    {
        // Arrange
        var bucketName = "test-bucket";
        var operation = "create";
        var message = "Test bucket operation exception";
        var innerException = new Exception("Inner exception");

        // Act
        var exception = new BucketOperationException(bucketName, operation, innerException);

        // Assert
        exception.BucketName.Should().Be(bucketName);
        exception.Operation.Should().Be(operation);
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void BucketOperationException_ConstructorWithInnerException_SetsProperties()
    {
        // Arrange
        var bucketName = "test-bucket";
        var operation = "delete";
        var innerException = new Exception("Inner exception");

        // Act
        var exception = new BucketOperationException(bucketName, operation, innerException);

        // Assert
        exception.BucketName.Should().Be(bucketName);
        exception.Operation.Should().Be(operation);
        exception.InnerException.Should().Be(innerException);
        exception.Message.Should().Contain(bucketName);
        exception.Message.Should().Contain(operation);
    }

    [Fact]
    public void BucketOperationException_ConstructorWithMessageOnly_SetsProperties()
    {
        // Arrange
        var bucketName = "test-bucket";
        var operation = "list";

        // Act
        var exception = new BucketOperationException(bucketName, operation);

        // Assert
        exception.BucketName.Should().Be(bucketName);
        exception.Operation.Should().Be(operation);
        exception.Message.Should().NotBeNullOrEmpty();
        exception.InnerException.Should().BeNull();
    }

    // Commented out - BinaryFormatter is obsolete in .NET 9
    // [Fact]
    // public void BucketOperationException_Serialization_WorksCorrectly()
    // {
    //     // Arrange
    //     var originalException = new BucketOperationException("test-bucket", "create", "Test message", new Exception("Inner"));
    //     var formatter = new BinaryFormatter();
    //     var stream = new MemoryStream();
    //
    //     // Act
    //     formatter.Serialize(stream, originalException);
    //     stream.Position = 0;
    //     var deserializedException = (BucketOperationException)formatter.Deserialize(stream);
    //
    //     // Assert
    //     deserializedException.BucketName.Should().Be(originalException.BucketName);
    //     deserializedException.Operation.Should().Be(originalException.Operation);
    //     deserializedException.Message.Should().Be(originalException.Message);
    //     deserializedException.InnerException.Should().NotBeNull();
    // }

    #endregion

    #region ObjectOperationException Tests

    [Fact]
    public void ObjectOperationException_Constructor_SetsAllProperties()
    {
        // Arrange
        var bucketName = "test-bucket";
        var objectName = "test-object.txt";
        var operation = "get";
        var innerException = new Exception("Inner exception");

        // Act
        var exception = new ObjectOperationException(bucketName, objectName, operation, innerException);

        // Assert
        exception.BucketName.Should().Be(bucketName);
        exception.ObjectName.Should().Be(objectName);
        exception.Operation.Should().Be(operation);
        exception.Message.Should().NotBeNullOrEmpty();
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void ObjectOperationException_ConstructorWithInnerException_SetsProperties()
    {
        // Arrange
        var bucketName = "test-bucket";
        var objectName = "test-object.txt";
        var operation = "put";
        var innerException = new Exception("Inner exception");

        // Act
        var exception = new ObjectOperationException(bucketName, objectName, operation, innerException);

        // Assert
        exception.BucketName.Should().Be(bucketName);
        exception.ObjectName.Should().Be(objectName);
        exception.Operation.Should().Be(operation);
        exception.InnerException.Should().Be(innerException);
        exception.Message.Should().Contain(bucketName);
        exception.Message.Should().Contain(objectName);
        exception.Message.Should().Contain(operation);
    }

    [Fact]
    public void ObjectOperationException_ConstructorWithMessageOnly_SetsProperties()
    {
        // Arrange
        var bucketName = "test-bucket";
        var objectName = "test-object.txt";
        var operation = "delete";

        // Act
        var exception = new ObjectOperationException(bucketName, objectName, operation);

        // Assert
        exception.BucketName.Should().Be(bucketName);
        exception.ObjectName.Should().Be(objectName);
        exception.Operation.Should().Be(operation);
        exception.Message.Should().NotBeNullOrEmpty();
        exception.InnerException.Should().BeNull();
    }

    // Commented out - BinaryFormatter is obsolete in .NET 9
    // [Fact]
    // public void ObjectOperationException_Serialization_WorksCorrectly()
    // {
    //     // Arrange
    //     var originalException = new ObjectOperationException("test-bucket", "test-object.txt", "get", "Test message", new Exception("Inner"));
    //     var formatter = new BinaryFormatter();
    //     var stream = new MemoryStream();
    //
    //     // Act
    //     formatter.Serialize(stream, originalException);
    //     stream.Position = 0;
    //     var deserializedException = (ObjectOperationException)formatter.Deserialize(stream);
    //
    //     // Assert
    //     deserializedException.BucketName.Should().Be(originalException.BucketName);
    //     deserializedException.ObjectName.Should().Be(originalException.ObjectName);
    //     deserializedException.Operation.Should().Be(originalException.Operation);
    //     deserializedException.Message.Should().Be(originalException.Message);
    //     deserializedException.InnerException.Should().NotBeNull();
    // }

    #endregion

    #region Exception Hierarchy Tests

    [Fact]
    public void BucketOperationException_InheritsFromMinioServiceException()
    {
        // Arrange
        var exception = new BucketOperationException("test-bucket", "create");

        // Assert
        exception.Should().BeAssignableTo<MinioServiceException>();
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void ObjectOperationException_InheritsFromMinioServiceException()
    {
        // Arrange
        var exception = new ObjectOperationException("test-bucket", "test-object.txt", "get");

        // Assert
        exception.Should().BeAssignableTo<MinioServiceException>();
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void MinioServiceException_InheritsFromException()
    {
        // Arrange
        var exception = new MinioServiceException("Test message", "test_error_code", "Test description");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    #endregion

    #region Exception Message Tests

    [Fact]
    public void BucketOperationException_Message_ContainsBucketNameAndOperation()
    {
        // Arrange
        var bucketName = "my-test-bucket";
        var operation = "create";
        var innerException = new Exception("Inner exception");

        // Act
        var exception = new BucketOperationException(bucketName, operation, innerException);

        // Assert
        exception.Message.Should().Contain(bucketName);
        exception.Message.Should().Contain(operation);
    }

    [Fact]
    public void ObjectOperationException_Message_ContainsAllProperties()
    {
        // Arrange
        var bucketName = "my-test-bucket";
        var objectName = "my-test-object.txt";
        var operation = "get";
        var innerException = new Exception("Inner exception");

        // Act
        var exception = new ObjectOperationException(bucketName, objectName, operation, innerException);

        // Assert
        exception.Message.Should().Contain(bucketName);
        exception.Message.Should().Contain(objectName);
        exception.Message.Should().Contain(operation);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void BucketOperationException_NullBucketName_HandlesGracefully()
    {
        // Arrange
        var bucketName = (string)null!;
        var operation = "create";
        var message = "Test message";

        // Act
        var exception = new BucketOperationException(bucketName, operation);

        // Assert
        exception.BucketName.Should().BeNull();
        exception.Operation.Should().Be(operation);
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void BucketOperationException_EmptyOperation_HandlesGracefully()
    {
        // Arrange
        var bucketName = "test-bucket";
        var operation = "";
        var message = "Test message";

        // Act
        var exception = new BucketOperationException(bucketName, operation);

        // Assert
        exception.BucketName.Should().Be(bucketName);
        exception.Operation.Should().Be(operation);
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void ObjectOperationException_NullObjectName_HandlesGracefully()
    {
        // Arrange
        var bucketName = "test-bucket";
        var objectName = (string)null!;
        var operation = "get";
        var message = "Test message";

        // Act
        var exception = new ObjectOperationException(bucketName, objectName, operation);

        // Assert
        exception.BucketName.Should().Be(bucketName);
        exception.ObjectName.Should().BeNull();
        exception.Operation.Should().Be(operation);
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void MinioServiceException_NullMessage_HandlesGracefully()
    {
        // Arrange
        string message = null!;
        var errorCode = "minio_service_error";

        // Act
        var exception = new MinioServiceException(message ?? "Error", errorCode);

        // Assert
        exception.Message.Should().NotBeNull();
        exception.InnerException.Should().BeNull();
    }

    #endregion
}
