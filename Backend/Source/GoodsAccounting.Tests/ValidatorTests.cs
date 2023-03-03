using GoodsAccounting.Services.Validator;
using NUnit.Framework;

namespace GoodsAccounting.Tests;

public class ValidatorTests
{
    private static readonly IPasswordValidator PasswordValidator = new Validator();

    [Test]
    [Description("Test valid password")]
    public void ValidPasswordTest()
    {
        const string validPassword = "Az!2.sssA";
        Assert.True(PasswordValidator.Validate(validPassword));
    }

    [Test]
    [Description("Password to short")]
    public void PasswordTooShortTest()
    {
        const string invalidPassword = "Az!2.";
        Assert.False(PasswordValidator.Validate(invalidPassword));
    }

    [Test]
    [Description("Password doesn't contain special symbol")]
    public void PasswordNoSpecialSymbol()
    {
        const string invalidPassword = "AzdG2sssA";
        Assert.False(PasswordValidator.Validate(invalidPassword));
    }

    [Test]
    [Description("Password doesn't contain digit character")]
    public void PasswordNoDigitTest()
    {
        const string invalidPassword = "AzdG!sssA";
        Assert.False(PasswordValidator.Validate(invalidPassword));
    }

    [Test]
    [Description("Password doesn't contain upper case letter")]
    public void PasswordNoUpperTest()
    {
        const string invalidPassword = "fz!2.sss.";
        Assert.False(PasswordValidator.Validate(invalidPassword));
    }

    [Test]
    [Description("Password doesn't contain lower case letter")]
    public void PasswordNoLowerTest()
    {
        const string invalidPassword = "A2!2.2FGA";
        Assert.False(PasswordValidator.Validate(invalidPassword));
    }
}