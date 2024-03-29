import './Inventarisation.css';
import { FC, useState, useEffect } from 'react';
import { Container, Col, Row, Form, Button } from 'react-bootstrap';

import WorkingArea from '../base/working/WorkingArea';
import InvestitionSupply from './InventarisationSupply';
import InvestitionRevision from './InventarisationRevision';
import InvestitionEditing from './InventarisationEditing';

const KEY: string = "inventarisation_current_area"
const SUPPLY:string = "Поставки";
const REVISION:string = "Ревизия";
const EDITING:string = "Редактирование";

function GetNext(current: string): string{
    if (current === SUPPLY){
        return REVISION;
    }

    if (current === REVISION){
        return EDITING;
    }

    return SUPPLY;
}

function GetPrevious(current: string): string{
    if (current === REVISION){
        return SUPPLY;
    }

    if (current === EDITING){
        return REVISION;
    }

    return EDITING
}

function GetCurrentArea(current: string){
    if (current === REVISION){
        return <InvestitionRevision />
    }

    if (current === EDITING){
        return <InvestitionEditing />
    }

    return <InvestitionSupply />
}

const InventarisationRoot: FC = () => {
    const [title, setTitle] = useState(localStorage.getItem(KEY) ?? SUPPLY);
    useEffect(() => {
        localStorage.setItem(KEY, title)
    }, [title]);
    
    return(
        <WorkingArea>
            <Container className='inventarisation-page-root'>
                <Row className='inventarisation-page-row'>
                    <Col>
                        <Form.Group className='historysearch-panel-col-right-form' >
                            <Button type='button' className='left-change-date-button' disabled={title === SUPPLY} onClick={() => setTitle(GetPrevious(title))} />
                            <Form.Control type="text" className='inventarisation-page-part' value={title} readOnly />
                            <Button type='button' className='right-change-date-button' disabled={title === EDITING} onClick={() => setTitle(GetNext(title))} />
                        </Form.Group>
                    </Col>
                    <Col />
                </Row>
                <Row className='investition-table-base'>
                    {GetCurrentArea(title)}
                </Row>
            </Container>
        </WorkingArea>
    )
}

export default InventarisationRoot;