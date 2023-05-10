import classes from './Account.module.css';
import { useAppSelector } from '../../common/redux/hooks';
import { selectUserName, selectUserIsAdmin } from '../../common/redux/UserSlice';
import { FC } from 'react';
import { Container } from 'react-bootstrap';
import AddUser from './ManagerPanel/Add/AddUser';
import RemoveUser from './ManagerPanel/Remove/RemoveUser';
import PasswordChange from './AcountControl/Password/PasswordChangeControl';
import ContactControl from './AcountControl/Contact/ContactControl';
import LayoutBase from '../layouts/BaseLayout';

const AccountPage: FC = () => {
    const displayedName = useAppSelector(selectUserName);
    const admin = useAppSelector(selectUserIsAdmin);
    return(
        <LayoutBase>
            <div className={classes.root}>
                <div className={classes.accountHeader}>{displayedName}</div>
                <div className={classes.accountRole}>{admin ? "Менеджер" : "Продавец"}</div>
                <br />
                <div className={classes.header}>Управление аккаунтом</div>
                <ContactControl />
                <PasswordChange />
                <div className={classes.header}>Администрирование</div>
                {admin ? <AddUser /> : <Container />}
                {admin ? <RemoveUser /> : <Container />}
            </div>
        </LayoutBase>
    );
}

export default AccountPage;