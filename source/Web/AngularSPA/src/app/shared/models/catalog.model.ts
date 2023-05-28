import { ICatalogItem } from './catalogItem.model';

export interface ICatalog {
    pageIndex: number;
    data: ICatalogItem[];
    displayData: ICatalogItem[];
    pageSize: number;
    count: number;
}
