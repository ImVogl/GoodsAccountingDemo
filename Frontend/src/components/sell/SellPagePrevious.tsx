import './SellPage.css'
import { FC, useState, useEffect, ReactElement } from 'react';
import { Container, Form, Col, Button, Row } from 'react-bootstrap';

import ApiClientWrapper from '../../common/utilites/ApiClientWrapper';
import { selectUserIdentifier } from '../../common/redux/UserSlice';
import { useAppSelector, useAppDispatch } from '../../common/redux/hooks';
import { IReducedItemInfoDto } from '../../common/utilites/SwaggerClient';
import { getNearstDay } from './utils';

interface ISnapshot{
    id: string;
    name: string;
    sold: number;
    price: number;
}

interface ISnapshotCategory{
    category:string;
    sold: number;
    snapshots: ISnapshot[];
}

// Search function
export function GetCategories(snapshots: IReducedItemInfoDto[], search: string):ISnapshotCategory[]
{
    let parts = search.toUpperCase().split(':', 2);
    let category = (parts.length === 2 ? parts[0] : "").trimStart();
    let searchPattern = (parts.length === 2 ? parts[1] : search.toUpperCase()).trimStart();
    let result: ISnapshotCategory[] = []
    snapshots.forEach(item => {
        if (((category !== "" && category === item.category.toUpperCase()) || category === "") && item.name.toUpperCase().startsWith(searchPattern)){
            let index = result.findIndex(c => c.category === item.category)
            if (index >= 0){
                result[index].sold += item.sold;
                result[index].snapshots.push({ id: item.id, name: item.name, sold: item.sold, price: item.price })
            }
            else{
                result.push({ category: item.category, sold: item.sold, snapshots: [{ id: item.id, name: item.name, sold: item.sold, price: item.price }] })
            }
        }
    });

    return result;
}

function getMinDate(dates:Date[]):Date{
    if (dates.length === 0){
        return new Date();
    }

    let locIndex = 0;
    let minDate = dates[0].getTime();
    for (let i = 0; i < dates.length; i++){
        let tmp = dates[i].getTime();
        if (minDate > tmp){
            minDate = tmp;
            locIndex = i;
        }
    }
    
    return dates[locIndex];
}

function getMaxDate(dates:Date[]):Date{
    if (dates.length === 0){
        return new Date();
    }

    let locIndex = 0;
    let minDate = dates[0].getTime();
    for (let i = 0; i < dates.length; i++){
        let tmp = dates[i].getTime();
        if (minDate < tmp){
            minDate = tmp;
            locIndex = i;
        }
    }
    
    return dates[locIndex];
}

const SoldGoodsList: FC<ISnapshotCategory[]> = (categories:ISnapshotCategory[]): ReactElement => {
    let sold: Map<string, number> = new Map<string, number>();
    for (let i = 0; i < categories.length; i++){
        for (let j = 0; j < categories[i].snapshots.length; j++){
            sold.set(categories[i].snapshots[j].id, 0);
        }
    }
    
    let elements = categories.map((snapshot) => 
    {
        return (
            <div className='sell-page-row-block' key={snapshot.category.concat("-div")}>
                <Row className="sell-page-category" key={snapshot.category}>
                    <Col>{snapshot.category}</Col>
                    <Col className='sell-page-category-sold'>{snapshot.sold}</Col>
                </Row>
                {
                    snapshot.snapshots.map((item) => {
                        return(
                            <Row className='sell-page-item' key = {item.id}>
                                <Col className='sell-page-item-name'>{item.name}</Col>
                                <Col className='sell-page-item-intermediate'>{item.price}</Col>
                                <Col className='sell-page-item-sold'>{item.sold}</Col>
                            </Row>
                        )
                    })
                }
            </div>
        )
    })
    return(
        <Container className='sell-page-container'>
            {elements}
        </Container>
    )
}

const SellPagePrevious: FC = () => {
    const dispatcher = useAppDispatch();
    let client = new ApiClientWrapper(dispatcher);
    const identifier = useAppSelector(selectUserIdentifier);
    const [date, setDate] = useState(new Date());

    const initNames: string[] = [];
    const initCashValues: number[] = [];
    const initSnapshots: ISnapshotCategory[][] = [];
    const [index, setIndex] = useState(-1);
    const [names, setNames] = useState(initNames);
    const [cashValues, setCashValues] = useState(initCashValues);
    const [snapshots, setSnapshots] = useState(initSnapshots);

    let initDays: Date[] = [];
    const [days, setDays] = useState(initDays);
    const [dayIndex, setDayIndex] = useState(-1);
    const [search, setSearch] = useState("");
    useEffect(() => {
        const fetchSnapshots = async () => {
            return await client.getStatistics(identifier, date);
        };
    
        fetchSnapshots().then((response) => {
            let locNames: string[] = [];
            let locCash: number[] = [];
            let snapshots: ISnapshotCategory[][] = [];
            for (let i = 0; i < response.length; i++){
                locNames.push(response[i].name);
                locCash.push(response[i].cash);
                snapshots.push(GetCategories(response[i].snapshots, search));
            }

            setNames(locNames);
            setCashValues(locCash);
            setSnapshots(snapshots);
            setIndex(locNames.length > 0 ? 0 : -1);
        })
    }, [date]);

    useEffect(() => {
        const shiftDays = async () => {
            return await client.getShiftDays(identifier);
        };

        shiftDays().then(dates => {
            setDays(dates);
            setDayIndex(dates.length > 0 ? 0 : -1);
        });
    }, []);
    return(
        <div className='reduced-history-page-list-block'>
            <Col className=''>
                <Form className='search-panel'>
                    <Container className='historysearch-panel-container'>
                        <Col className='historysearch-panel-col-left'>
                            <Form.Group className="mb-3" controlId="search">
                                <Form.Control
                                    type="text"
                                    className='search-panel control'
                                    placeholder="Поиск..."
                                    onChange={(event) => setSearch(event.target.value)} />
                            </Form.Group>
                        </Col>
                        <Col className='historysearch-panel-col-right'>
                            <Form.Group className='historysearch-panel-col-right-form' >
                                <Button type='button' className='left-change-date-button' onClick={() => {dayIndex === 0 ? setDayIndex(0) : setDayIndex(dayIndex - 1)}} />
                                <Form.Control
                                    type="date"
                                    className='history-snapshot-data'
                                    max={getMaxDate(days).toLocaleDateString("sv")}
                                    min={getMinDate(days).toLocaleDateString("sv")}
                                    value={date.toLocaleDateString("sv")}
                                    onChange={(e) => setDate(getNearstDay(e.target.value, days))} />
                                <Button type='button' className='right-change-date-button' onClick={() => {dayIndex === days.length - 1 ? setDayIndex(days.length - 1) : setDayIndex(dayIndex + 1)}} />
                            </Form.Group>
                        </Col>
                    </Container>
                </Form>
            </Col>
            <Container className='sell-page-container'>
                <Container className='sell-page-item sell-page-full-history-container'>
                    <Col className='sell-page-item-name'>Товары</Col>
                    <Col className='sell-page-item-intermediate'>Цена</Col>
                    <Col className='sell-page-category-sold'>Продажа</Col>
                </Container>
                {SoldGoodsList(index > -1 ? snapshots[index] : [])}
            </Container>
            <Form.Group className='snapshots-selector-form' >
                <Button
                    type='button'
                    className='left-change-date-button'
                    onClick={() => {index === 0 ? setIndex(0) : setIndex(index - 1)}}
                    disabled={index === 0} />
                <Form.Label className='snapshot-seller'>{index > -1 ? names[index] : ""}</Form.Label>
                <Button
                    type='button'
                    className='right-change-date-button'
                    onClick={() => {index === names.length - 1 ? setIndex(0) : setIndex(index + 1)}}
                    disabled={index === names.length - 1} />
                <Form.Label className='cash-in-cashbox'>Остаток в касе: {index > -1 ? cashValues[index] : 0}</Form.Label>
            </Form.Group>
        </div>);
}

export default SellPagePrevious;
