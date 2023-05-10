import './Account.css';
import classes from './Account.module.css';
import { useAppSelector } from '../../common/redux/hooks';
import { selectUserName, selectUserIsAdmin } from '../../common/redux/UserSlice';
import { FC } from 'react';
import { Container } from 'react-bootstrap';
import WorkingArea from '../base/working/WorkingArea';
import AddUser from './ManagerPanel/Add/AddUser';
import RemoveUser from './ManagerPanel/Remove/RemoveUser';
import PasswordChange from './AcountControl/Password/PasswordChangeControl';
import ContactControl from './AcountControl/Contact/ContactControl';

const AccountPage: FC = () => {
    const displayedName = useAppSelector(selectUserName);
    const admin = useAppSelector(selectUserIsAdmin);
    return(
        <WorkingArea>
            <div className='account-page-root'>
                <div className='account-header'>{admin ? "Менеджер" : "Продавец"} {displayedName}</div>
                <div className={classes.header}>Управление аккаунтом</div>
                <ContactControl />
                <PasswordChange />
                <div className={classes.header}>Администрирование</div>
                {admin ? <AddUser /> : <Container />}
                {admin ? <RemoveUser /> : <Container />}
            </div>
        </WorkingArea>
    );
}

export default AccountPage;