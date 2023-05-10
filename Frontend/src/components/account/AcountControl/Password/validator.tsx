import { IChangePasswordForm } from './PasswordChangeControl';
import { ObjectSchema, object, string, ref } from 'yup'
import { MIN_PASSWORD_LENGTH, checkAllSymbols } from '../../../../common/utilites/PasswordValidation';

const SchemaValidation: ObjectSchema<IChangePasswordForm> = object({
    password: string().test(
        'symbols-check',
        'Пароль должен быть не короче ' + MIN_PASSWORD_LENGTH + ' символов; содержать заглавную, строчную букву, цифру и спец символ.',
        checkAllSymbols
    ).defined('Пароль не может быть пустой!'),
    newPassword: string().test(
        'symbols-check',
        'Пароль должен быть не короче ' + MIN_PASSWORD_LENGTH + ' символов; содержать заглавную, строчную букву, цифру и спец символ.',
        checkAllSymbols
    ).defined('Пароль не может быть пустой!'),
    confirmPassword: string().oneOf([ref('newPassword')], "Пароли дольжны совпадать.")
    .defined('Пароль не может быть пустой!')
});

export default SchemaValidation