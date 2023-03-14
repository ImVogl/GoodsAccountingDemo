import './SellPage.css'
import { FC, useState, useEffect, ReactElement } from 'react';
import { Container, Form, Col, Button } from 'react-bootstrap';

import { ICategory, GetCategories } from '../../common/utilites/Common';

const SellPagePrevious: FC = () => {
    const [date, setDate] = useState(new Date());
    const [search, setSearch] = useState("");
    return(
        <div className='reduced-history-page-list-block'>
            <Col className=''>
                <Form className='search-panel'>
                    <Container className='historysearch-panel-container'>
                        <Col className='historysearch-panel-col-left'>
                            <Form.Group className="mb-3" controlId="search">
                                <Form.Control type="text" className='search-panel control' placeholder="Поиск..." onChange={(event) => setSearch(event.target.value)} />
                            </Form.Group>
                        </Col>
                        <Col className='historysearch-panel-col-right'>
                            <Form.Group className='historysearch-panel-col-right-form' controlId="duedate">
                                <Button type='button' className='left-change-date-button' />
                                <Form.Control type="date" className='history-snapshot-data' value={date.toISOString().split('T')[0]} onChange={(e) => setDate(new Date(e.target.value))} />
                                <Button type='button' className='right-change-date-button' />
                            </Form.Group>
                        </Col>
                    </Container>
                </Form>
            </Col>
            <Container className='sell-page-container'>
                // 
            </Container>
        </div>);
}

export default SellPagePrevious;
