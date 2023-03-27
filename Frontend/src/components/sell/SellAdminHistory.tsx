import './SellPage.css'
import { FC, useState, useEffect, ReactElement } from 'react';
import { Container, Form, Col, Button, Row } from 'react-bootstrap';

import ApiClientWrapper from '../../common/utilites/ApiClientWrapper';
import { selectUserIdentifier } from '../../common/redux/UserSlice';
import { useAppSelector, useAppDispatch } from '../../common/redux/hooks';
import { StorageItemInfoDto } from '../../common/utilites/SwaggerClient';
import { getNearstDay } from './utils';
import { badRequestProcessor } from '../../common/utilites/Common';

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
    const parts = search.toUpperCase().split(':', 2);
    const category = (parts.length === 2 ? parts[0] : "").trimStart();
    const searchPattern = (parts.length === 2 ? parts[1] : search.toUpperCase()).trimStart();
    const result: ISnapshotCategory[] = [];
    snapshots.forEach(item => {
        if (((category !== "" && category === item.category.toUpperCase()) || category === "") && item.name.toUpperCase().startsWith(searchPattern)){
            const index = result.findIndex(c => c.category === item.category);
            const snapahot: ISnapshot = { 
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
    for (let i = 0; i < dates.length; i++) {
        const tmp = dates[i].getTime();
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
    for (let i = 0; i < dates.length; i++) {
        const tmp = dates[i].getTime();
        if (minDate < tmp){
            minDate = tmp;
            locIndex = i;
        }
    }

    return dates[locIndex];
}

const SoldGoodsList: FC<ISnapshotCategory[]> = (categories:ISnapshotCategory[]): ReactElement => {
    const elements = categories.map((snapshot) => 
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
    });
    return(
        <Container className='sell-page-container'>
            {elements}
        </Container>
    )
}

const SellAdminHistory: FC = () => {
    const client = new ApiClientWrapper(useAppDispatch());
    const identifier = useAppSelector(selectUserIdentifier);
    const [date, setDate] = useState(new Date());

    const initStringArray: string[] = [];
    const initNumberArray: number[] = [];
    const initSnapshots: ISnapshotCategory[][] = [];
    const [index, setIndex] = useState(-1);
    const [names, setNames] = useState(initStringArray);
    const [cashValues, setCashValues] = useState(initNumberArray);
    const [writeOff, setWriteOff] = useState(initStringArray);
    const [soldTotal, setSoldTotal] = useState(initNumberArray);
    const [snapshots, setSnapshots] = useState(initSnapshots);

    const initDays: Date[] = [];
    const [days, setDays] = useState(initDays);
    const [dayIndex, setDayIndex] = useState(-1);
    const [search, setSearch] = useState("");
    useEffect(() => {
        const fetchSnapshots = async () => {
            return await client.getFullStatistics(date);
        };
    
        fetchSnapshots().then((response) => {
            const locIncome: number[] = [];
            const locWriteOff: string[] = [];
            const locNames: string[] = [];
            const locCash: number[] = [];
            const snapshots: ISnapshotCategory[][] = [];
            for (let i = 0; i < response.length; i++){
                locNames.push(response[i].name);
                locCash.push(response[i].cash);
                snapshots.push(GetCategories(response[i].snapshots, search));
                locIncome.push(0);
                let locWriteOffRetail = 0;
                let locWriteOffWhole = 0;
                response[i].snapshots.forEach(item => {
                    locIncome[locIncome.length - 1] += item.income;
                    locWriteOffRetail += item.write_off * item.r_price;
                    locWriteOffWhole += item.write_off * item.w_price;
                });

                locWriteOff.push(locWriteOffWhole.toString().concat("/", locWriteOffRetail.toString()))
            }

            setNames(locNames);
            setCashValues(locCash);
            setSnapshots(snapshots);
            setWriteOff(locWriteOff);
            setSoldTotal(locIncome);
            setIndex(locNames.length > 0 ? 0 : -1);
        }).catch(exception => {
            if (!badRequestProcessor(exception)){
                console.error(exception);
            }
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

    useEffect(()=>{
        setDate(getNearstDay(days[dayIndex], days));
    }, [dayIndex])

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
                <Container className='sell-page-item-wide sell-page-full-history-container'>
                    <Col className='sell-page-item-name'>Товары</Col>
                    <Col className='sell-page-item-intermediate'>Единиц на скаладе</Col>
                    <Col className='sell-page-item-intermediate'>Списано опт/розн</Col>
                    <Col className='sell-page-item-intermediate'>Цена опт/розн</Col>
                    <Col className='sell-page-item-intermediate'>Поставки</Col>
                    <Col className='sell-page-category-sold'>Продажа</Col>
                </Container>
                {SoldGoodsList(index > -1 ? snapshots[index] : [])}
            </Container>
            <Form.Group className='snapshots-selector-form snapshots-selector-form-wide' >
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
                <Form.Label className='cash-in-cashbox'>Списание: {index > -1 ? writeOff[index] : "0/0"}</Form.Label>
                <Form.Label className='cash-in-cashbox'>Прибыль: {index > -1 ? soldTotal[index] : 0}</Form.Label>
            </Form.Group>
        </div>);
}

export default SellAdminHistory;
