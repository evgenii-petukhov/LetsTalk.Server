using FluentAssertions;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.Persistence.Abstractions;
using Moq;

namespace LetsTalk.Server.UnitTests.Validators;

[TestFixture]
public class CreateMessageCommandValidatorTests
{
    private CreateMessageCommandValidator _validator;
    private Mock<IAccountRepository> _mockAccountRepository;

    [SetUp]
    public void SetUp()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _validator = new(_mockAccountRepository.Object);
    }

    [Test]
    public async Task CreateMessageCommandValidator_EmptyModel()
    {
        // Arrange
        var request = new CreateMessageCommand();
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(0))
            .Returns(Task.FromResult(false));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(4);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Text is required",
            "Text cannot be empty",
            "Recipient Id is required",
            "Sender Id is required"
        });
    }
}
