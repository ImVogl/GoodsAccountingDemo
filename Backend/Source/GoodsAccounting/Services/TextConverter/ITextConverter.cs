using GoodsAccounting.Model.Exceptions;
using JetBrains.Annotations;

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
    /// <exception cref="BadCharConverterException"><see cref="BadCharConverterException"/>.</exception>
    /// <exception cref="InvalidCastException">Can't cast one of character from source.</exception>
    [NotNull]
    string Convert([NotNull] string source);
}