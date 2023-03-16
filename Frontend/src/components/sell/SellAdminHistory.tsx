import './SellPage.css'
import { FC, useState, useEffect, ReactElement } from 'react';
import { Container, Form, Col, Button, Row } from 'react-bootstrap';

import ApiClientWrapper from '../../common/utilites/ApiClientWrapper';
import { selectUserIdentifier } from '../../common/redux/UserSlice';
import { useAppSelector, useAppDispatch } from '../../common/redux/hooks';
import { StorageItemInfoDto } from '../../common/utilites/SwaggerClient';

interface ISnapshot{
    id: string;
    name: string;
    sold: number;
    write_off: number;
    receipt: number;
    storage: number;
    r_price: number;
    w_price: number;
    income: number;
    wsp_spending: number;
    wow_los: number;
    wor_los: number;
}

interface ISnapshotCategory{
    category:string;
    income: number;
    wsp_spending: number;
    wow_los: number;
    wor_los: number;
    snapshots: ISnapshot[];
}

// Search function
export function GetCategories(snapshots: StorageItemInfoDto[], search: string):ISnapshotCategory[]
{
    let parts = search.toUpperCase().split(':', 2);
    let category = (parts.length === 2 ? parts[0] : "").trimStart();
    let searchPattern = (parts.length === 2 ? parts[1] : search.toUpperCase()).trimStart();
    let result: ISnapshotCategory[] = []
    snapshots.forEach(item => {
        if (((category !== "" && category === item.category.toUpperCase()) || category === "") && item.name.toUpperCase().startsWith(searchPattern)){
            let index = result.findIndex(c => c.category === item.category)
            let snapahot: ISnapshot = { 
                id: item.id,
                name: item.name,
                sold: item.sold,
                write_off: item.write_off,
                receipt: item.receipt,
                storage: item.storage,
                r_price: item.r_price,
                w_price: item.w_price,
                income: item.income,
                wsp_spending: item.wsp_spending,
                wow_los: item.wow_los,
                wor_los: item.wor_los
            };

            if (index >= 0){
                result[index].income += item.income;
                result[index].wsp_spending += item.wsp_spending;
                result[index].wow_los += item.wow_los;
                result[index].wor_los += item.wor_los;
                result[index].snapshots.push(snapahot);
            }
            else{
                result.push({ 
                    category: item.category,
                    income: item.income,
                    wsp_spending: item.wsp_spending,
                    wow_los: item.wow_los,
                    wor_los: item.wor_los,
                    snapshots: [snapahot]
                })
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
    let elements = categories.map((snapshot) => 
    {
        return (
            <div className='sell-page-row-block' key={snapshot.category.concat("-div")}>
                <Row className="sell-page-category-wide" key={snapshot.category}>
                    <Col>{snapshot.category}</Col>
                    <Col />
                    <Col className='sell-page-item-intermediate'>{snapshot.wow_los}/{snapshot.wor_los} руб</Col>
                    <Col />
                    <Col className='sell-page-item-intermediate'>{snapshot.wsp_spending} руб</Col>
                    <Col className='sell-page-category-sold'>{snapshot.income} руб</Col>
                </Row>
                {
                    snapshot.snapshots.map((item) => {
                        return(
                            <Row className='sell-page-item-wide' key = {item.id}>
                                <Col className='sell-page-item-name'>{item.name}</Col>
                                <Col className='sell-page-item-intermediate'>{item.storage}</Col>
                                <Col className='sell-page-item-intermediate'>{item.wow_los}/{item.wor_los} руб({item.write_off})</Col>
                                <Col className='sell-page-item-intermediate'>{item.w_price}/{item.r_price} руб</Col>
                                <Col className='sell-page-item-intermediate'>{item.wsp_spending} руб({item.receipt})</Col>
                                <Col className='sell-page-item-sold'>{item.income} руб({item.sold})</Col>
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

const SellAdminHistory: FC = () => {
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
            return await client.getFullStatistics(identifier, date);
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

    const setNearstDay = (day: string | Date) =>{
        if (days.length === 0){
            setDate(new Date());
            return;
        }

        var targetDate = typeof day === "string" ? new Date(day) : day;
        let min = Math.abs(targetDate.getTime() - days[0].getTime());
        let locIndex = 0;
        for (let i = 0; i < days.length; i++){
            let diff = Math.abs(targetDate.getTime() - days[i].getTime());
            if (min > diff){
                min = diff;
                locIndex = i;
            }
        }

        setDate(days[locIndex]);
    };
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
                                    max={getMaxDate(days).toISOString().split('T')[0]}
                                    min={getMinDate(days).toISOString().split('T')[0]}
                                    value={getMinDate(days).toISOString().split('T')[0]}
                                    onChange={(e) => setNearstDay(e.target.value)} />
                                <Button type='button' className='right-change-date-button' onClick={() => {dayIndex === days.length - 1 ? setDayIndex(days.length - 1) : setDayIndex(dayIndex + 1)}} />
                            </Form.Group>
                        </Col>
                    </Container>
                </Form>
            </Col>
            <Container className='sell-page-container'>
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

export default SellAdminHistory;
