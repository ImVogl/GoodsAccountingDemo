import classes from './PasswordChangeControl.module.css'
import ApiClientWrapper from '../../../../common/utilites/ApiClientWrapper';
import { badRequestProcessor } from '../../../../common/utilites/Common';
import { useAppDispatch } from '../../../../common/redux/hooks';
import { Form, Container } from 'react-bootstrap';
import { FC, useState } from 'react';
import { ValidationError } from 'yup';
import Schema from './validator'
import { CommonFloatingLabelInput } from '../../../UI/Form/FloatingLabelInput/CommonFloatingLabelInput';
import { CommonButton } from '../../../UI/Button/CommonButton';

const PASSWORD_ID: string = 'password-group-form';
const NEW_PASSWORD_ID: string = 'new-password-group-form';
const CONFIRM_ID: string = 'confirm-password-group-form';

// Интерфейс типа для входных сведений для смены пароля.
export interface IChangePasswordForm{
    password: string;
    newPassword: string;
    confirmPassword: string;
}

// Обработчик события - смена пароля.
const handleChangePassword = async (client: ApiClientWrapper, form: HTMLFormElement, setErrors: Function): Promise<void> => {
    const values:IChangePasswordForm = { password: "", newPassword: "", confirmPassword: "" };
    const validationErrors:IChangePasswordForm = { password: "", newPassword: "", confirmPassword: "" };
    for (let i = 0; i < form.length; i++){
        if (form[i].localName !== "input"){
            continue;
        }

        let element = (form[i] as HTMLInputElement);
        if (element.id === PASSWORD_ID){
            values.password = element.value;
        }

        if (element.id === NEW_PASSWORD_ID){
            values.newPassword = element.value;
        }

        if (element.id === CONFIRM_ID){
            values.confirmPassword = element.value;
        }
    };

    if (!(await Schema.isValid(values))){
        await Schema.validate(values, { abortEarly: false }).catch((e) => {
            let errors = e as ValidationError;
            if (errors === undefined || errors === null){
                setErrors(validationErrors);
                return;
            };
            
            for (let i = 0; i < errors.inner.length; i++){
                let tmpPath = errors.inner[i].path;
                if (tmpPath === undefined){
                    continue;
                }

                if (tmpPath === "password"){
                    validationErrors.password = errors.inner[i].message;
                }

                if (tmpPath === "newPassword"){
                    validationErrors.newPassword = errors.inner[i].message;
                }

                if (tmpPath === "confirmPassword"){
                    validationErrors.confirmPassword = errors.inner[i].message;
                }
            }
        });

        setErrors(validationErrors);
        return;
    }
    
    try{
        await client.changePassword(values.password, values.newPassword);
    }
    catch (exception){
        if (!badRequestProcessor(exception)){
            console.error(exception);
            alert("Не удалось изменить пароль!");
        }
    }
};

// Компонента с формой смены пароля.
const PasswordChange: FC = () => {
    const initialValues: IChangePasswordForm = { password: "", newPassword: "", confirmPassword: "" };
    const initErrors: IChangePasswordForm = { password: "", newPassword: "", confirmPassword: "" };
    const [submitting, setSubmitting] = useState(false);
    const [passwordForm, setPasswordForm] = useState(initialValues);
    const [errors, setErrors] = useState(initErrors);
    const client = new ApiClientWrapper(useAppDispatch());

    return(
        <Form
        className={classes.container}
            onSubmit={async event => {
                setSubmitting(true);
                let form = event.target as HTMLFormElement;
                await handleChangePassword(client, form, setErrors);
                setSubmitting(false);
            }}
            autoComplete="off">
            <Form.Group className={classes.container}>
                <Form.Label className={classes.sublabel}>Изменить пароль</Form.Label>
                <Container className={classes.row}>
                    <Form.Group className={classes.column}>
                        <CommonFloatingLabelInput 
                            label="Текущий пароль"
                            error={errors.password}
                            type="password"
                            value={passwordForm.password}
                            id={PASSWORD_ID}
                            onChange={event => { 
                                    setPasswordForm({ password:event.target.value, newPassword:passwordForm.newPassword, confirmPassword: passwordForm.confirmPassword }) 
                                }
                            }
                        />
                    </Form.Group>
                    <Form.Group className={classes.column}>
                        <CommonFloatingLabelInput 
                            label="Новый пароль"
                            error={errors.newPassword}
                            type="password"
                            value={passwordForm.newPassword}
                            id={PASSWORD_ID}
                            onChange={event => { 
                                    setPasswordForm({ password:event.target.value, newPassword:passwordForm.newPassword, confirmPassword: passwordForm.confirmPassword }) 
                                }
                            }
                        />
                    </Form.Group>
                    <Form.Group className={classes.column}>
                        <CommonFloatingLabelInput 
                            label="Подтверждение"
                            error={errors.confirmPassword}
                            type="password"
                            value={passwordForm.confirmPassword}
                            id={PASSWORD_ID}
                            onChange={event => { 
                                    setPasswordForm({ password:event.target.value, newPassword:passwordForm.newPassword, confirmPassword: passwordForm.confirmPassword }) 
                                }
                            }
                        />
                    </Form.Group>
                    <CommonButton title="Сохранить" className={classes.column} type='submit' disabled={submitting} />
                </Container>
            </Form.Group>
        </Form>
    )
}

export default PasswordChange;