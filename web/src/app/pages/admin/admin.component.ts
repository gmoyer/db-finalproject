import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { Payment } from '../../models/payment';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss'
})
export class AdminComponent implements OnInit {
  payments: Payment[] = [];
  updateSuccess: boolean = false;

  constructor(private api: ApiService) {}
  ngOnInit(): void {
    this.api.getPayments().subscribe(payments => this.payments = payments.filter(payment => !payment.valid));
    setTimeout(() => console.log(this.payments), 1000);
  }

  approvePayment(payment: Payment) {
    payment.valid = true;
    this.api.updatePayment(payment).subscribe(() => {
      this.updateSuccess = true;
      this.payments = this.payments.filter(p => p.id !== payment.id);
    });
  }

  deletePayment(payment: Payment) {
  }
}
