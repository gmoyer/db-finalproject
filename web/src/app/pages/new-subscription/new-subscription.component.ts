import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import moment from 'moment';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-new-subscription',
  templateUrl: './new-subscription.component.html',
  styleUrl: './new-subscription.component.scss'
})
export class NewSubscriptionComponent {
  form: FormGroup;
  success: boolean = false;

  constructor(private formBuilder: FormBuilder, private api: ApiService, public router: Router, private userService: UserService) {
    this.form = this.formBuilder.group({
      startDate: [null, Validators.required],
      endDate: [null, Validators.required],
      amount: [null, Validators.required],
    }, { validator: this.dateLessThan('startDate', 'endDate') });
  }
  dateLessThan(startControl: string, endControl: string): any {
    return (formGroup: FormGroup) => {
      const start = formGroup.controls[startControl];
      const end = formGroup.controls[endControl];

      if (start.value && end.value && moment(start.value).isAfter(moment(end.value))) {
        end.setErrors({ dateLessThan: true });
      } else {
        end.setErrors(null);
      }
    };
  }

  get startDate() {
    return this.form.get('startDate');
  }
  get endDate() {
    return this.form.get('endDate');
  }
  get amount() {
    return this.form.get('amount');
  }

  onSubmit() {
    this.success = false;
    if (this.form.valid) {
      let subscription = {
        startDate: moment(this.form.value.startDate).format('YYYY-MM-DD'),
        endDate: moment(this.form.value.endDate).format('YYYY-MM-DD'),
        serverCost: this.form.value.amount,
      }
      
      this.api.addSubscription(subscription).subscribe(sub => {
        this.success = true;
        this.userService.updateUser();
      });
    }
  }
}
