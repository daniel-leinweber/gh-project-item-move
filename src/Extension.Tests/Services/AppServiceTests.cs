using Extension.Interfaces;
using Extension.Models;
using Extension.Services;
using Moq;

namespace Extension.Tests.Services;

[TestClass]
public class AppServiceTests
{
    private Mock<IGitHubService>? _gitHubServiceMock;
    private Mock<ICliPromptService>? _cliPromptServiceMock;
    private Mock<IConsole>? _consoleMock;
    private AppService? _appService;

    [TestInitialize]
    public void Setup()
    {
        _consoleMock = new Mock<IConsole>();
        _gitHubServiceMock = new Mock<IGitHubService>();
        _cliPromptServiceMock = new Mock<ICliPromptService>();
        _appService = new AppService(
            _gitHubServiceMock.Object,
            _cliPromptServiceMock.Object,
            _consoleMock.Object
        );
    }

    [TestMethod]
    public void Run_ShouldReturnSuccess_WhenAllInputsAreValid()
    {
        // Arrange
        var options = new CommandLineOptions
        {
            Owner = "valid-owner",
            ProjectName = "valid-project",
            IssueId = 456,
            ColumnName = "To Do",
        };

        var owner = "valid-owner";
        var project = new Project
        {
            Id = "1",
            Number = 123,
            Title = "valid-project",
            Owner = owner,
        };
        var issue = new ProjectItem
        {
            Id = "1",
            Content = new Content
            {
                Repository = "Repo",
                Type = "Type",
                Url = new("https://www.github.com"),
                Title = "Title",
                Body = "Body",
                Number = 456,
            },
            Title = "Issue 1",
            Repository = new("https://www.github.com"),
            Labels = ["Label1"],
        };
        var issues = new List<ProjectItem> { issue };
        var projectStatusField = new ProjectField
        {
            Id = "1",
            Name = "Status",
            Options = new[]
            {
                new Option { Name = "To Do", Id = "1" },
            },
        };
        var column = new Option { Name = "To Do", Id = "1" };

        // Setup mocks
        _gitHubServiceMock!
            .Setup(m => m.GetProjects(owner))
            .Returns(new List<Project> { project });
        _gitHubServiceMock.Setup(m => m.GetIssues(project, owner)).Returns(issues);
        _gitHubServiceMock
            .Setup(m => m.GetProjectField(project.Number, "Status", owner))
            .Returns(projectStatusField);
        _gitHubServiceMock
            .Setup(m => m.MoveIssue(issue.Id, project.Id, projectStatusField.Id, column.Id))
            .Returns(true);
        _cliPromptServiceMock!
            .Setup(m => m.PrintSingleSelectMenu(It.IsAny<string>(), It.IsAny<string[]>(), null))
            .Returns("To Do");

        // Act
        var result = _appService!.Run(options);

        // Assert
        Assert.AreEqual(ExitCode.Success, result.ExitCode);
        Assert.AreEqual("Issue moved successfully!", result.Message);
    }

    [TestMethod]
    public void Run_ShouldReturnError_WhenOwnerIsInvalid()
    {
        // Arrange
        var options = new CommandLineOptions
        {
            Owner = null, // Simulate missing owner
            ProjectName = "valid-project",
            IssueId = 123,
            ColumnName = "To Do",
        };

        _gitHubServiceMock!.Setup(m => m.GetOwners()).Returns(new List<string>()); // No owners returned

        // Act
        var result = _appService!.Run(options);

        // Assert
        Assert.AreEqual(ExitCode.Error, result.ExitCode);
        Assert.AreEqual("Error: No owner! Please provide a valid owner.", result.Message);
    }

    [TestMethod]
    public void Run_ShouldReturnError_WhenProjectIsInvalid()
    {
        // Arrange
        var options = new CommandLineOptions
        {
            Owner = "valid-owner",
            ProjectName = null, // Simulate missing project
            IssueId = 123,
            ColumnName = "To Do",
        };

        var owner = "valid-owner";
        _gitHubServiceMock!.Setup(m => m.GetOwners()).Returns(new List<string> { owner });
        _gitHubServiceMock!.Setup(m => m.GetProjects(owner)).Returns(new List<Project>()); // No projects returned

        // Act
        var result = _appService!.Run(options);

        // Assert
        Assert.AreEqual(ExitCode.Error, result.ExitCode);
        Assert.AreEqual("Error: No Project! Please provide a valid project name.", result.Message);
    }

    [TestMethod]
    public void Run_ShouldReturnError_WhenNoIssuesFound()
    {
        // Arrange
        var options = new CommandLineOptions
        {
            Owner = "valid-owner",
            ProjectName = "valid-project",
            IssueId =
                null // Simulate user input for issue selection
            ,
        };

        var owner = "valid-owner";
        var project = new Project
        {
            Id = "1",
            Number = 123,
            Title = "valid-project",
            Owner = owner,
        };

        _gitHubServiceMock!.Setup(m => m.GetOwners()).Returns(new List<string> { owner });
        _gitHubServiceMock!.Setup(m => m.GetProjects(owner)).Returns(new List<Project> { project });
        _gitHubServiceMock!
            .Setup(m => m.GetIssues(project, owner))
            .Returns(new List<ProjectItem>()); // No issues returned

        // Act
        var result = _appService!.Run(options);

        // Assert
        Assert.AreEqual(ExitCode.Error, result.ExitCode);
        Assert.AreEqual("Error: No issue! Please provide a valid issue number.", result.Message);
    }
}
