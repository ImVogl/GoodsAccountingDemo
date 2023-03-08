import { ObjectSchema, object, string, TestFunction, TestContext } from 'yup'
import { ILoginForm } from './NavigationBar'

const MIN_PASSWORD_LENGTH = 8;

// Validation: value can't cantains incorrect symbols.
const checkNoBadSymbols: TestFunction<string|undefined, {}> = (value, context):boolean => {
    let s_value = String(value);
    if (s_value.length < MIN_PASSWORD_LENGTH){
        return false;
    }

    return true;
    return !(/[!@#$%^&*()-_+=/\\?><.,`~{}]/.test(s_value));
}

const Schema: ObjectSchema<ILoginForm> = object({
    login: string().defined('Логин не может быть пустым!'),
        password: string().test(
            'symbols-check',
            'Пароль должен быть не короче ' + MIN_PASSWORD_LENGTH + ' символов',
            checkNoBadSymbols
        ).defined('Пароль не может быть пустой!')
  });

export default Schema