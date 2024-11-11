import { Invoice } from "./invoice";

export interface SubscriptionPeriod {
    id: number;
    startDate: string;
    endDate: string;
    serverCost: number;
    userCount: number;
    userCost: number;
    nextUserCost: number;
    invoices: Invoice[];
}