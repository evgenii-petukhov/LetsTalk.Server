using FluentAssertions;
using LetsTalk.Server.Notifications.Services;

namespace LetsTalk.Server.Notifications.Tests;

[TestFixture]
public class ConnectionManagerTests
{
    private ConnectionManager _connectionManager;

    [SetUp]
    public void SetUp()
    {
        _connectionManager = new ConnectionManager();
    }

    [Test]
    public void GetConnectionIds_When_AccountIdDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = _connectionManager.GetConnectionIds("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void AddConnectionId_When_NewAccount_ShouldCreateNewEntry()
    {
        // Act
        _connectionManager.AddConnectionId("account1", "connection1");

        // Assert
        _connectionManager.ConnectionIdAccountIdBy.Should().ContainKey("account1");
        _connectionManager.ConnectionIdAccountIdBy["account1"].Should().Contain("connection1");
    }

    [Test]
    public void AddConnectionId_When_ExistingAccount_ShouldAddToExistingEntry()
    {
        // Arrange
        _connectionManager.AddConnectionId("account1", "connection1");

        // Act
        _connectionManager.AddConnectionId("account1", "connection2");

        // Assert
        _connectionManager.ConnectionIdAccountIdBy["account1"].Should().HaveCount(2);
        _connectionManager.ConnectionIdAccountIdBy["account1"].Should().Contain("connection1");
        _connectionManager.ConnectionIdAccountIdBy["account1"].Should().Contain("connection2");
    }

    [Test]
    public void GetConnectionIds_When_AccountExists_ShouldReturnConnectionIds()
    {
        // Arrange
        _connectionManager.AddConnectionId("account1", "connection1");
        _connectionManager.AddConnectionId("account1", "connection2");

        // Act
        var result = _connectionManager.GetConnectionIds("account1");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain("connection1");
        result.Should().Contain("connection2");
    }

    [Test]
    public void RemoveConnectionId_When_ConnectionExists_ShouldRemoveConnection()
    {
        // Arrange
        _connectionManager.AddConnectionId("account1", "connection1");
        _connectionManager.AddConnectionId("account1", "connection2");

        // Act
        _connectionManager.RemoveConnectionId("connection1");

        // Assert
        _connectionManager.ConnectionIdAccountIdBy["account1"].Should().HaveCount(1);
        _connectionManager.ConnectionIdAccountIdBy["account1"].Should().Contain("connection2");
        _connectionManager.ConnectionIdAccountIdBy["account1"].Should().NotContain("connection1");
    }

    [Test]
    public void RemoveConnectionId_When_LastConnectionForAccount_ShouldRemoveAccount()
    {
        // Arrange
        _connectionManager.AddConnectionId("account1", "connection1");

        // Act
        _connectionManager.RemoveConnectionId("connection1");

        // Assert
        _connectionManager.ConnectionIdAccountIdBy.Should().NotContainKey("account1");
    }

    [Test]
    public void RemoveConnectionId_When_ConnectionDoesNotExist_ShouldNotThrow()
    {
        // Act & Assert
        _connectionManager.Invoking(x => x.RemoveConnectionId("nonexistent"))
            .Should().NotThrow();
    }

    [Test]
    public void AddConnectionId_When_SameConnectionIdAddedTwice_ShouldNotDuplicate()
    {
        // Act
        _connectionManager.AddConnectionId("account1", "connection1");
        _connectionManager.AddConnectionId("account1", "connection1");

        // Assert
        _connectionManager.ConnectionIdAccountIdBy["account1"].Should().HaveCount(1);
        _connectionManager.ConnectionIdAccountIdBy["account1"].Should().Contain("connection1");
    }

    [Test]
    public void RemoveConnectionId_When_MultipleAccountsExist_ShouldOnlyRemoveFromCorrectAccount()
    {
        // Arrange
        _connectionManager.AddConnectionId("account1", "connection1");
        _connectionManager.AddConnectionId("account2", "connection2");

        // Act
        _connectionManager.RemoveConnectionId("connection1");

        // Assert
        _connectionManager.ConnectionIdAccountIdBy.Should().NotContainKey("account1");
        _connectionManager.ConnectionIdAccountIdBy.Should().ContainKey("account2");
        _connectionManager.ConnectionIdAccountIdBy["account2"].Should().Contain("connection2");
    }
}