import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { PaymentMethod } from '../../models/payment-method';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrl: './payment.component.scss'
})
export class PaymentComponent implements OnInit {
  paymentForm: FormGroup;
  paymentSuccess: boolean = false;

  paymentTypes: any[] = [];

  constructor(private formBuilder: FormBuilder, private api: ApiService, private userService: UserService, public router: Router) {
    this.paymentForm = this.formBuilder.group({
      amount: [0, Validators.required],
      type: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.getPaymentMethods();
  }

  get amount() {
    return this.paymentForm.get('amount');
  }

  get type() {
    return this.paymentForm.get('type');
  }

  getPaymentMethods() {
    this.api.getPaymentMethods().subscribe((methods: PaymentMethod[]) => {
      this.paymentTypes = methods.map(method => ({ value: method.id, label: method.name }));
    });
  }

  onSubmit() {
    if (this.paymentForm.valid && this.userService.currentUser) {
      var payment = {
        amount: this.paymentForm.value.amount,
        paymentMethodId: this.paymentForm.value.type,
        valid: false,
        userId: this.userService.currentUser.id
      }

      this.api.addPayment(payment).subscribe((payment) => {
        this.paymentSuccess = true;
      });
    }
  }
}
