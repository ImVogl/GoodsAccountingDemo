import { GetCategories } from './Common';
import { IGoodsItemDto } from './SwaggerClient';
import { expect, test } from '@jest/globals'

describe('Get categories tests.', () => {
    const activeCatFruitsFirst: IGoodsItemDto = {category: "Fruits", id: "1", name: "Apple", price: 10, active: true, storage: 10 };
    const activeCatVegetablesFirst: IGoodsItemDto = {category: "Vegetables", id: "2", name: "Carrot", price: 10, active: true, storage: 10 };
    const activeCatVegetablesSecond: IGoodsItemDto = {category: "Vegetables", id: "3", name: "Cabbage", price: 10, active: true, storage: 10 };
    const notActiveCatFruits: IGoodsItemDto = {category: "Fruits", id: "4", name: "Watermellow", price: 10, active: false, storage: 10 };
    const notActiveCatVegetables: IGoodsItemDto = {category: "Vegetables", id: "5", name: "Сucumber", price: 10, active: false, storage: 10 };
    const activeCatFruitsSecond: IGoodsItemDto = {category: "Fruits", id: "6", name: "Cantaloupe", price: 10, active: true, storage: 10 };
    const activeCatVegetablesThree: IGoodsItemDto = {category: "Vegetables", id: "7", name: "Potatos", price: 10, active: true, storage: 10 };
    const testData: IGoodsItemDto[] =
    [
        activeCatFruitsFirst,
        activeCatFruitsSecond,
        activeCatVegetablesFirst,
        activeCatVegetablesSecond,
        notActiveCatFruits,
        notActiveCatVegetables,
        activeCatVegetablesThree
    ];

    test("No searching result.", () => {  
        let categories = GetCategories(testData, "$t");
        expect(categories.length).toEqual(0);
    });
    
    test("Get simple searching result.", () => {  
        let categories = GetCategories(testData, "ap");
        expect(categories.length).toEqual(1);
        expect(categories[0].name).toEqual("Fruits");
        expect(categories[0].goods.length).toEqual(1);
        expect(categories[0].goods[0].id).toEqual("1");
    });
    
    test("Get result for target category.", () => {  
        let categories = GetCategories(testData, "vegetables:");
        expect(categories.length).toEqual(1);
        expect(categories[0].name).toEqual("Vegetables");
        expect(categories[0].goods.length).toEqual(3);
        expect(categories[0].goods[0].id).toEqual("2");
        expect(categories[0].goods[1].id).toEqual("3");
        expect(categories[0].goods[2].id).toEqual("7");
    });
    
    test("Get three suitable results.", () => {  
        let categories = GetCategories(testData, "ca");
        expect(categories.length).toEqual(2);
        let vagetableIndex = categories[0].name === "Vegetables" ? 0 : 1;
        let fruitIndex = categories[0].name === "Fruits" ? 0 : 1;

        expect(categories[vagetableIndex].name).toEqual("Vegetables");
        expect(categories[fruitIndex].name).toEqual("Fruits");
        expect(categories[vagetableIndex].goods.length).toEqual(2);
        expect(categories[vagetableIndex].goods[0].id).toEqual("2");
        expect(categories[vagetableIndex].goods[1].id).toEqual("3");
        expect(categories[fruitIndex].goods.length).toEqual(1);
        expect(categories[fruitIndex].goods[0].id).toEqual("6");
    });
    
    test("Get two suitable results inside category.", () => {  
        let categories = GetCategories(testData, "Vegetables:  ca");
        expect(categories.length).toEqual(1);
        expect(categories[0].name).toEqual("Vegetables");
        expect(categories[0].goods.length).toEqual(2);
        expect(categories[0].goods[0].id).toEqual("2");
        expect(categories[0].goods[1].id).toEqual("3");
    });
});