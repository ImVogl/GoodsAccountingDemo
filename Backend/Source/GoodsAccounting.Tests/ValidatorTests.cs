using GoodsAccounting.Model.DTO;
using GoodsAccounting.Services.Validator;
using NUnit.Framework;

namespace GoodsAccounting.Tests;

public class ValidatorTests
{
    private static readonly IPasswordValidator PasswordValidator = new Validator();
    private static readonly IDtoValidator DtoValidator = new Validator();

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

    [Test]
    [Description("Invalid login test")]
    public void InvalidLoginTest()
    {
        const string validPassword = "Az!2.sssA";
        const string validLogin = "abc";
        const string loginSpace = "abn c";
        const string loginUnicode = "abс";

        Assert.True(DtoValidator.Validate(new SignInDto { UserLogin = validLogin, Password = validPassword }));
        Assert.False(DtoValidator.Validate(new SignInDto { UserLogin = string.Empty, Password = validPassword }));
        Assert.False(DtoValidator.Validate(new SignInDto { UserLogin = loginSpace, Password = validPassword }));
        Assert.False(DtoValidator.Validate(new SignInDto { UserLogin = loginUnicode, Password = validPassword }));
    }

    [Test]
    [Description("Invalid update storage DTO test.")]
    public void InvalidUpdateStorageDtoTest()
    {
        Assert.That(
            DtoValidator.Validate(
                new UpdatingGoodsDto { Id = -1, Items = new List<UpdatingGoodsItemDto>() }), Is.False);

        Assert.That(
            DtoValidator.Validate(new UpdatingGoodsDto
            {
                Id = 1, Items = new List<UpdatingGoodsItemDto>
                {
                    new()
                    {
                        Id = Guid.NewGuid(), CurrentItemsInStorageCount = -1, Name = "123", RetailPrice = 10F,
                        WholeScalePrice = 10F
                    }
                }
            }), Is.False);

        Assert.That(
            DtoValidator.Validate(new UpdatingGoodsDto
            {
                Id = 1,
                Items = new List<UpdatingGoodsItemDto>
                {
                    {
                        new()
                        {
                            Id = Guid.NewGuid(), CurrentItemsInStorageCount = 1, Name = string.Empty, RetailPrice = 10F,
                            WholeScalePrice = 10F
                        }
                    }
                }
            }), Is.False);

        Assert.That(
            DtoValidator.Validate(new UpdatingGoodsDto
            {
                Id = 1,
                Items = new List<UpdatingGoodsItemDto>
                {
                    {
                        new()
                        {
                            Id = Guid.NewGuid(), CurrentItemsInStorageCount = 1, Name = "123", RetailPrice = -1F,
                            WholeScalePrice = 10F
                        }
                    }
                }
            }), Is.False);

        Assert.That(
            DtoValidator.Validate(new UpdatingGoodsDto
            {
                Id = 1,
                Items = new List<UpdatingGoodsItemDto>
                {
                    {
                        new()
                        {
                            Id = Guid.NewGuid(), CurrentItemsInStorageCount = 1, Name = "123", RetailPrice = 10F,
                            WholeScalePrice = -1F
                        }
                    }
                }
            }), Is.False);
    }
}