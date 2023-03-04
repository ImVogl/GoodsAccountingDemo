using GoodsAccounting.Model.Exceptions;
using GoodsAccounting.Services.TextConverter;
using NUnit.Framework;

namespace GoodsAccounting.Tests;

public class TextConverterTests
{
    private static readonly ITextConverter Converter = new TextConverter();

    [Test]
    [Description("Test valid characters conversion.")]
    public void ValidTextConversionTest()
    {
        const string valid = "АБВГДЕЁЖЗИЙКЛМНУФХЦЧШЩЪЫьЭЮЯабвгдеёжзийклмнуфхцчшщъыьэюяABCDEFGHIJKLMNOPQRSTUVWXYZabsdefghijklmnopqrstuvwxyz";
        const string expected =
            "ABVGDEEJZIYKLMNUFXTseChWTshbI'EJuJaabvgdeejziyklmnufxtsechwtshbi'ejujaABCDEFGHIJKLMNOPQRSTUVWXYZabsdefghijklmnopqrstuvwxyz";

        Assert.That(Converter.Convert(valid), Is.EqualTo(expected));
    }

    [Test]
    [Description("Test non letter ansi characters and non cyrillic.")]
    public void TextWithNonLetterCharactersTest()
    {
        const string invalidDigit = "Иван0в";
        const string invalidSpecChar = "Иванов!";
        const string invalidNonCyrillicUnicode = "ИвановՑ";
        const string invalidOutOfRangeCyrillic = "ИвановЏ";

        Assert.Throws<BadCharConverterException>(() => Converter.Convert(invalidDigit));
        Assert.Throws<BadCharConverterException>(() => Converter.Convert(invalidSpecChar));
        Assert.Throws<BadCharConverterException>(() => Converter.Convert(invalidNonCyrillicUnicode));
        Assert.Throws<ArgumentOutOfRangeException>(() => Converter.Convert(invalidOutOfRangeCyrillic));
    }
}