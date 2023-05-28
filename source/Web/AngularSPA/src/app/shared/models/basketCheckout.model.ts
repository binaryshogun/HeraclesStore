export interface IBasketCheckout {
    city: number;
    street: string;
    state: string;
    country: number;
    zipCode: string;
    cardNumber: string;
    cardExpiration: Date;
    cardSecurityNumber: string;
    cardHolder: string;
    cardTypeId: number;
}