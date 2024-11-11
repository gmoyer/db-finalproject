import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user';
import { Payment } from '../../models/payment';
import { ApiService } from '../../services/api.service';
import { SubscriptionPeriod } from '../../models/subscription-period';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  user: User | null = null;
  payments: Payment[] = [];
  status: 'active' | 'available' | 'none' = 'none';
  minimumPayment: number = 0;
  purchasable: boolean = false;
  purchaseConfirm: boolean = false;
  subscriptionId: number | null = null;

  constructor(private userService: UserService, private api: ApiService) {}
  ngOnInit(): void {
    this.userService.user$.subscribe(user => {
      this.user = user;

      // then we can check the subscription status
      this.api.getSubscriptionPeriods().subscribe(data => {
        if (data.length === 0) {
          this.status = 'none';
          return;
        }
        let currentPeriod = data.find(period => new Date(period.startDate) <= new Date() && new Date(period.endDate) > new Date());
        if (!currentPeriod) {
          this.status = 'none';
          return;
        }
        this.subscriptionId = currentPeriod.id;
        if (currentPeriod.invoices.length > 0) {
          this.status = 'active';
        } else {
          this.status = 'available';
          this.minimumPayment = currentPeriod.nextUserCost;
          if (this.user && this.user.balance >= this.minimumPayment) {
            this.purchasable = true;
          }
        }
      });
    });
    this.api.getPayments().subscribe(payments => this.payments = payments);
  }

  addInvoice() {
    if (this.user && this.subscriptionId) {
      let invoice = {
        userId: this.user.id,
        subscriptionPeriodId: this.subscriptionId
      }
      this.api.addInvoice(invoice).subscribe(() => {
        console.log('Invoice added');
        this.purchaseConfirm = false;
        this.ngOnInit(); // refresh the page
      });
    }
  }
}
