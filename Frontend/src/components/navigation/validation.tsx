import { ObjectSchema, object, string } from 'yup'
import { ILoginForm } from './NavigationBar'
import { MIN_PASSWORD_LENGTH, checkAllSymbols } from '../../common/utilites/PasswordValidation';

const Schema: ObjectSchema<ILoginForm> = object({
    login: string().defined('Логин не может быть пустым!'),
    password: string().test(
        'symbols-check',
        'Пароль должен быть не короче ' + MIN_PASSWORD_LENGTH + ' символов; содержать заглавную, строчную букву, цифру и спец символ.',
        checkAllSymbols
    ).defined('Пароль не может быть пустой!')
});

export default Schema