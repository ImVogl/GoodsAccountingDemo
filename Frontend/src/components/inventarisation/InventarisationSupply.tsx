import './Inventarisation.css';
import { FC, ReactElement, useEffect, useState } from 'react';
import { Container, Col, Row, Form, Button, ButtonGroup } from 'react-bootstrap';
import { ICategory, GetCategories, badRequestProcessor } from '../../common/utilites/Common';
import ApiClientWrapper from '../../common/utilites/ApiClientWrapper';
import { useAppDispatch, useAppSelector } from '../../common/redux/hooks';
import { selectUserIdentifier, selectShiftUser } from '../../common/redux/UserSlice';
import { GoodsSuppliesDto, GoodsItemSupplyDto } from '../../common/utilites/SwaggerClient';

const PRICE_POSTFIX: string = '.price';
const SUPPLY_POSTFIX: string = '.supply';

function isSupply(fullId: string): boolean{
    return fullId.endsWith(SUPPLY_POSTFIX)
}

function isPrice(fullId: string): boolean{
    return fullId.endsWith(PRICE_POSTFIX)
}

function extractItemId(fullId: string): string{
    if (isSupply(fullId)){
        return fullId.replace(SUPPLY_POSTFIX, '');
    }

    if (isPrice(fullId)){
        return fullId.replace(PRICE_POSTFIX, '');
    }

    return ""
}

export async function sendSupplyAsync(client: ApiClientWrapper, identifier: number, form: HTMLFormElement):Promise<void>{
    let dto = new GoodsSuppliesDto();
    dto.id = identifier;
    let grouppedValues: { [id: string] : {[type: string]: number}; } = {};
    for (let i = 0; i < form.length; i++){
        if (form[i].localName !== "input"){
            continue;
        }

        if (!isPrice(form[i].id) && !isSupply(form[i].id)){
            continue;
        }

        let value = (form[i] as HTMLInputElement).value;
        let defaultValue = (form[i] as HTMLInputElement).defaultValue;
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

            let identifier = form[i].id;
            if (!grouppedValues.hasOwnProperty(extractItemId(identifier))){
                grouppedValues[extractItemId(identifier)] = {}
            }

            grouppedValues[extractItemId(identifier)][isSupply(identifier) ? SUPPLY_POSTFIX : PRICE_POSTFIX] = number;
        } 
        catch (error){
            console.error(error);
            alert("Не удалось отправить сведения о проданных товарах.");
            return;
        }
    };

    for (let identifier in grouppedValues){
        let item = new GoodsItemSupplyDto();
        item.id = identifier;
        item.price = grouppedValues[identifier][PRICE_POSTFIX];
        item.receipt = grouppedValues[identifier][SUPPLY_POSTFIX];
        dto.items.push(item);
    }

    try{
        await client.updateSupplySate(dto);
    }
    catch (exception){
        if (!badRequestProcessor(exception)){
            console.error(exception);
            alert("Не удалось отправить сведения о проданных товарах.");
        }
    }
}

const SupplyGoodsList: FC<ICategory[]> = (categories:ICategory[]): ReactElement => {
    const [sending, setSending] = useState(false);
    const client = new ApiClientWrapper(useAppDispatch());
    const identifier = useAppSelector(selectUserIdentifier);
    const shift = useAppSelector(selectShiftUser);
    return (
        <Form className='investition-table-base' onSubmit={async (event) => {
            let form = event.target as HTMLFormElement;
            await sendSupplyAsync(client, identifier, form);
            setSending(false);
        }}>
            {
                categories.map(category => {
                        return(
                                <div className='investition-table-base' key={category.name.concat("-div")}>
                                    <Form.Group className='investition-table-row-base' key={category.name}>
                                        <Form.Label className='sell-page-item-name investition-table-category'>{category.name}</Form.Label>
                                        <Col/>
                                        <Col/>
                                    </Form.Group>
                                    {
                                        category.goods.map(item => {
                                                    return(
                                                        <Form.Group className='investition-table-row-base' key = {item.id}>
                                                            <Form.Label className='sell-page-item-name investition-table-item'>{item.name}</Form.Label>
                                                            <Form.Control
                                                                type='number'
                                                                id={item.id.concat(PRICE_POSTFIX)}
                                                                className='sell-page-item-intermediate investition-table-item investition-table-form'
                                                                defaultValue={item.whole} />
                                                            <Form.Control
                                                                type='number'
                                                                id={item.id.concat(SUPPLY_POSTFIX)}
                                                                className='sell-page-category-sold investition-table-item investition-table-form investition-table-sold'
                                                                defaultValue={0} />
                                                        </Form.Group>
                                                    )
                                                }
                                            )
                                    }
                                </div>
                            )
                        }
                    )
            }
            <ButtonGroup className='investition-table-row-base'>
                <Col/>
                <Col/>
                <Button className='sell-page-category-sold investition-table-item investition-table-button investition-table-sold' type='submit' disabled={sending || !shift}>Принять</Button>
            </ButtonGroup>
        </Form>
    )
}

const InventarisationSupply: FC = () =>{
    const init: ICategory[] = [];
    const [goods, setGoods] = useState(init);
    const client = new ApiClientWrapper(useAppDispatch());
    useEffect(
        () => {
            const fetchData = async () =>{
                let goodsDto = await client.getAllGoods();
                setGoods(GetCategories(goodsDto, ""));
            }
            
            fetchData().catch(exception => {
                if (!badRequestProcessor(exception)){
                    console.error(exception);
                }
            });
        }, []
    )
    return(
        <Container className='investition-table-base'>
            <Row className='investition-table-row-base'>
                <Col className='sell-page-item-name'>Товары</Col>
                <Col className='sell-page-item-intermediate'>Оптовая цена</Col>
                <Col className='sell-page-category-sold'>Единиц поставлено</Col>
            </Row>
            {SupplyGoodsList(goods)}
        </Container>
    )
}

export default InventarisationSupply;