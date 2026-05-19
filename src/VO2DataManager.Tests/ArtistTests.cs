using FluentAssertions;
using SharedServices.Models.Music;

namespace VO2DataManager.Tests;

/// <summary>
/// Tests the Artist model: Name required, Songs collection navigation.
/// The Artist model has no Country property (Country is accessed via Chart entries).
/// </summary>
public class ArtistTests
{
    [Fact]
    public void Artist_Name_DefaultsToEmptyString()
    {
        var artist = new Artist();
        artist.Name.Should().NotBeNull();
        artist.Name.Should().Be(string.Empty);
    }

    [Fact]
    public void Artist_Name_CanBeSet()
    {
        var artist = new Artist { Name = "The Beatles" };
        artist.Name.Should().Be("The Beatles");
    }

    [Fact]
    public void Artist_Name_Required_ShouldNotBeEmpty()
    {
        var artist = new Artist { Name = "Adele" };
        artist.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Artist_Songs_DefaultsToEmptyCollection()
    {
        var artist = new Artist();
        artist.Songs.Should().NotBeNull();
        artist.Songs.Should().BeEmpty();
    }

    [Fact]
    public void Artist_Songs_CanAddSongs()
    {
        var artist = new Artist { Name = "Queen" };
        artist.Songs.Add(new Song { Title = "Bohemian Rhapsody", ArtistId = artist.Guid });
        artist.Songs.Add(new Song { Title = "We Will Rock You", ArtistId = artist.Guid });

        artist.Songs.Should().HaveCount(2);
        artist.Songs.Select(s => s.Title).Should().Contain("Bohemian Rhapsody");
    }

    [Fact]
    public void Artist_Guid_IsAutomaticallyGenerated()
    {
        var artist = new Artist();
        artist.Guid.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void TwoArtists_HaveDifferentGuids()
    {
        var a1 = new Artist { Name = "Artist One" };
        var a2 = new Artist { Name = "Artist Two" };

        a1.Guid.Should().NotBe(a2.Guid);
    }
}
