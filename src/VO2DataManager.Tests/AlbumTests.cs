using FluentAssertions;
using SharedServices.Models.AiData;

namespace VO2DataManager.Tests;

/// <summary>
/// Tests the Album model from SharedServices.Models.AiData.
/// Title is nullable string; ReleaseYear is int?.
/// </summary>
public class AlbumTests
{
    [Fact]
    public void Album_Title_CanBeSet()
    {
        var album = new Album { Title = "Abbey Road" };
        album.Title.Should().Be("Abbey Road");
    }

    [Fact]
    public void Album_Title_Required_ShouldNotBeNullOrEmpty_ForValidAlbum()
    {
        var album = new Album { Title = "Thriller" };
        album.Title.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Album_Title_DefaultsToNull()
    {
        var album = new Album();
        album.Title.Should().BeNull();
    }

    [Fact]
    public void Album_ReleaseYear_CanBeSetTo1970()
    {
        var album = new Album { ReleaseYear = 1970 };
        album.ReleaseYear.Should().Be(1970);
    }

    [Fact]
    public void Album_ReleaseYear_Between1900And2030_IsValid()
    {
        var validYears = new[] { 1900, 1970, 2000, 2024, 2030 };

        foreach (var year in validYears)
        {
            var album = new Album { ReleaseYear = year };
            album.ReleaseYear.Should().BeGreaterOrEqualTo(1900);
            album.ReleaseYear.Should().BeLessOrEqualTo(2030);
        }
    }

    [Fact]
    public void Album_ReleaseYear_Before1900_IsOutOfExpectedRange()
    {
        var album = new Album { ReleaseYear = 1800 };
        (album.ReleaseYear < 1900).Should().BeTrue();
    }

    [Fact]
    public void Album_ReleaseYear_After2030_IsOutOfExpectedRange()
    {
        var album = new Album { ReleaseYear = 2099 };
        (album.ReleaseYear > 2030).Should().BeTrue();
    }

    [Fact]
    public void Album_ReleaseYear_NullByDefault()
    {
        var album = new Album();
        album.ReleaseYear.Should().BeNull();
    }

    [Fact]
    public void Album_Label_IsOptional()
    {
        var album = new Album { Title = "Some Album", Label = null };
        album.Label.Should().BeNull();
    }

    [Fact]
    public void Album_Label_CanBeSet()
    {
        var album = new Album { Title = "Dark Side of the Moon", Label = "Harvest Records" };
        album.Label.Should().Be("Harvest Records");
    }
}
