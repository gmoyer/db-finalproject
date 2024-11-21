import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user';
import { Payment } from '../../models/payment';
import { ApiService } from '../../services/api.service';
import { SubscriptionPeriod } from '../../models/subscription-period';
import { SubscriptionService } from '../../services/subscription.service';
import { firstValueFrom } from 'rxjs';

// Extend subscription period to have a status field
interface SubscriptionPeriodWithStatus extends SubscriptionPeriod {
  name: string;
  status: 'active' | 'available';
  purchasable: boolean;
  purchaseConfirm: boolean;
}


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  user: User | null = null;
  payments: Payment[] = [];
  periods: SubscriptionPeriodWithStatus[] = [];

  constructor(
    private userService: UserService, 
    private api: ApiService,
    private subscriptionService: SubscriptionService) {}
  async ngOnInit() {
    this.periods = [];

    // Don't need this right now so just subscribe to it
    this.api.getPayments().subscribe(payments => this.payments = payments);

    // Get the user and subscription period data
    this.user = await firstValueFrom(this.userService.user$);
    const currentPeriod = await firstValueFrom(this.subscriptionService.currentPeriod$);
    const nextPeriod = await firstValueFrom(this.subscriptionService.nextPeriod$);

    // Determine the status of the current period
    if (currentPeriod) {
      this.periods.push({ 
        ...currentPeriod,
        name: 'Current Period',
        status: currentPeriod.invoices.length > 0 ? 'active' : 'available',
        purchasable: this.user != null && this.user.balance >= currentPeriod.nextUserCost,
        purchaseConfirm: false
      });
    }

    // Determine the status of the next period
    if (nextPeriod) {
      this.periods.push({ 
        ...nextPeriod,
        name: 'Next Period',
        status: nextPeriod.invoices.length > 0 ? 'active' : 'available',
        purchasable: this.user != null && this.user.balance >= nextPeriod.nextUserCost,
        purchaseConfirm: false
      });
    }
  }

  addInvoice(period: SubscriptionPeriodWithStatus) {
    if (!this.user) {
      return;
    }

    let invoice = {
      userId: this.user.id,
      subscriptionPeriodId: period.id
    }
    this.api.addInvoice(invoice).subscribe(() => {
      console.log('Invoice added');
      this.subscriptionService.populatePeriods(); // refresh subscription period information
      this.ngOnInit(); // refresh the page
    });
  }
}
