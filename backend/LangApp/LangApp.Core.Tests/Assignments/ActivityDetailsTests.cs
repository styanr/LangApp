using LangApp.Core.ValueObjects.Assignments;

namespace LangApp.Core.Tests.Assignments;

public class ActivityDetailsTests
{
    [Fact]
    public void Default_CanBeGradedAutomatically_ShouldBeTrue()
    {
        // Arrange & Act
        var details = new ActivityDetails();

        // Assert
        Assert.True(details.CanBeGradedAutomatically);
    }

    [Fact]
    public void Equals_WithSameValue_ShouldReturnTrue()
    {
        // Arrange
        var details1 = new ActivityDetails { CanBeGradedAutomatically = false };
        var details2 = new ActivityDetails { CanBeGradedAutomatically = false };

        // Act & Assert
        Assert.Equal(details1, details2);
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var details1 = new ActivityDetails { CanBeGradedAutomatically = true };
        var details2 = new ActivityDetails { CanBeGradedAutomatically = false };

        // Act & Assert
        Assert.NotEqual(details1, details2);
    }
}