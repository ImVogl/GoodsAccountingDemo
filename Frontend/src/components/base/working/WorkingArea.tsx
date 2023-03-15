import './WorkingArea.css';
import { FC, ReactElement, PropsWithChildren, useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom';
import { Row, Container, Button, Form } from 'react-bootstrap';

import { useAppSelector, useAppDispatch } from '../../../common/redux/hooks';
import { selectShiftUser, selectUserName, selectUserIdentifier } from '../../../common/redux/UserSlice'
import { ACCOUNT, INDEX } from '../../../common/utilites/Paths';
import LayoutBase from '../../layouts/BaseLayout';
import Modal from '../modal/Modal';
import ApiClientWrapper from '../../../common/utilites/ApiClientWrapper';
import TokenService from '../../../common/utilites/TokenService';

interface Children { }
const WorkingArea: FC<PropsWithChildren<Children>> = (props: PropsWithChildren<Children>): ReactElement => {
    const [loading, setLoading] = useState(false);
    const [closing, setClosing] = useState(false);
    const navigate = useNavigate();
    const dispatcher = useAppDispatch();
    const opened = useAppSelector(selectShiftUser);
    const displayedName = useAppSelector(selectUserName);
    const identifier = useAppSelector(selectUserIdentifier);
    const [cash, setCash] = useState(0);
    const [active, setActive] = useState(false);
    let client = new ApiClientWrapper();
    let tokenService = new TokenService(dispatcher);
    let shiftButton = opened ? "Закрыть смену" : "Открыть смену";
    useEffect(() => {
        const fetchData = async () => {
            if (cash === undefined || cash < 0){
                setActive(false);
                alert("Некорректная сумма!");
                throw "Incorrect cash value";
            }

            try{
                await client.closeWorkingShift(identifier, cash);
                tokenService.reset();
                navigate(INDEX);
            }
            catch (error){
                alert(error);
                console.error(error)
            }
            finally{
                setActive(false);
                setClosing(false);
            }
            
        };

        if (closing && opened && active) {
            fetchData().catch(console.error);
        }

    }, [closing]);

    useEffect(() => {
        const fetchData = async () =>{
            await client.initWorkingShift(identifier);
            setLoading(false);
        };

        if (loading) {
            if (opened){
                setActive(true);
                setLoading(false);
            }
            else{
                fetchData().catch(console.error);
            }
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
                        <Button className='working-area-button' variant="success" type="submit" disabled={closing} onClick={() => setClosing(true)}>Закрыть смену</Button>
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