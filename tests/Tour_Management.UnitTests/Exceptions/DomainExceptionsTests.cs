using System;
using System.Collections.Generic;
using Xunit;
using Tour_Management.Domain.Exceptions;

namespace Tour_Management.UnitTests.Exceptions;

public class DomainExceptionsTests
{
    [Fact]
    public void NotFoundException_WithMessage_SetsMessage()
    {
        var ex = new NotFoundException("Entity not found");
        Assert.Equal("Entity not found", ex.Message);
    }

    [Fact]
    public void NotFoundException_WithEntityNameAndKey_FormatsMessage()
    {
        var ex = new NotFoundException("Tour", 42);
        Assert.Contains("Tour", ex.Message);
        Assert.Contains("42", ex.Message);
    }

    [Fact]
    public void NotFoundException_WithStringKey_FormatsMessage()
    {
        var ex = new NotFoundException("UserInfo", "user@example.com");
        Assert.Contains("UserInfo", ex.Message);
        Assert.Contains("user@example.com", ex.Message);
    }

    [Fact]
    public void NotFoundException_IsException()
    {
        var ex = new NotFoundException("Test");
        Assert.IsAssignableFrom<Exception>(ex);
    }

    [Fact]
    public void NotFoundException_CanBeCaught_AsException()
    {
        NotFoundException? caught = null;
        try { throw new NotFoundException("Not found"); }
        catch (NotFoundException e) { caught = e; }
        Assert.NotNull(caught);
        Assert.Equal("Not found", caught.Message);
    }

    [Fact]
    public void ValidationException_WithMessage_SetsMessage()
    {
        var ex = new ValidationException("Validation failed");
        Assert.Equal("Validation failed", ex.Message);
    }

    [Fact]
    public void ValidationException_WithMessage_HasEmptyErrors()
    {
        var ex = new ValidationException("Validation failed");
        Assert.NotNull(ex.Errors);
        Assert.Empty(ex.Errors);
    }

    [Fact]
    public void ValidationException_WithErrors_SetsErrors()
    {
        var errors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required.", "Invalid email format." } },
            { "Password", new[] { "Password is too short." } }
        };
        var ex = new ValidationException(errors);
        Assert.Equal("One or more validation errors occurred.", ex.Message);
        Assert.Equal(2, ex.Errors.Count);
        Assert.Contains("Email", ex.Errors.Keys);
        Assert.Contains("Password", ex.Errors.Keys);
    }

    [Fact]
    public void ValidationException_WithErrors_ErrorsAreReadOnly()
    {
        var errors = new Dictionary<string, string[]>
        {
            { "Field", new[] { "Error message" } }
        };
        var ex = new ValidationException(errors);
        Assert.IsAssignableFrom<IReadOnlyDictionary<string, string[]>>(ex.Errors);
    }

    [Fact]
    public void ValidationException_IsException()
    {
        var ex = new ValidationException("Test");
        Assert.IsAssignableFrom<Exception>(ex);
    }

    [Fact]
    public void ValidationException_CanBeCaught_AsException()
    {
        ValidationException? caught = null;
        try { throw new ValidationException("Validation error"); }
        catch (ValidationException e) { caught = e; }
        Assert.NotNull(caught);
        Assert.Equal("Validation error", caught.Message);
    }

    [Fact]
    public void DuplicateEntityException_WithMessage_SetsMessage()
    {
        var ex = new DuplicateEntityException("Entity already exists");
        Assert.Equal("Entity already exists", ex.Message);
    }

    [Fact]
    public void DuplicateEntityException_IsException()
    {
        var ex = new DuplicateEntityException("Test");
        Assert.IsAssignableFrom<Exception>(ex);
    }

    [Fact]
    public void DuplicateEntityException_CanBeCaught_AsException()
    {
        DuplicateEntityException? caught = null;
        try { throw new DuplicateEntityException("Duplicate entity"); }
        catch (DuplicateEntityException e) { caught = e; }
        Assert.NotNull(caught);
        Assert.Equal("Duplicate entity", caught.Message);
    }

    [Fact]
    public void DuplicateEntityException_MessageContainsEmailInfo()
    {
        var email = "user@example.com";
        var ex = new DuplicateEntityException($"A user with email '{email}' already exists.");
        Assert.Contains(email, ex.Message);
    }
}
