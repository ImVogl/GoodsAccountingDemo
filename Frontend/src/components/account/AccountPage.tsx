import './Account.css'
import { useAppSelector, useAppDispatch } from '../../common/redux/hooks';
import { selectUserName, selectUserIsAdmin, selectUserIdentifier } from '../../common/redux/UserSlice';
import { FC, useState, useEffect } from 'react';
import { Form, Container, Button } from 'react-bootstrap';
import WorkingArea from '../base/working/WorkingArea';
import ApiClientWrapper from '../../common/utilites/ApiClientWrapper';
import Schema from './validator'
import { ValidationError } from 'yup';

interface IUser{
    id: number;
    login: string;
}

class User implements IUser{
    id!: number;
    login!: string; 
}

export interface IChangePasswordForm{
    password: string;
    newPassword: string;
    confirmPassword: string;
}

const PASSWORD_ID: string = 'password-group-form';
const NEW_PASSWORD_ID: string = 'new-password-group-form';
const CONFIRM_ID: string = 'confirm-password-group-form';
const SURNAME_ID: string = 'surname-password-group-form';
const NAME_ID: string = 'name-password-group-form'

const handleCreateNewUser = async (identifier: number, client: ApiClientWrapper, form: HTMLFormElement, date: Date): Promise<string> => {
    let name:string = "";
    let surname: string = "";
    for (let i = 0; i < form.length; i++){
        if (form[i].localName !== "input"){
            continue;
        }

        let element = (form[i] as HTMLInputElement);
        if (element.id === NAME_ID){
            name = element.value;
        }

        if (element.id === SURNAME_ID){
            surname = element.value;
        }
    };

    try {
        const response = await client.addNewUser(identifier, name, surname, date);
        return `Login: ${response.login}; Password: ${response.password}`;
    } catch (exception) {
        console.error(exception);
        return "";
    }
};

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
        console.error(exception);
        alert("Не удалось изменить пароль!");
    }
};

const handleRemoveUser = async (client: ApiClientWrapper, id:number): Promise<void> =>{
    try{
        await client.removeUser(id);
    }
    catch (exception){
        console.error(exception);
        alert("Не удалось удалить пользователя")
    };
}

const AddUser: FC = () => {
    const [date, setDate] = useState(new Date());
    const [submitting, setSubmitting] = useState(false);
    const [message, setMessage] = useState("");
    useEffect(() => {
        if (message !== ""){
            navigator.clipboard.writeText(message);
        }
    }, [message]);
    const client = new ApiClientWrapper(useAppDispatch());
    const identifier = useAppSelector(selectUserIdentifier);
    return(
        <Form 
            className='main-account-container'
            autoComplete="off"
            onSubmit={event => {
                setSubmitting(true);
                let form = event.target as HTMLFormElement;
                handleCreateNewUser(identifier, client, form, date)
                    .then(message => { 
                        setMessage(message);
                        setSubmitting(false);
                        })
                    .catch(error => console.error(error));
                    }
                }
            >
            <Form.Group className='main-account-container'>
                <Form.Label className='main-account-sublabel'>Добавить продавца</Form.Label>
                <Container className='main-account-row'>
                    <Form.Group className='main-account-col'>
                        <Form.Label className='main-account-control'>Фамилия</Form.Label>
                        <Form.Control className='main-account-control' type="text" id = {SURNAME_ID} />
                    </Form.Group>
                    <Form.Group className='main-account-col'>
                        <Form.Label className='main-account-control'>Имя</Form.Label>
                        <Form.Control className='main-account-control' type="text" id = {NAME_ID} />
                    </Form.Group>
                    <Form.Group className='main-account-col'>
                        <Form.Label className='main-account-control'>Дата рождения</Form.Label>
                        <Form.Control
                            className='main-account-control'
                            type="date"
                            value={date.toLocaleDateString("sv")}
                            onChange={event => setDate(new Date(event.target.value))}
                            max={(new Date()).toLocaleDateString("sv")} />
                    </Form.Group>
                    <Button className='main-account-col main-account-button' type='submit' disabled={submitting}>Добавить</Button>
                </Container>
            </Form.Group>
        </Form>);
}

const RemoveUser: FC = () => {
    const initUsers: IUser[] = [];
    const initUser = new User();
    const client = new ApiClientWrapper(useAppDispatch());
    const [user, setUser] = useState(initUser);
    const [users, setUsers] = useState(initUsers);
    const [submitting, setSubmitting] = useState(false);
    useEffect(() => {
        const fetchusers = async () => {
            return await client.getAllUsers();
        };

        fetchusers().then(users => {
            setUsers(users);
            let existUser = users.find(item => item.id === user.id);
            if (existUser !== undefined){
                return;
            }

            let locUser = new User();
            locUser.id = users.length > 0 ? users[0].id :-1;
            locUser.login = users.length > 0 ? users[0].login : "";
            setUser(locUser);
        });
    }, [user.login, user.id]);

    return(
        <Form
            className='main-account-container'
            onSubmit={async () => {
                setSubmitting(true);
                await handleRemoveUser(client, user.id);
                setSubmitting(false);}}
            >
            <Form.Group className='main-account-container'>
                <Form.Label className='main-account-sublabel'>Удалить продавца</Form.Label>
                <Container className='main-account-row main-account-row-remove'>
                    <Form.Group className='main-account-col'>
                        <Form.Control
                            as='select'
                            className='main-account-control'
                            value={user.login}
                            onChange={event => {
                                let tmpUser = users.find(item => item.login === event.target.value);
                                if (tmpUser !== undefined){
                                    setUser(tmpUser);
                                }
                                }}>
                            {users.map((locUser) => <option value={locUser.login} key={locUser.id}>{locUser.login}</option>)}
                        </Form.Control>
                    </Form.Group>
                    <Button className='main-account-col main-account-button' type='submit' disabled={submitting}>Удалить</Button>
                </Container>
            </Form.Group>
        </Form>);
}

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