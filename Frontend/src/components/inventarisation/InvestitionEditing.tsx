import './Investition.css';
import { FC, useEffect, useState } from 'react';
import { Form, Button, ButtonGroup } from 'react-bootstrap';
import ApiClientWrapper from '../../common/utilites/ApiClientWrapper';
import { useAppDispatch, useAppSelector } from '../../common/redux/hooks';
import { selectUserIdentifier } from '../../common/redux/UserSlice';
import { IGoodsItemDto } from '../../common/utilites/SwaggerClient';
import Modal from '../base/modal/Modal';
import { useFormik, FormikHelpers } from 'formik';
import Schema from './validation';

export interface INewItem{
    item: string;
    category: string;
    storage: number;
    w_price: number;
    r_price: number;
}

async function removeAsync(client: ApiClientWrapper, identifier: number, itemIdentifier: string):Promise<void>{
    client.removeGoodsItem(identifier, itemIdentifier).catch(error => {
        console.error(error);
        alert("Не удалось удалить позицию.");
    })
}

async function restoreAsync(client: ApiClientWrapper, identifier: number, itemIdentifier: string):Promise<void>{
    client.restoreGoodsItem(identifier, itemIdentifier).catch(error => {
        console.error(error);
        alert("Не удалось восстановить позицию.");
    })
}

const InvestitionEditing: FC = () =>{
    const init: IGoodsItemDto[] = [];
    const [goods, setGoods] = useState(init);
    const [sending, setSending] = useState(false);
    const [active, setActive] = useState(false);
    const identifier = useAppSelector(selectUserIdentifier);
    const client = new ApiClientWrapper(useAppDispatch());

    const AddNewItemAsync = async (values: INewItem, actions: FormikHelpers<INewItem> ) => { 
        try{
            await client.addNewGoodsItem(identifier, values.item, values.category, values.storage, values.w_price, values.r_price);
            setActive(false);
        }
        catch (exception){
          alert(exception)
        }
        finally{
          actions.resetForm();
        }
      }
    
    const initialValues: INewItem = { item: "Новый товар", category: "Категория", storage: 0, w_price: 0, r_price: 0 };
    const { values, errors, touched, isSubmitting, handleBlur, handleChange, handleSubmit, } = useFormik(
        {
            initialValues,
            validationSchema: Schema,
            onSubmit: async (values, actions) => await AddNewItemAsync(values, actions)
        });

    useEffect(
        () => {
            const fetchData = async () =>{
                let goodsDto = await client.getAllGoods();
                setGoods(goodsDto);
            }
            
            fetchData().catch(console.error);
        }, [sending]
    )
    return(
        <div className='investition-table-base'>
            <Modal active={active} setActive={setActive}>
                <Form onSubmit={values => handleSubmit(values)} autoComplete='off' >
                    <Form.Group className="form-group" controlId="item">
                        <Form.Control
                            type='text'
                            value={values.item}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            className={errors.item && touched.item ? "input-error" : "form-control-pass"}
                            placeholder='Товар...' />
                        <Form.Text>{errors.item}</Form.Text>
                    </Form.Group>
                    <Form.Group className="form-group" controlId="category">
                        <Form.Control
                            type='text'
                            value={values.category}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            className={errors.category && touched.category ? "input-error" : "form-control-pass"}
                            placeholder='Группа...' />
                        <Form.Text>{errors.category}</Form.Text>
                    </Form.Group>
                    <Form.Group className="form-group" controlId="storage">
                        <Form.Control
                            type='number'
                            value={values.storage}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            className={errors.storage && touched.storage ? "input-error" : "form-control-pass"}
                            placeholder='Единиц на складе...' />
                        <Form.Text>{errors.storage}</Form.Text>
                    </Form.Group>
                    <Form.Group className="form-group" controlId="whole_price">
                        <Form.Control
                            type='number'
                            value={values.w_price}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            className={errors.w_price && touched.w_price ? "input-error" : "form-control-pass"}
                            placeholder='Оптовая цена...' />
                        <Form.Text>{errors.w_price}</Form.Text>
                    </Form.Group>
                    <Form.Group className="form-group" controlId="retail_price">
                        <Form.Control
                            type='number'
                            value={values.r_price}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            className={errors.r_price && touched.r_price ? "input-error" : "form-control-pass"}
                            placeholder='Розничная цена...' />
                        <Form.Text>{errors.r_price}</Form.Text>
                    </Form.Group>
                    <Button className='working-area-button' variant="success" type="submit" disabled={isSubmitting}>Добавить</Button>
                </Form>
            </Modal>
            <Form className='investition-table-base'>
                {
                    goods.map(item => {
                            return(
                                <Form.Group className='investition-table-row-editing' key = {item.id}>
                                    <Form.Label className={'sell-page-item-name investition-table-item' + (item.active ? '' : ' investition-table-strikethrough')}>{item.name}</Form.Label>
                                    <Button
                                        className='sell-page-category-sold investition-table-item investition-table-button-editing investition-table-sold'
                                        type='button'
                                        onClick={async () => {
                                            setSending(true);
                                            if (item.active){ await removeAsync(client, identifier, item.id) }
                                            else { await restoreAsync(client, identifier, item.id) }
                                            setSending(false);
                                        }}
                                        disabled={sending}>{item.active ? "Удалить" : "Восстановить"}</Button>
                                </Form.Group>
                            )
                        }
                    )
                }
                <ButtonGroup className='investition-group-button'>
                    <Button className='investition-table-full-button' onClick={() => setActive(true)} type='button' disabled={sending}>Добавить</Button>
                </ButtonGroup>
            </Form>
        </div>
        
    )
}

export default InvestitionEditing;