import './HomePage.css'
import React, { FC } from 'react';
import { Row, Container, Col } from 'react-bootstrap';

import { getBaseUrl } from '../../common/utilites/Common';
import { Client, IGoodsItemDto } from '../../common/utilites/SwaggerClient'

import LayoutBase from '../layouts/base/BaseLayout'

interface IGoodsItem{
    id: string;
    name: string;
    price: number;
}

interface ICategory{
    name: string;
    goods: IGoodsItem[];
}

function GetCategories(goods: IGoodsItemDto[]):ICategory[]
{
    let result: ICategory[] = []
    goods.forEach(item => {
        if (item.active){
            let index = result.findIndex(c => c.name == item.category)
            if (index >= 0){
                result[index].name = item.category;
                result[index].goods.push({ id: item.id, name: item.name, price: item.price })
            }
            else{
                result.push({ name: item.category, goods: [{ id: item.id, name: item.name, price: item.price }] })
            }
        }
    });

    return result;
}

const HomePage: FC = () => {
    const init: ICategory[] = [];
    const [goods, setGoods] = React.useState(init);
    const client = new Client(getBaseUrl());
    React.useEffect(
        () => {
            const fetchData = async () =>{
                let goods = await client.goods();
                setGoods(GetCategories(goods));
            }
            
            fetchData().catch(console.error);
        }, []
    )
    
    return(
        <LayoutBase>
            <Container>
            {
                goods.map((category) => 
                { 
                    return (
                        <div>
                            <Row className="category" key={category.name}>{category.name}</Row>
                            {
                                category.goods.map((item) => { 
                                    return(
                                        <Row className='item' key = {item.id}>
                                            <Col className='item-name'>{item.name}</Col>
                                            <Col className='item-price'>{item.price} Руб</Col>
                                        </Row>
                                    )
                                })
                            }
                        </div>
                    )
                })
            }
            </Container>
        </LayoutBase>
    );
}

export default HomePage;
