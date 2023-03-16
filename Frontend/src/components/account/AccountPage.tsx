import './Account.css'
import { useAppSelector, useAppDispatch } from '../../common/redux/hooks';
import { selectUserName, selectUserIsAdmin } from '../../common/redux/UserSlice';
import { FC, useState, useEffect } from 'react';
import { Form, Container, Button, Dropdown } from 'react-bootstrap';
import WorkingArea from '../base/working/WorkingArea';
import ApiClientWrapper from '../../common/utilites/ApiClientWrapper';

const AddUser: FC = () => {
    const [date, setDate] = useState(new Date());
    return(
        <Form.Group className='main-account-container'>
            <Form.Label className='main-account-sublabel'>Добавить продавца</Form.Label>
            <Container className='main-account-row'>
                <Form.Group className='main-account-col'>
                    <Form.Label className='main-account-control'>Фамилия</Form.Label>
                    <Form.Control className='main-account-control' type="text" />
                </Form.Group>
                <Form.Group className='main-account-col'>
                    <Form.Label className='main-account-control'>Имя</Form.Label>
                    <Form.Control className='main-account-control' type="text" />
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
                <Button className='main-account-col main-account-button'>Добавить</Button>
            </Container>
        </Form.Group>);
}

const RemoveUser: FC = () => {
    const initUsers: string[] = [];
    const client = new ApiClientWrapper(useAppDispatch());
    const [user, setUser] = useState("");
    const [users, setUsers] = useState(initUsers);
    useEffect(() => {
        const fetchusers = async () => {
            return await client.getAllUsers();
        };

        fetchusers().then(users => {
            setUsers(users);
            setUser(users.length > 0 ? users[0] : "");
        });
    })
    return(
        <Form.Group className='main-account-container'>
            <Form.Label className='main-account-sublabel'>Удалить продавца</Form.Label>
            <Container className='main-account-row main-account-row-remove'>
                <Form.Group className='main-account-col'>
                    <Form.Control as='select' className='main-account-control' value={user} onChange={event => setUser(event.target.value)}>
                        {users.map((locUser) => <option value={locUser}>{locUser}</option>)}
                    </Form.Control>
                </Form.Group>
                <Button className='main-account-col main-account-button'>Удалить</Button>
            </Container>
        </Form.Group>);
}

const AccountPage: FC = () => {
    const displayedName = useAppSelector(selectUserName);
    const admin = useAppSelector(selectUserIsAdmin);
    return(
        <WorkingArea>
            <div className='account-page-root'>
                <div className='account-header'>{admin ? "Менеджер" : "Продавец"} {displayedName}</div>
                <Form className='main-account-container'>
                    <Form.Group className='main-account-container'>
                        <Form.Label className='main-account-sublabel'>Изменить пароль</Form.Label>
                        <Container className='main-account-row'>
                            <Form.Group className='main-account-col'>
                                <Form.Label className='main-account-control'>Текущий пароль</Form.Label>
                                <Form.Control className='main-account-control' type="text" />
                            </Form.Group>
                            <Form.Group className='main-account-col'>
                                <Form.Label className='main-account-control'>Новый пароль</Form.Label>
                                <Form.Control className='main-account-control' type="text" />
                            </Form.Group>
                            <Form.Group className='main-account-col'>
                                <Form.Label className='main-account-control'>Подтверждение</Form.Label>
                                <Form.Control className='main-account-control' type="text" />
                            </Form.Group>
                            <Button className='main-account-col main-account-button'>Принять</Button>
                        </Container>
                    </Form.Group>
                    {admin ? <AddUser /> : <Container />}
                    {admin ? <RemoveUser /> : <Container />}
                </Form>
            </div>
        </WorkingArea>
    );
}

export default AccountPage;