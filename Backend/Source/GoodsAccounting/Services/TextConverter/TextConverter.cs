using System.Text.RegularExpressions;
using GoodsAccounting.Model.Exceptions;
using NLog;
using ILogger = NLog.ILogger;

namespace GoodsAccounting.Services.TextConverter;

/// <summary>
/// Converter text characters from cyrillic to latin symbols.
/// </summary>
public class TextConverter : ITextConverter
{
    /// <summary>
    /// Logger for current class.
    /// </summary>
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Regular expression for cyrillic characters.
    /// </summary>
    private readonly Regex _regexC = new Regex(@"\p{IsCyrillic}", RegexOptions.CultureInvariant, TimeSpan.FromSeconds(30));

    /// <summary>
    /// Regular expression for latin characters.
    /// </summary>
    private readonly Regex _regexL = new Regex(@"[aA-zZ]", RegexOptions.CultureInvariant, TimeSpan.FromSeconds(30));

    /// <inheritdoc />
    public string Convert(string source)
    {
        var cyrillic = _regexC.Matches(source);
        var latin = _regexL.Matches(source);
        if (cyrillic.Count + latin.Count != source.Length)
        {
            Logger.Error($"Source text \'{source}\' containt non cyrillic and non-latin characters.");
            throw new BadCharConverterException();
        }

        foreach (var c in cyrillic)
        {
            if (c is not Match match)
                throw new InvalidCastException();
            

            source = source.Replace(match.Value, GetMappedChar(match.Value));
        }

        return source;
    }

    /// <summary>
    /// Getting mapped latin character.
    /// </summary>
    /// <param name="cyrillic">Cyrillic character.</param>
    /// <returns></returns>
    private string GetMappedChar(string cyrillic)
    {
        switch (cyrillic)
        {
            case "\u0410":
                return "A";
            case "\u0411":
                return "B";
            case "\u0412":
                return "V";
            case "\u0413":
                return "G";
            case "\u0414":
                return "D";
            case "\u0415":
                return "E";
            case "\u0401":
                return "E";
            case "\u0416":
                return "J";
            case "\u0417":
                return "Z";
            case "\u0418":
                return "I";
            case "\u0419":
                return "Y";
            case "\u041A":
                return "K";
            case "\u041B":
                return "L";
            case "\u041C":
                return "M";
            case "\u041D":
                return "N";
            case "\u041E":
                return "O";
            case "\u041F":
                return "P";
            case "\u0420":
                return "R";
            case "\u0421":
                return "C";
            case "\u0422":
                return "T";
            case "\u0423":
                return "U";
            case "\u0424":
                return "F";
            case "\u0425":
                return "X";
            case "\u0426":
                return "Tse";
            case "\u0427":
                return "Ch";
            case "\u0428":
                return "W";
            case "\u0429":
                return "Tsh";
            case "\u042A":
                return string.Empty;
            case "\u042B":
                return "bI";
            case "\u042C":
                return "'";
            case "\u042D":
                return "E";
            case "\u042E":
                return "Ju";
            case "\u042F":
                return "Ja";
        }

        switch (cyrillic)
        {
            case "\u0430":
                return "a";
            case "\u0431":
                return "b";
            case "\u0432":
                return "v";
            case "\u0433":
                return "g";
            case "\u0434":
                return "d";
            case "\u0435":
                return "e";
            case "\u0451":
                return "e";
            case "\u0436":
                return "j";
            case "\u0437":
                return "z";
            case "\u0438":
                return "i";
            case "\u0439":
                return "y";
            case "\u043A":
                return "k";
            case "\u043B":
                return "l";
            case "\u043C":
                return "m";
            case "\u043D":
                return "n";
            case "\u043E":
                return "o";
            case "\u043F":
                return "P";
            case "\u0440":
                return "r";
            case "\u0441":
                return "c";
            case "\u0442":
                return "t";
            case "\u0443":
                return "u";
            case "\u0444":
                return "f";
            case "\u0445":
                return "x";
            case "\u0446":
                return "tse";
            case "\u0447":
                return "ch";
            case "\u0448":
                return "w";
            case "\u0449":
                return "tsh";
            case "\u044A":
                return string.Empty;
            case "\u044B":
                return "bi";
            case "\u044C":
                return "'";
            case "\u044D":
                return "e";
            case "\u044E":
                return "ju";
            case "\u044F":
                return "ja";
        }

        throw new ArgumentOutOfRangeException();
    }
}