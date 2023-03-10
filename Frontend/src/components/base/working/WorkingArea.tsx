import './WorkingArea.css';
import { FC, ReactElement, PropsWithChildren, useState, useEffect } from 'react'
import { Row, Container, Button, Form } from 'react-bootstrap';

import { getBaseUrl } from '../../../common/utilites/Common';
import { getCurrentToken } from '../../../common/utilites/UpdateTokenService';
import { useAppSelector, useAppDispatch } from '../../../common/redux/hooks';
import { selectShiftUser, selectUserName, selectUserIdentifier, updateShiftState } from '../../../common/redux/UserSlice'
import { ACCOUNT } from '../../../common/utilites/Paths';
import LayoutBase from '../../layouts/BaseLayout';
import Modal from '../modal/Modal';
import { InitClient, Client } from '../../../common/utilites/SwaggerClient';

interface Children { }
const WorkingArea: FC<PropsWithChildren<Children>> = (props: PropsWithChildren<Children>): ReactElement => {
    const [loading, setLoading] = useState(false);
    const dispatcher = useAppDispatch();
    const opened = useAppSelector(selectShiftUser);
    const displayedName = useAppSelector(selectUserName);
    const dentifier = useAppSelector(selectUserIdentifier);
    const [cash, setCash] = useState(0);
    const [active, setActive] = useState(false);
    let clientInit = new InitClient(getBaseUrl());
    let client = new Client(getBaseUrl());
    let shiftButton = opened ? "Закрыть смену" : "Открыть смену";
    useEffect(() => {
        const fetchData = opened
        ? async () => {
            if (cash === undefined){
                alert("Некорректная сумма!");
                throw "Incorrect cash value";
            }

            await client.close(dentifier, cash, getCurrentToken());
            dispatcher(updateShiftState(false));
            setLoading(false);}
        : async () =>{
            await clientInit.shift(dentifier, getCurrentToken());
            dispatcher(updateShiftState(true));
            setLoading(false);
        };

        if (loading) {
            fetchData().catch(console.error);
        }
      }, [loading]);

    return(
        <div>
            <Modal active={active} setActive={setActive}>
                <Form>
                    <Container className="mb-3">
                        <Form.Group className="form-group" controlId="cash">
                            <Form.Label>Остаток в кассе:</Form.Label>
                            <Form.Control type="number" onChange={event => setCash(parseInt(event.target.value))} />
                        </Form.Group>
                        <Button variant="success" type="submit" disabled={loading}>Закрыть смену</Button>
                    </Container>
                </Form>
            </Modal>
            <LayoutBase>
                <div className='working-area-root'>
                    <div className='working-area-block'>
                        {props.children}
                    </div>
                    <div className='working-area-control-block'>
                        <Container className='working-area-container'>
                            <Row className='working-area-row'>
                                <Button className='working-area-button' variant="outline-dark" type="submit" disabled={loading} onClick={!loading ? () => setLoading(true) : () => {}}>{shiftButton}</Button>
                            </Row>
                            <Row className='working-area-row'>
                                <Button className='working-area-button' variant="outline-dark" href={ACCOUNT}>{displayedName}</Button></Row>
                            <Row />
                        </Container>
                    </div>
                </div>
            </LayoutBase>
        </div>
    )
}

export default  WorkingArea;