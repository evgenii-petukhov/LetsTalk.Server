﻿using FluentAssertions;
using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Utility.Common;

namespace LetsTalk.Server.API.Validation.Tests;

[TestFixture]
public class GenerateLoginCodeRequestValidatorTests
{
    private const string ValidEmailSample = "email@example.com";

    private GenerateLoginCodeRequestValidator _validator;
    private static readonly string[] expectation =
        [
            "Email cannot be empty",
            "Email must be a valid email",
            "Anti Spam Token cannot be empty",
            "Anti-spam check failed"
        ];
    private static readonly string[] expectationArray =
        [
            "Anti-spam check failed"
        ];

    [SetUp]
    public void SetUp()
    {
        _validator = new(new SecuritySettings
        {
            AntiSpamTokenLifeTimeInSeconds = 60
        });
    }

    [Test]
    public async Task ValidateAsync_When_EmailIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new GenerateLoginCodeRequest();
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(
        [
            "Email is required",
            "Email cannot be empty",
            "Anti Spam Token cannot be empty",
            "Anti-spam check failed"
        ]);
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public async Task ValidateAsync_When_EmailIsEmpty_ShouldContainValidationErrors(string email)
    {
        // Arrange
        var request = new GenerateLoginCodeRequest
        {
            Email = email
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(expectation);
    }

    [Test]
    [TestCase("plainaddress")]
    [TestCase("#@%^%#$@#$@#.com")]
    [TestCase("@example.com")]
    [TestCase("email.example.com")]
    [TestCase("email@example@example.com")]
    public async Task ValidateAsync_When_EmailIsInvalid_ShouldContainValidationErrors(string email)
    {
        // Arrange
        var request = new GenerateLoginCodeRequest
        {
            Email = email
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(
        [
            "Email must be a valid email",
            "Anti Spam Token cannot be empty",
            "Anti-spam check failed"
        ]);
    }

    [Test]
    [TestCase(-60)]
    [TestCase(60)]
    public async Task ValidateAsync_When_EmailIsValid_AntiSpamTokenIsInvalid_ShouldContainValidationErrors(long delta)
    {
        // Arrange
        var request = new GenerateLoginCodeRequest
        {
            Email = ValidEmailSample,
            AntiSpamToken = DateHelper.GetUnixTimestamp() + delta
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(expectationArray);
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(-1)]
    [TestCase(-59)]
    [TestCase(59)]
    public async Task ValidateAsync_When_EmailIsValid_AntiSpamTokenIsValid_ShouldBeValid(long delta)
    {
        // Arrange
        var request = new GenerateLoginCodeRequest
        {
            Email = ValidEmailSample,
            AntiSpamToken = DateHelper.GetUnixTimestamp() + delta
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }
}
