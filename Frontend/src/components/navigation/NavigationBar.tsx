import './NavigationBar.css'

import React, { FC } from 'react'
import { useFormik, FormikHelpers } from "formik";
import { Navbar, Nav } from 'react-bootstrap'
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import Container from 'react-bootstrap/Container';
import Col from 'react-bootstrap/Col';

import { useAppSelector, useAppDispatch } from '../../common/redux/hooks';
import { selectTitle } from '../../common/redux/TitleSlice';
import { selectUserLogon, selectUserError, signInAsync, selectUserToken, selectUserExpired } from '../../common/redux/UserSlice';
import { SignIn } from '../../common/redux/UserSlice';
import TokenUpdater from '../../common/utilites/TokenService';
import { INDEX, SELLS, INV } from '../../common/utilites/Paths';

import Modal from '../base/modal/Modal';
import Schema from './validation';

const NavigationPanel: FC = () => {
    const title = useAppSelector(selectTitle)
    return(
        <div>
            <Navbar>
                <Navbar.Collapse>
                    <Nav>
                        <Nav.Link className='nav-link' href={INDEX}>{title}</Nav.Link>
                        <Nav.Link className='nav-link' href={SELLS}>Продажи</Nav.Link>
                        <Nav.Link className='nav-link' href={INV}>Инвентаризация</Nav.Link>
                    </Nav>
                </Navbar.Collapse>
            </Navbar>         
        </div>
    )
}

export interface ILoginForm{
    login: string;
    password: string;
}

const NavigationBar: FC = () => {
    const [active, setActive] = React.useState(false);
    const [entered, setEntered] = React.useState(false);
    const title = useAppSelector(selectTitle);
    const logon = useAppSelector(selectUserLogon);
    const token = useAppSelector(selectUserToken);
    const expired = useAppSelector(selectUserExpired);
    const error = useAppSelector(selectUserError);
    const dispatch = useAppDispatch();
    const updater = new TokenUpdater(useAppDispatch());
    React.useEffect(() => {
        if (error !== null && error !== undefined && error !== ""){
            alert(error);
        }
    }, [error]);
    React.useEffect(() =>{
        if (logon){
            updater.set(token, expired);
            updater.update();
        }
        else{
            updater.reset();
        }
    }, [token]);
    const HandleSubmitMain = async (values: ILoginForm, actions: FormikHelpers<ILoginForm> ) => { 
        try{
            let dto = new SignIn()
            dto.login = values.login;
            dto.password = values.password;
            dispatch(signInAsync(dto));
            setActive(false);
        }
        catch (exception){
          alert(exception)
        }
        finally{
          actions.resetForm();
        }
      }
    
    const initialValues: ILoginForm = { login: "", password: "" };
    const { values, errors, touched, isSubmitting, handleBlur, handleChange, handleSubmit, } = useFormik(
        {
            initialValues,
            validationSchema: Schema,
            onSubmit: async (values, actions) => await HandleSubmitMain(values, actions)
        });

    return(
        <div className='navigation' onMouseOver={() => setEntered(true)} onMouseOut={() => setEntered(false)}>
        <Modal active={active} setActive={setActive}>
            <Form onSubmit={values => handleSubmit(values)} autoComplete="off">
                <Container className="mb-3">
                    <Form.Group className={errors.password && touched.password ?"form-group-error": "form-group"} controlId="login">
                        <Form.Control
                            type="text"
                            value={values.login}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            className={errors.login && touched.login ? "input-error" : "form-control-pass"}
                            placeholder="Логин..." />
                        <Form.Text>{errors.login}</Form.Text>
                    </Form.Group>
                    <Form.Group className={errors.password && touched.password ?"form-group-error": "form-group"} controlId="password">
                        <Form.Control
                            type="password"
                            value={values.password}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            className={errors.password && touched.password ? "input-error" : "form-control-pass"}
                            placeholder="Пароль..." />
                        <Form.Text>{errors.password}</Form.Text>
                    </Form.Group>
                    <Col align="center"><Button className='navigation-btn' variant="success" type="submit" disabled={isSubmitting}>Авторизация</Button></Col>
                </Container>
            </Form>
        </Modal>
            {
                entered
                ? logon ? <div><NavigationPanel /></div> : <Button className='navigation-btn' onClick={() => setActive(true)}>Авторизация</Button>
                : <div>{title}</div>
            }
        </div>
    );
}

export default NavigationBar;