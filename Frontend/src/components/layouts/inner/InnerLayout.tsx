import './InnerLayout.css';
import { FC, ReactElement, PropsWithChildren } from 'react'
import { Row, Container, Button } from 'react-bootstrap';

import { useAppSelector } from '../../../common/redux/hooks';
import { selectShiftUser, selectUserName } from '../../../common/redux/UserSlice'
import { ACCOUNT } from '../../../common/utilites/Paths';
import LayoutBase from '../base/BaseLayout';

interface Children { }
const InnerLayout: FC<PropsWithChildren<Children>> = (props: PropsWithChildren<Children>): ReactElement => {
    const opened = useAppSelector(selectShiftUser);
    const displayedName = useAppSelector(selectUserName);
    let shiftButton = opened ? "Закрыть смену" : "Открыть смену";
    return(
        <LayoutBase>
            <div className='root'>
                <div className='working-block'>
                    {props.children}
                </div>
                <div className='control-block'>
                    <Container>
                        <Row><Button variant="outline-dark" type="submit">{shiftButton}</Button></Row>
                        <Row><Button variant="outline-dark" href={ACCOUNT}>{displayedName}</Button></Row>
                        <Row />
                    </Container>
                </div>
            </div>
        </LayoutBase>
    )
}

export default  InnerLayout;