import './Investition.css';
import { FC, ReactElement, useEffect, useState } from 'react';
import { Container, Col, Row, Form, Button, ButtonGroup } from 'react-bootstrap';
import ApiClientWrapper from '../../common/utilites/ApiClientWrapper';
import { useAppDispatch, useAppSelector } from '../../common/redux/hooks';
import { selectUserIdentifier } from '../../common/redux/UserSlice';
import { GoodsRevisionDto, RevisionGoodsItemDto, IGoodsItemDto } from '../../common/utilites/SwaggerClient';

const GROUP_POSTFIX: string = '.category';
const STORAGE_POSTFIX: string = '.storage';
const PRICE_POSTFIX: string = '.price';
const WRITE_OFF_POSTFIX: string = '.write_off';

function extractPostfix(fullId: string): string{
    if (fullId.endsWith(GROUP_POSTFIX)){
        return GROUP_POSTFIX;
    }

    if (fullId.endsWith(STORAGE_POSTFIX)){
        return STORAGE_POSTFIX;
    }
    
    if (fullId.endsWith(PRICE_POSTFIX)){
        return PRICE_POSTFIX;
    }
    
    if (fullId.endsWith(WRITE_OFF_POSTFIX)){
        return WRITE_OFF_POSTFIX;
    }

    return "";
}

function extractItemId(fullId: string): string{
    let postfix = extractPostfix(fullId);
    return postfix === "" ? "" : fullId.replace(postfix, '');
}

export async function sendSupplyAsync(client: ApiClientWrapper, identifier: number, form: HTMLFormElement):Promise<void>{
    let dto = new GoodsRevisionDto();
    dto.id = identifier;
    let grouppedValues: { [id: string] : {[type: string]: number}; } = {};
    let categories: { [id: string] : string; } = {};
    for (let i = 0; i < form.length; i++){
        if (form[i].localName !== "input"){
            continue;
        }

        let postfix = extractPostfix(form[i].id)
        if (postfix === ''){
            continue;
        }

        let value = (form[i] as HTMLInputElement).value;
        let defaultValue = (form[i] as HTMLInputElement).defaultValue;
        if (postfix === GROUP_POSTFIX){
            categories[extractItemId(form[i].id)] = (value !== null && value !== undefined) ? value : defaultValue;
            continue;
        }
        
        if (value === null || value === undefined || value === ""){
            value = defaultValue;
        }

        try{
            let number = parseInt(value);
            if (Number.isNaN(number)){
                continue;
            }
            if (number < 0){
                number = 0;
            }

            grouppedValues[extractItemId(form[i].id)][postfix] = number;
        } 
        catch (error){
            console.error(error);
            alert("Не удалось отправить сведения о проданных товарах.");
            return;
        }
    };

    for (let identifier in grouppedValues){
        let item = new RevisionGoodsItemDto();
        item.id = identifier;
        item.category = categories[identifier];
        item.storage = grouppedValues[identifier][STORAGE_POSTFIX];
        item.price = grouppedValues[identifier][PRICE_POSTFIX];
        item.write_off = grouppedValues[identifier][WRITE_OFF_POSTFIX];
        dto.items.push(item);
    }

    try{
        await client.revision(dto);
    }
    catch (error){
        console.error(error);
        alert("Не удалось отправить сведения о проданных товарах.");
    }
}

const RevisionGoodsList: FC<IGoodsItemDto[]> = (goods:IGoodsItemDto[]): ReactElement => {
    let editedGoods:IGoodsItemDto[] = [];
    for (let i = 0; i < goods.length; i++){
        if (goods[i].active){
            editedGoods.push(goods[i]);
        }
    }
    const [sending, setSending] = useState(false);
    let client = new ApiClientWrapper(useAppDispatch());
    const identifier = useAppSelector(selectUserIdentifier);
    return (
        <Form className='investition-table-base' onSubmit={async (event) => {
            let form = event.target as HTMLFormElement;
            await sendSupplyAsync(client, identifier, form);
            setSending(false);
        }}>
            {
                editedGoods.map(item => {
                        return(
                            <Form.Group className='investition-table-row-revision' key = {item.id}>
                                <Form.Label className='sell-page-item-name investition-table-item'>{item.name}</Form.Label>
                                <Form.Control
                                    type='text'
                                    id={item.id.concat(GROUP_POSTFIX)}
                                    className='sell-page-item-intermediate investition-table-item investition-table-form investition-table-form-revision investition-table-form-revision-category'
                                    defaultValue={item.category} />
                                <Form.Control
                                    type='number'
                                    id={item.id.concat(STORAGE_POSTFIX)}
                                    className='sell-page-item-intermediate investition-table-item investition-table-form investition-table-form-revision'
                                    defaultValue={item.storage} />
                                <Form.Control
                                    type='number'
                                    id={item.id.concat(PRICE_POSTFIX)}
                                    className='sell-page-item-intermediate investition-table-item investition-table-form investition-table-form-revision'
                                    defaultValue={item.price} />
                                <Form.Control
                                    type='number'
                                    id={item.id.concat(WRITE_OFF_POSTFIX)}
                                    className='sell-page-category-sold investition-table-item investition-table-form investition-table-sold'
                                    defaultValue={0} />
                            </Form.Group>
                        )
                    }
                )
            }
            <ButtonGroup className='investition-table-row-base'>
                <Col/>
                <Col/>
                <Button className='sell-page-category-sold investition-table-item investition-table-button investition-table-sold' type='submit' disabled={sending}>Принять</Button>
            </ButtonGroup>
        </Form>
    )
}

const InvestitionRevision: FC = () =>{
    const init: IGoodsItemDto[] = [];
    const [goods, setGoods] = useState(init);
    const client = new ApiClientWrapper(useAppDispatch());
    useEffect(
        () => {
            const fetchData = async () =>{
                let goodsDto = await client.getAllGoods();
                setGoods(goodsDto);
            }
            
            fetchData().catch(console.error);
        }, []
    )
    return(
        <Container className='investition-table-base'>
            <Row className='investition-table-row-revision'>
                <Col className='sell-page-item-name'>Товары</Col>
                <Col className='sell-page-item-intermediate'>Группа</Col>
                <Col className='sell-page-item-intermediate'>Склад</Col>
                <Col className='sell-page-item-intermediate'>Розничная цена</Col>
                <Col className='sell-page-category-sold'>Списание</Col>
            </Row>
            {RevisionGoodsList(goods)}
        </Container>
    )
}

export default InvestitionRevision;