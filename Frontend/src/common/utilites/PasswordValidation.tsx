import { TestFunction } from 'yup'

export const MIN_PASSWORD_LENGTH = 8;
export function lengthEnought(value: string):boolean
{
    return value.length >= MIN_PASSWORD_LENGTH
}

export function containsSpecSymbol(value: string): boolean
{
    return /[!@#$%^&*()\-_+=/\\?><.,`~{}'":;]/.test(value);
}

export function containsLetter(value: string): boolean
{
    return /[A-Z]+/.test(value) && /[a-z]+/.test(value);
}

export function containsDigit(value: string): boolean
{
    return /[0-9]+/.test(value) && /[a-z]+/.test(value);
}

// Validation: value can't cantains incorrect symbols.
export const checkAllSymbols: TestFunction<string|undefined, {}> = (value, context):boolean => {
    let s_value = String(value);
    if (!lengthEnought(s_value)){
        return false;
    }

    return containsSpecSymbol(s_value) && containsLetter(s_value) && containsDigit(s_value);
}
