import './../../Account.css'
import classes from './AddUser.module.css'
import { useAppSelector, useAppDispatch } from '../../../../common/redux/hooks';
import { selectUserIdentifier } from '../../../../common/redux/UserSlice';
import ApiClientWrapper from '../../../../common/utilites/ApiClientWrapper';
import { FC, useState, useEffect } from 'react';
import { Form, Row } from 'react-bootstrap';
import { badRequestProcessor } from '../../../../common/utilites/Common';
import { CommonFloatingLabelInput } from '../../../UI/Form/FloatingLabelInput/CommonFloatingLabelInput';
import { CommonButton } from '../../../UI/Button/CommonButton';

const SURNAME_ID: string = 'surname-new-seller-form';
const NAME_ID: string = 'name-new-seller-form'
const EMAIL_ID: string = 'email-new-seller-form'
const TELEGRAM_ID: string = 'telegram-new-seller-form'

// Обработка запроса на создание нового пользователя.
const handleCreateNewUser = (identifier: number, client: ApiClientWrapper, form: HTMLFormElement, date: Date): Promise<string> => {
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

    return client.addNewUser(identifier, name, surname, date)
        .then(response => `Login: ${response.login}; Password: ${response.password}`)
        .catch(exception => {
            if (badRequestProcessor(exception)){
                return "";
            }
    
            console.error(exception);
            return "";
        });
};

// Компонент добавления нового пользователя.
const AddUser: FC = () => {
    const [date, setDate] = useState(new Date());
    const [submitting, setSubmitting] = useState(false);
    const [message, setMessage] = useState("");
    useEffect(() => {
        if (message !== ""){
            navigator.clipboard.writeText(message)
                .then(() => {
                    alert("Логин и пароль были скопированы в буфер обмена");
                    window.location.reload();
                })
                .catch(() => {
                    alert(message);
                    window.location.reload();
                });
            
        }
    }, [message]);

    const client = new ApiClientWrapper(useAppDispatch());
    const identifier = useAppSelector(selectUserIdentifier);
    return(
        <Form 
            className={classes.container}
            onSubmit={event => {
                event.preventDefault();
                setSubmitting(true);
                let form = event.target as HTMLFormElement;
                handleCreateNewUser(identifier, client, form, date)
                    .then(message => { 
                        setMessage(message);
                        form.reset();
                        setSubmitting(false);
                        })
                    .catch(error => {
                        console.error(error);
                        form.reset();
                        setSubmitting(false);
                    });
                    }
                }
            >
            <Form.Group className={classes.container}>
                <Form.Label className={classes.sublabel}>Добавить нового продавца</Form.Label>
                <Row className={classes.row}>
                    <Form.Group className={classes.column}><CommonFloatingLabelInput label='Email' type="email" id={EMAIL_ID} /></Form.Group>
                    <Form.Group className={classes.column}><CommonFloatingLabelInput label='@telegram' type="text" id={TELEGRAM_ID} /></Form.Group></Row>
                <Row className={classes.row}>
                    <Form.Group className={classes.column}><CommonFloatingLabelInput label='Фамилия' type="text" id={SURNAME_ID} /></Form.Group>
                    <Form.Group className={classes.column}><CommonFloatingLabelInput label='Имя' type="text" id={NAME_ID} /></Form.Group>
                    <Form.Group className={classes.column}>
                        <CommonFloatingLabelInput
                            label='Дата рождения'
                            type="date"
                            value={date.toLocaleDateString("sv")}
                            onChange={event => setDate(new Date(event.target.value))}
                            max={(new Date()).toLocaleDateString("sv")} />
                    </Form.Group>
                    <CommonButton title='Добавить' className={classes.column} type='submit' disabled={submitting} />
                </Row>
            </Form.Group>
        </Form>);
}

export default AddUser;
