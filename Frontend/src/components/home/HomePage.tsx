import './HomePage.css'
import React, { FC, ReactElement } from 'react';
import { Row, Container, Col, Form } from 'react-bootstrap';

import { ICategory, GetCategories, badRequestProcessor } from '../../common/utilites/Common'
import ApiClientWrapper from '../../common/utilites/ApiClientWrapper'

import LayoutBase from '../layouts/BaseLayout'

const GoodsList: FC<ICategory[]> = (goods:ICategory[]): ReactElement => {
    let elements = goods.map((category) => 
    {
        return (
            <div className='row-block' key={category.name.concat("-div")}>
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

    return(<Container className='container'>{elements}</Container>)
}

const HomePage: FC = () => {
    const init: ICategory[] = [];
    const [goods, setGoods] = React.useState(init);
    const [search, setSearch] = React.useState("");
    const client = new ApiClientWrapper();
    React.useEffect(
        () => {
            const fetchData = async () =>{
                let goods = await client.getAllGoods();
                setGoods(GetCategories(goods, search));
            }
            
            fetchData().catch(exception => {
                if (!badRequestProcessor(exception)){
                    console.error(exception);
                }
            });
        }, [search]
    )
    
    return(
        <LayoutBase>
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
            </div>
            <div className='list-blok'>
                {GoodsList(goods)}
            </div>
        </LayoutBase>
    );
}

export default HomePage;
