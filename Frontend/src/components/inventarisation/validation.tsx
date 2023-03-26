import { ObjectSchema, object, string, number } from 'yup'
import { INewItem } from './InvestitionEditing'

const Schema: ObjectSchema<INewItem> = object({
    item: string()
        .defined('Имя товара не может быть пустым!'),
    category: string()
        .defined('Категория товара не может быть пустой!'),
    storage: number()
        .integer('Число товаров на складе не может быть дробным!')
        .min(0, 'Число товаров на склане не может быть отрицательным!')
        .defined('Число товаров на складе не может быть пустым!'),
    w_price: number()
        .min(0, 'Цена не может быть отрицательной!')
        .defined('Цена не может быть пустой!'),
    r_price: number()
        .min(0, 'Цена не может быть отрицательной!')
        .defined('Цена не может быть пустой!')
});

export default Schema