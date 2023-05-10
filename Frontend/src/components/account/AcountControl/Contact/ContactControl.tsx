import classes from './ContactControl.module.css'
import { useAppSelector, useAppDispatch } from '../../../../common/redux/hooks';
import { selectUserIdentifier } from '../../../../common/redux/UserSlice';
import ApiClientWrapper from '../../../../common/utilites/ApiClientWrapper';
import { FC, useState } from 'react';
import { Form, Row } from 'react-bootstrap';
import { badRequestProcessor } from '../../../../common/utilites/Common';
import { CommonFloatingLabelInput } from '../../../UI/Form/FloatingLabelInput/CommonFloatingLabelInput';
import { CommonButton } from '../../../UI/Button/CommonButton';

const EMAIL_ID: string = 'email-contact-data';
const TELEGRAM_ID: string = 'telegram-contact-data';

// Обработка запроса на создание нового пользователя.
const handleUpdateContactData = (identifier: number, client: ApiClientWrapper, form: HTMLFormElement): Promise<string> => {
    let email:string = "";
    let telegram: string = "";
    for (let i = 0; i < form.length; i++){
        if (form[i].localName !== "input"){
            continue;
        }

        let element = (form[i] as HTMLInputElement);
        if (element.id === EMAIL_ID){
            email = element.value;
        }

        if (element.id === TELEGRAM_ID){
            telegram = element.value;
        }
    };

    return client.addNewUser(identifier, email, telegram, new Date())
        .then(response => `Login: ${response.login}; Password: ${response.password}`)
        .catch(exception => {
            if (badRequestProcessor(exception)){
                return "";
            }
    
            console.error(exception);
            return "";
        });
};

// Компонента с контактными сведениями. 
const ContactControl: FC = () => {
    const [submitting, setSubmitting] = useState(false);
    const client = new ApiClientWrapper(useAppDispatch());
    const identifier = useAppSelector(selectUserIdentifier);
    return(
        <Form 
            className={classes.container}
            onSubmit={event => {
                event.preventDefault();
                setSubmitting(true);
                let form = event.target as HTMLFormElement;
                handleUpdateContactData(identifier, client, form)
                    .then(() => {
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
                <Form.Label className={classes.sublabel}>Изменить контактные данные</Form.Label>
                <Row className={classes.row}>
                    <Form.Group className={classes.column}><CommonFloatingLabelInput label='Email' type="email" id={EMAIL_ID} /></Form.Group>
                    <Form.Group className={classes.column}><CommonFloatingLabelInput label='@telegram' type="text" id={TELEGRAM_ID} /></Form.Group>
                    <CommonButton title='Сохранить' className={classes.column} type='submit' disabled={submitting} />
                </Row>
            </Form.Group>
        </Form>);
}

export default ContactControl;