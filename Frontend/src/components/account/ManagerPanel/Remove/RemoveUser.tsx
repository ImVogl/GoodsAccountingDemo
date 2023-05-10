import classes from './RemoveUser.module.css'
import ApiClientWrapper from '../../../../common/utilites/ApiClientWrapper';
import { useAppDispatch } from '../../../../common/redux/hooks';
import { badRequestProcessor } from '../../../../common/utilites/Common';
import { Form, Container, Button } from 'react-bootstrap';
import { FC, useState, useEffect } from 'react';
import { CommonButton } from '../../../UI/Button/CommonButton';

// Интерфейс входного параметра со сведениями о пользователе.
interface IUser{
    id: number;
    login: string;
}

// Входные параметры со сведениями о пользователе.
class User implements IUser{
    id!: number;
    login!: string; 
}

// Обработчик события, нажатия кнопки удалить пользователя.
const handleRemoveUser = async (client: ApiClientWrapper, id:number): Promise<void> =>{
    try{
        await client.removeUser(id);
    }
    catch (exception){
        if (!badRequestProcessor(exception)){
            console.error(exception);
            alert("Не удалось удалить пользователя");
        }
    };
}

// Компонента с формой удаления пользователя. 
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
        }).catch(exception => badRequestProcessor(exception))
    }, [user.login, user.id]);

    return(
        <Form
            className={classes.container}
            onSubmit={async () => {
                setSubmitting(true);
                await handleRemoveUser(client, user.id);
                setSubmitting(false);}}
            >
            <Form.Group className={classes.container}>
                <Form.Label className={classes.sublabel}>Удалить продавца</Form.Label>
                <Container className={classes.row}>
                    <Form.Group className={classes.column}>
                        <Form.Control
                            as='select'
                            className={classes.control}
                            value={user.login}
                            onChange={
                                event => 
                                {
                                    let tmpUser = users.find(item => item.login === event.target.value);
                                    if (tmpUser !== undefined){
                                        setUser(tmpUser);
                                    }
                                }
                            }>
                            {users.map((locUser) => <option value={locUser.login} key={locUser.id} className={classes.listOption}>{locUser.login}</option>)}
                        </Form.Control>
                    </Form.Group>
                    <CommonButton title={"Удалить"} className={classes.column} type='submit' disabled={submitting} />
                </Container>
            </Form.Group>
        </Form>);
}

export default RemoveUser;
