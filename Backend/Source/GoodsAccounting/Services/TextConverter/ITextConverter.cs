namespace GoodsAccounting.Services.TextConverter;

/// <summary>
/// Converter unicode string to ansi equivalent.
/// </summary>
public interface ITextConverter
{
    /// <summary>
    /// Converting сyrillic text to latin.
    /// </summary>
    /// <param name="source">Source text.</param>
    /// <returns>Translated text.</returns>
    string Convert(string source);
}