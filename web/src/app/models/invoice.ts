import { SubscriptionPeriod } from "./subscription-period";
import { User } from "./user";

export interface Invoice {
    id: number;
    userId: number;
    subscriptionPeriodId: number;
    amount: number;
    user?: User;
    subscriptionPeriod?: SubscriptionPeriod;
}