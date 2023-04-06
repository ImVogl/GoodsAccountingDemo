import './Inventarisation.css';
import { FC, useEffect, useState } from 'react';
import { Form, Button, ButtonGroup } from 'react-bootstrap';
import ApiClientWrapper from '../../common/utilites/ApiClientWrapper';
import { useAppDispatch, useAppSelector } from '../../common/redux/hooks';
import { selectUserIdentifier, selectShiftUser } from '../../common/redux/UserSlice';
import { IGoodsItemDto } from '../../common/utilites/SwaggerClient';
import Modal from '../base/modal/Modal';
import { useFormik, FormikHelpers } from 'formik';
import Schema from './validation';
import { badRequestProcessor } from '../../common/utilites/Common';

export interface INewItem{
    item: string;
    category: string;
    storage: number;
    whole: number;
    retail: number;
}

async function removeAsync(client: ApiClientWrapper, identifier: number, itemIdentifier: string):Promise<void>{
    client.removeGoodsItem(identifier, itemIdentifier).catch(exception => {
        if (!badRequestProcessor(exception)){
            console.error(exception);
            alert("Не удалось удалить позицию.");
        }
    })
}

async function restoreAsync(client: ApiClientWrapper, identifier: number, itemIdentifier: string):Promise<void>{
    client.restoreGoodsItem(identifier, itemIdentifier).catch(exception => {
        if (!badRequestProcessor(exception)){
            console.error(exception);
            alert("Не удалось восстановить позицию.");
        }
    })
}

const InventarisationEditing: FC = () =>{
    const init: IGoodsItemDto[] = [];
    const initCategories: string [] = [];
    const [goods, setGoods] = useState(init);
    const [categories, setCategories] = useState(initCategories);
    const [sending, setSending] = useState(false);
    const [active, setActive] = useState(false);
    const identifier = useAppSelector(selectUserIdentifier);
    const shift = useAppSelector(selectShiftUser);
    const client = new ApiClientWrapper(useAppDispatch());

    const AddNewItemAsync = async (values: INewItem, actions: FormikHelpers<INewItem> ) => { 
        try{
            await client.addNewGoodsItem(identifier, values.item, values.category, values.storage, values.whole, values.retail);
            setActive(false);
        }
        catch (exception){
            if (!badRequestProcessor(exception)){
                alert(exception);
            }
        }
        finally{
          actions.resetForm();
        }
      }
    
    const initialValues: INewItem = { item: "Новый товар", category: "Категория", storage: 0, whole: 0, retail: 0 };
    const { values, errors, touched, isSubmitting, handleBlur, handleChange, handleSubmit, } = useFormik(
        {
            initialValues: initialValues,
            validationSchema: Schema,
            onSubmit: async (values, actions) => await AddNewItemAsync(values, actions)
        });
        
    useEffect(
        () => {
            const fetchData = async () =>{
                const goodsDto = await client.getAllGoods();
                setGoods(goodsDto);
                const categories: string [] = [];
                for (let item of goodsDto){
                    if (categories.indexOf(item.category) === -1){
                        categories.push(item.category);
                    }
                }

                setCategories(categories);
            }
            
            fetchData().catch(exception => {
                if (!badRequestProcessor(exception)){
                    console.error(exception);
                }
            });
        }, [sending, active]
    );

    return(
        <div className='investition-table-base'>
            <Modal active={active} setActive={setActive}>
                <Form onSubmit={values => handleSubmit(values)} autoComplete='off' >
                    <Form.Group className="form-group inventarisation-popup-form-group" controlId="item">
                        <Form.Control
                            type='text'
                            value={values.item}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            className={errors.item && touched.item ? "input-error" : "form-control-pass"}
                            placeholder='Товар...' />
                        <Form.Text>{errors.item}</Form.Text>
                    </Form.Group>
                    <Form.Group className="form-group inventarisation-popup-form-group" controlId="category">
                        <Form.Control
                            list='categories'
                            type='text'
                            value={values.category}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            className={errors.category && touched.category ? "input-error" : "form-control-pass"}
                            placeholder='Группа...' />
                            <datalist id="categories" >
                                {categories.map(value => <option key={value + '_id'} value={value} />)}
                            </datalist>
                        <Form.Text>{errors.category}</Form.Text>
                    </Form.Group>
                    <Form.Group className="form-group inventarisation-popup-form-group" controlId="storage">
                        <Form.Control
                            type='number'
                            value={values.storage}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            className={errors.storage && touched.storage ? "input-error investition-table-form-number" : "form-control-pass investition-table-form-number"}
                            placeholder='Единиц на складе...' />
                        <Form.Text>{errors.storage}</Form.Text>
                    </Form.Group>
                    <Form.Group className="form-group inventarisation-popup-form-group" controlId="whole">
                        <Form.Control
                            type='number'
                            value={values.whole}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            className={errors.whole && touched.whole ? "input-error investition-table-form-number" : "form-control-pass investition-table-form-number"}
                            placeholder='Оптовая цена...' />
                        <Form.Text>{errors.whole}</Form.Text>
                    </Form.Group>
                    <Form.Group className="form-group inventarisation-popup-form-group" controlId="retail">
                        <Form.Control
                            type='number'
                            value={values.retail}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            className={errors.retail && touched.retail ? "input-error investition-table-form-number" : "form-control-pass investition-table-form-number"}
                            placeholder='Розничная цена...' />
                        <Form.Text>{errors.retail}</Form.Text>
                    </Form.Group>
                    <Form.Group className="form-group inventarisation-popup-button-group">
                        <Button className='working-area-button' variant="success" type="submit" disabled={isSubmitting || !shift}>Добавить</Button>
                    </Form.Group>
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
                                        onClick={() => {
                                            setSending(true);
                                            if (item.active){ 
                                                removeAsync(client, identifier, item.id)
                                                    .then(() => setSending(false))
                                                    .catch(exception => { if (!badRequestProcessor(exception)){ console.error(exception); } });
                                            } else { 
                                                restoreAsync(client, identifier, item.id)
                                                    .then(() => setSending(false))
                                                    .catch(exception => { if (!badRequestProcessor(exception)){ console.error(exception); } });
                                            }
                                        }}
                                        disabled={sending || !shift}>{item.active ? "Удалить" : "Восстановить"}</Button>
                                </Form.Group>
                            )
                        }
                    )
                }
                <ButtonGroup className='investition-group-button'>
                    <Button className='investition-table-full-button' onClick={() => setActive(true)} type='button' disabled={sending || !shift}>Добавить</Button>
                </ButtonGroup>
            </Form>
        </div>
        
    )
}

export default InventarisationEditing;