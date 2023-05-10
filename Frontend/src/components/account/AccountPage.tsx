import './Account.css'
import { useAppSelector, useAppDispatch } from '../../common/redux/hooks';
import { selectUserName, selectUserIsAdmin } from '../../common/redux/UserSlice';
import { FC, useState } from 'react';
import { Form, Container, Button } from 'react-bootstrap';
import WorkingArea from '../base/working/WorkingArea';
import ApiClientWrapper from '../../common/utilites/ApiClientWrapper';
import Schema from './validator'
import { ValidationError } from 'yup';
import { badRequestProcessor } from '../../common/utilites/Common';
import AddUser from './ManagerPanel/Add/AddUser';
import RemoveUser from './ManagerPanel/Remove/RemoveUser';

export interface IChangePasswordForm{
    password: string;
    newPassword: string;
    confirmPassword: string;
}

const PASSWORD_ID: string = 'password-group-form';
const NEW_PASSWORD_ID: string = 'new-password-group-form';
const CONFIRM_ID: string = 'confirm-password-group-form';

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

const AccountPage: FC = () => {
    const initialValues: IChangePasswordForm = { password: "", newPassword: "", confirmPassword: "" };
    const initErrors: IChangePasswordForm = { password: "", newPassword: "", confirmPassword: "" };
    const [submitting, setSubmitting] = useState(false);
    const [passwordForm, setPasswordForm] = useState(initialValues);
    const [errors, setErrors] = useState(initErrors);
    const displayedName = useAppSelector(selectUserName);
    const admin = useAppSelector(selectUserIsAdmin);
    const client = new ApiClientWrapper(useAppDispatch());
    return(
        <WorkingArea>
            <div className='account-page-root'>
                <div className='account-header'>{admin ? "Менеджер" : "Продавец"} {displayedName}</div>
                <Form
                    className='main-account-container'
                    onSubmit={async event => {
                        setSubmitting(true);
                        let form = event.target as HTMLFormElement;
                        await handleChangePassword(client, form, setErrors);
                        setSubmitting(false);
                    }}
                    autoComplete="off">
                    <Form.Group className='main-account-container'>
                        <Form.Label className='main-account-sublabel'>Изменить пароль</Form.Label>
                        <Container className='main-account-row'>
                            <Form.Group className='main-account-col'>
                                <Form.Label className='main-account-control'>Текущий пароль</Form.Label>
                                <Form.Control
                                    type="password"
                                    value={passwordForm.password}
                                    onChange={event => { setPasswordForm({ password:event.target.value, newPassword:passwordForm.newPassword, confirmPassword: passwordForm.confirmPassword }) }}
                                    id={PASSWORD_ID}
                                    className={errors.password !== "" ? "main-account-control input-control-error" : "main-account-control"} />
                                <Form.Text>{errors.password}</Form.Text>
                            </Form.Group>
                            <Form.Group className='main-account-col'>
                                <Form.Label className='main-account-control'>Новый пароль</Form.Label>
                                <Form.Control
                                    type="password"
                                    value={passwordForm.newPassword}
                                    onChange={event => { setPasswordForm({ password:passwordForm.password, newPassword:event.target.value, confirmPassword: passwordForm.confirmPassword }) }}
                                    id={NEW_PASSWORD_ID}
                                    className={errors.newPassword !== "" ? "main-account-control input-control-error" : "main-account-control"} />
                                <Form.Text>{errors.newPassword}</Form.Text>
                            </Form.Group>
                            <Form.Group className='main-account-col'>
                                <Form.Label className='main-account-control'>Подтверждение</Form.Label>
                                <Form.Control
                                    type="password"
                                    value={passwordForm.confirmPassword}
                                    onChange={event => { setPasswordForm({ password:passwordForm.password, newPassword:passwordForm.newPassword, confirmPassword:event.target.value }) }}
                                    id={CONFIRM_ID}
                                    className={errors.confirmPassword !== "" ? "main-account-control input-control-error" : "main-account-control"} />
                                <Form.Text>{errors.confirmPassword}</Form.Text>
                            </Form.Group>
                            <Button className='main-account-col main-account-button' type='submit' disabled={submitting}>Принять</Button>
                        </Container>
                    </Form.Group>
                </Form>
                {admin ? <AddUser /> : <Container />}
                {admin ? <RemoveUser /> : <Container />}
            </div>
        </WorkingArea>
    );
}

export default AccountPage;