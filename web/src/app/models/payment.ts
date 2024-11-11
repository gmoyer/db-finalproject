import { PaymentMethod } from "./payment-method";
import { User } from "./user";

export interface Payment {
    id: number;
    amount: number;
    date: string;
    paymentMethodId: number;
    paymentMethod: PaymentMethod;
    valid: boolean;
    userId: number;
    user?: User;
}