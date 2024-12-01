import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { Payment } from '../../models/payment';
import { SubscriptionPeriod } from '../../models/subscription-period';
import { AdminSubscriptionService } from '../../services/subscription.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss'
})
export class AdminComponent implements OnInit {
  payments: Payment[] = [];
  updateSuccess: boolean = false;

  previousPeriod: SubscriptionPeriod | null = null;
  currentPeriod: SubscriptionPeriod | null = null;
  nextPeriod: SubscriptionPeriod | null = null;

  rolloverCommands: string[] = [];

  constructor(private api: ApiService, private subscriptionService: AdminSubscriptionService) {}
  async ngOnInit() {
    this.api.getPayments().subscribe(payments => this.payments = payments.filter(payment => !payment.valid));

    this.previousPeriod = await firstValueFrom(this.subscriptionService.previousPeriod$);
    this.currentPeriod = await firstValueFrom(this.subscriptionService.currentPeriod$);
    this.nextPeriod = await firstValueFrom(this.subscriptionService.nextPeriod$);

    this.calculateRolloverCommands();
  }

  approvePayment(payment: Payment) {
    payment.valid = true;
    this.api.updatePayment(payment).subscribe(() => {
      this.updateSuccess = true;
      this.payments = this.payments.filter(p => p.id !== payment.id);
    });
  }

  deletePayment(payment: Payment) {
    this.api.deletePayment(payment.id).subscribe(() => {
      this.updateSuccess = true;
      this.payments = this.payments.filter(p => p.id !== payment.id);
    });
  }

  calculateRolloverCommands() {
    if (!this.currentPeriod) {
      return;
    }
    if (!this.previousPeriod) {
      this.rolloverCommands = this.currentPeriod.invoices.map(invoice => `whitelist add ${invoice.user?.playertag}`);
      return;
    }

    const previousUsers = this.previousPeriod.invoices.map(invoice => invoice.user?.playertag);
    const currentUsers = this.currentPeriod.invoices.map(invoice => invoice.user?.playertag);

    const usersToRemove = previousUsers.filter(tag => !currentUsers.includes(tag));
    const usersToAdd = currentUsers.filter(tag => !previousUsers.includes(tag));

    this.rolloverCommands = [
      ...usersToRemove.map(tag => `whitelist remove ${tag}`),
      ...usersToAdd.map(tag => `whitelist add ${tag}`)
    ];
  }
}
