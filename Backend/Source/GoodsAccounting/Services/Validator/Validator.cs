﻿using GoodsAccounting.Model.DTO;
using System.Text.RegularExpressions;

namespace GoodsAccounting.Services.Validator;

/// <summary>
/// Data validator.
/// </summary>
public class Validator : IPasswordValidator, IDtoValidator
{
    /// <inheritdoc />
    public bool Validate(string value)
    {
        const int minPasswordLength = 8;
        var regexWordUp = new Regex(@"[A-Z]");
        var regexWordLow = new Regex(@"[a-z]");
        var regexDigit = new Regex(@"\d");
        var regexNotWord = new Regex(@"\W");
        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (value.Length < minPasswordLength)
            return false;
        
        return regexWordUp.IsMatch(value) 
               && regexWordLow.IsMatch(value) 
               && regexDigit.IsMatch(value) 
               && regexNotWord.IsMatch(value);
    }

    /// <inheritdoc />
    public bool Validate(SignInDto dto)
    {
        const int lastAnsiSymbol = 128;
        var regexSpace = new Regex(@"\s");
        return 
            !string.IsNullOrWhiteSpace(dto.UserLogin)
            && !regexSpace.IsMatch(dto.UserLogin)
            && dto.UserLogin.All(sym => sym < lastAnsiSymbol)
            && Validate(dto.Password);
    }

    /// <inheritdoc />
    public bool Validate(GoodsRevisionDto dto)
    {
        if (dto.Id < 1) 
            return false;

        foreach (var item in dto.Items)
        {
            if (item.Storage < 0) return false;
            if (item.RetailPrice < 0) return false;
            if (item.WriteOff < 0) return false;
            if (string.IsNullOrWhiteSpace(item.Category)) return false;
        }

        return true;
    }

    /// <inheritdoc />
    public bool Validate(GoodsSuppliesDto dto)
    {
        if (dto.Id < 1)
            return false;

        foreach (var item in dto.Items)
        {
            if (item.WholeScalePrice < 0) return false;
            if (item.Receipt < 0) return false;
        }

        return true;
    }

    /// <inheritdoc />
    public bool Validate(EditGoodsListDto dto)
    {
        if (dto == null) return false;
        if (dto.UserId < 1) return false;
        if (dto.CreateNew) return !string.IsNullOrWhiteSpace(dto.Name) && !string.IsNullOrWhiteSpace(dto.Category);
        return dto.Id != null;
    }
}