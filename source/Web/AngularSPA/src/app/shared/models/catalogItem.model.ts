export interface ICatalogItem {
    id: number;
    name: string;
    description: string;
    oldPrice: number;
    price: number;
    pictureUri: string;
    catalogBrandId: number;
    catalogBrand: string;
    catalogTypeId: number;
    catalogType: string;
    units: number;
}
