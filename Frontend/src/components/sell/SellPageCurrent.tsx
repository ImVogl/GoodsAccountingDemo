import './SellPage.css'
import { FC, useState, useEffect, ReactElement } from 'react';
import { Form, Container, Row, Col, ButtonGroup, Button } from 'react-bootstrap';

import { ICategory, GetCategories } from '../../common/utilites/Common';
import ApiClientWrapper from '../../common/utilites/ApiClientWrapper';
import { SoldGoodsDto } from '../../common/utilites/SwaggerClient';
import { selectUserIdentifier } from '../../common/redux/UserSlice';
import { useAppSelector, useAppDispatch } from '../../common/redux/hooks';

async function sendSoldAsync(client: ApiClientWrapper, identifier: number, form: HTMLFormElement):Promise<void>{
    let dto = new SoldGoodsDto();
    dto.id = identifier;

    debugger;
    for (let i = 0; i < form.length; i++){
        if (form[i].localName !== "input"){
            continue;
        }

        let value = (form[i] as HTMLInputElement).value;
        if (value === null || value === undefined || value === ""){
            continue;
        }

        try{
            let number = parseInt(value);
            if (Number.isNaN(number)){
                continue;
            }
            if (number < 0){
                throw "sold items count can't be less than zero";
            }

            dto.sold[form[i].id] = number;
        } 
        catch (error){
            console.error(error);
            alert("Не удалось отправить сведения о проданных товарах.");
            return;
        }
    };

    try{
        await client.soldGoods(dto);
    }
    catch (error){
        console.error(error);
        alert("Не удалось отправить сведения о проданных товарах.");
    }
}

const SoldGoodsList: FC<ICategory[]> = (categories:ICategory[]): ReactElement => {
    let sold: Map<string, number> = new Map<string, number>();
    for (let i = 0; i < categories.length; i++){
        for (let j = 0; j < categories[i].goods.length; j++){
            sold.set(categories[i].goods[j].id, 0);
        }
    }

    const [sending, setSending] = useState(false);
    const dispatcher = useAppDispatch();
    let client = new ApiClientWrapper(dispatcher);
    const identifier = useAppSelector(selectUserIdentifier);
    let elements = categories.map((category) => 
    {
        return (
            <div className='sell-page-row-block' key={category.name.concat("-div")}>
                <Row className="sell-page-category" key={category.name}>{category.name}</Row>
                {
                    category.goods.map((item) => {
                        return(
                            <Row className='sell-page-item-simple' key = {item.id}>
                                <Col className='sell-page-item-name'>{item.name}</Col>
                                <Col className='sell-page-item-sold'>
                                    <Form.Group className='sell-page-item-sold'>
                                        <Form.Control id = {item.id} className='cash-form-control' type='number' />
                                    </Form.Group>
                                </Col>
                            </Row>
                        )
                    })
                }
            </div>
        )
    })
    return(
        <Form onSubmit={async (event) => {
            let form = event.target as HTMLFormElement;
            await sendSoldAsync(client, identifier, form);
            setSending(false);
        }}>
            <div className='sell-page-list-block'>
                <Container className='sell-page-container'>
                    {elements}
                </Container>
            </div>
            <Container className='sell-page-container'>
                <Row>
                    <ButtonGroup className='sell-button-group-accept'>
                        <Col/>
                        <Button className='sell-button-accept' type='submit' disabled={sending} >Принять</Button>
                    </ButtonGroup>
                </Row>
            </Container>
        </Form>
    )
}

const SellPageCurrent: FC = () => {
    const init: ICategory[] = [];
    const [goods, setGoods] = useState(init);
    const [search, setSearch] = useState("");
    const dispatcher = useAppDispatch();
    const client = new ApiClientWrapper(dispatcher);
    useEffect(
        () => {
            const fetchData = async () =>{
                let goodsDto = await client.getAllGoods();
                setGoods(GetCategories(goodsDto, search));
            }
            
            fetchData().catch(console.error);
        }, [search]
    )
    return(
        <div>
            <Form className='search-panel'>
                <Form.Group className="mb-3" controlId="search">
                    <Form.Control
                        type="text"
                        className='search-panel control'
                        placeholder="Поиск..."
                        onChange={(event) => setSearch(event.target.value)} />
                </Form.Group>
            </Form>
            <div className='list-blok'>
                {SoldGoodsList(goods)}
            </div>
        </div>
    )
}

export default SellPageCurrent;
