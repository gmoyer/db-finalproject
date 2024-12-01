import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  registerForm: FormGroup;
  emailExists: boolean = false;
  registerFailed: boolean = false;

  constructor(private formBuilder: FormBuilder, private router: Router, private userService: UserService, private api: ApiService) {
    this.registerForm = this.formBuilder.group({
      name: ['', Validators.required],
      email: ['', Validators.required],
      playertag: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    },
    { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(formGroup: FormGroup) {
    const password = formGroup.get('password');
    const confirmPassword = formGroup.get('confirmPassword');
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
    } else {
      confirmPassword?.setErrors(null);
    }
  }

  get name() {
    return this.registerForm.get('name');
  }

  get email() {
    return this.registerForm.get('email');
  }

  get playertag() {
    return this.registerForm.get('playertag');
  }

  get password() {
    return this.registerForm.get('password');
  }

  get confirmPassword() {
    return this.registerForm.get('confirmPassword');
  }

  onSubmit() {
    this.registerFailed = false;
    this.emailExists = false;
    if (this.registerForm.valid) {
      const formData = this.registerForm.value;
      const user: User = {
        id: 0,
        name: formData.name,
        email: formData.email,
        playertag: formData.playertag,
        password: formData.password,
        role: 'User',
        autoInvoice: false,
        balance: 0
      };

      this.api.addUser(user).subscribe({
        next: () => {
          this.router.navigate(['/login'], { queryParams: { registerSuccess: true } });
        },
        error: (error) => {
          switch (error.error) {
            case 'Email already exists':
              this.emailExists = true;
              break;
            default:
              this.registerFailed = true;
              break;
          }
        }
      });
    }
  }
}
