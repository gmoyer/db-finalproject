export interface User {
    id: number;
    name: string;
    email: string;
    password: string;
    playertag: string;
    role: string;
    autoInvoice: boolean;
    balance: number;
}