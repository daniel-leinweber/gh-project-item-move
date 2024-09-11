using Extension.Interfaces;
using Extension.Models;
using Moq;

namespace Extension.Tests.Services;

[TestClass]
public class GitHubServiceTests
{
    private Mock<IGitHubService>? _gh;

    [TestInitialize]
    public void Setup()
    {
        _gh = new Mock<IGitHubService>() { CallBase = true };
    }

    [TestMethod]
    public void GetOwners_ShouldReturnOwnerList()
    {
        // Arrange
        var expectedOwners = new List<string> { "owner1", "owner2" };
        _gh!.Setup(m => m.GetOwners()).Returns(expectedOwners);

        // Act
        var owners = _gh.Object.GetOwners().ToList();

        // Assert
        CollectionAssert.AreEqual(expectedOwners, owners);
    }

    [TestMethod]
    public void GetProjects_ShouldReturnProjectsList()
    {
        // Arrange
        var expectedProjects = new List<Project>
        {
            new Project
            {
                Id = "Id1",
                Title = "Project1",
                Owner = "owner",
            },
            new Project
            {
                Id = "Id2",
                Title = "Project2",
                Owner = "owner",
            },
        };

        _gh!.Setup(m => m.GetProjects(It.IsAny<string>())).Returns(expectedProjects);

        // Act
        var projects = _gh.Object.GetProjects("owner");

        // Assert
        Assert.AreEqual(2, projects.Count);
        Assert.AreEqual("Project1", projects[0].Title);
    }

    [TestMethod]
    public void MoveIssue_ShouldReturnTrue_WhenIssueMovedSuccessfully()
    {
        // Arrange
        _gh!
            .Setup(m =>
                m.MoveIssue(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .Returns(true);

        // Act
        var result = _gh.Object.MoveIssue("issueId", "projectId", "fieldId", "optionId");

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void MoveIssue_ShouldReturnFalse_WhenIssueMoveFails()
    {
        // Arrange
        _gh!
            .Setup(m =>
                m.MoveIssue(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .Returns(false);

        // Act
        var result = _gh.Object.MoveIssue("issueId", "projectId", "fieldId", "optionId");

        // Assert
        Assert.IsFalse(result);
    }
}
