import { BadRequest } from './exceptions';
import { IGoodsItemDto } from './SwaggerClient'

// Getting base url fron config.
export function getBaseUrl(): string{
    const config = require('../../config.json');
    return config.use_tls ? config.server_url_ssl : config.server_url;
}

// Run timer.
export const sleep_ms = (ms:number) => new Promise(res => setTimeout(res, ms));

export interface IGoodsItem{
    id: string;
    name: string;
    price: number;
}

export interface ICategory{
    name: string;
    goods: IGoodsItem[];
}

// Search function
export function GetCategories(goods: IGoodsItemDto[], search: string):ICategory[]
{
    let parts = search.toUpperCase().split(':', 2);
    let category = (parts.length === 2 ? parts[0] : "").trimStart();
    let searchPattern = (parts.length === 2 ? parts[1] : search.toUpperCase()).trimStart();
    let result: ICategory[] = []
    goods.forEach(item => {
        if (item.active){
            if (((category !== "" && category === item.category.toUpperCase()) || category === "") && item.name.toUpperCase().startsWith(searchPattern)){
                let index = result.findIndex(c => c.name === item.category)
                if (index >= 0){
                    result[index].name = item.category;
                    result[index].goods.push({ id: item.id, name: item.name, price: item.price })
                }
                else{
                    result.push({ name: item.category, goods: [{ id: item.id, name: item.name, price: item.price }] })
                }
            }
        }
    });

    return result;
}

export function badRequestProcessor(error: any): boolean{
    if (error instanceof BadRequest){
        let converted = error as BadRequest;
        alert(converted.message);
        console.error(converted.message);
        return true;
    }

    return false;
}
