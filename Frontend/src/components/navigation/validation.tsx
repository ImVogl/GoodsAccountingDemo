import { ObjectSchema, object, string, TestFunction } from 'yup'
import { ILoginForm } from './NavigationBar'

const MIN_PASSWORD_LENGTH = 8;
export function lengthEnought(value: string):boolean
{
    return value.length >= MIN_PASSWORD_LENGTH
}

export function containsSpecSymbol(value: string): boolean
{
    return /[\!\@#\$\%\^\&\*\(\)\-\_\+\=\/\\\?\>\<\.\,\`\~\{\}]/.test(value);
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
const checkNoBadSymbols: TestFunction<string|undefined, {}> = (value, context):boolean => {
    let s_value = String(value);
    if (!lengthEnought(s_value)){
        return false;
    }

    return containsSpecSymbol(s_value) && containsLetter(s_value) && containsDigit(s_value);
}

const Schema: ObjectSchema<ILoginForm> = object({
    login: string().defined('Логин не может быть пустым!'),
        password: string().test(
            'symbols-check',
            'Пароль должен быть не короче ' + MIN_PASSWORD_LENGTH + ' символов; содержать заглавную, строчную букву, цифру и спец символ.',
            checkNoBadSymbols
        ).defined('Пароль не может быть пустой!')
  });

export default Schema