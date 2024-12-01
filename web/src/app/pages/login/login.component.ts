import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  loginFailed: boolean = false;
  registerRedirect: boolean = false;
  deleteRedirect: boolean = false;

  constructor(
    private formBuilder: FormBuilder, 
    private router: Router, 
    private userService: UserService, 
    private route: ActivatedRoute
  ) {
    this.loginForm = this.formBuilder.group({
      email: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.registerRedirect = this.route.snapshot.queryParamMap.get('registerSuccess') === 'true';
    this.deleteRedirect = this.route.snapshot.queryParamMap.get('deleteSuccess') === 'true';
  }

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const formData = this.loginForm.value;
      
      this.userService.login(formData.email, formData.password).subscribe({
        next: (user) => {
          if (user.role == 'Admin') {
            this.router.navigate(['/admin']);
          } else {
            this.router.navigate(['']);
          }
        },
        error: (error) => {
          console.error('Login failed', error);
          this.loginFailed = true;
        }
      });
    }
  }
}
