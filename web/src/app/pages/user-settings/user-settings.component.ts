import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { ApiService } from '../../services/api.service';
import { User } from '../../models/user';

@Component({
  selector: 'app-user-settings',
  templateUrl: './user-settings.component.html',
  styleUrl: './user-settings.component.scss'
})
export class UserSettingsComponent implements OnInit {
  updateForm: FormGroup;
  emailExists: boolean = false;
  updateFailed: boolean = false;
  updateSuccess: boolean = false;
  confirmDelete: boolean = false;

  constructor(private formBuilder: FormBuilder, private router: Router, private userService: UserService, private api: ApiService) {
    this.updateForm = this.formBuilder.group({
      name: [''],
      email: [''],
      playertag: [''],
      password: [''],
      invoice: [false]
    });
  }

  ngOnInit(): void {
    this.userService.user$.subscribe(user => {
      if (user) {
        this.updateForm.patchValue({
          name: user.name,
          email: user.email,
          playertag: user.playertag,
          invoice: user.autoInvoice
        });
      }
    });
  }

  get name() {
    return this.updateForm.get('name');
  }

  get email() {
    return this.updateForm.get('email');
  }

  get playertag() {
    return this.updateForm.get('playertag');
  }

  get password() {
    return this.updateForm.get('password');
  }

  get invoice() {
    return this.updateForm.get('invoice');
  }

  onSubmit() {
    this.updateFailed = false;
    this.emailExists = false;
    this.updateSuccess = false;
    if (this.updateForm.valid && this.userService.currentUser) {
      const formData = this.updateForm.value;
      const user: User = {
        id: this.userService.currentUser.id,
        name: formData.name,
        email: formData.email,
        playertag: formData.playertag,
        password: formData.password,
        role: '',
        autoInvoice: formData.invoice,
        balance: 0
      };

      this.api.updateUser(user).subscribe({
        next: () => {
          this.updateSuccess = true;
          this.userService.updateUser();
        },
        error: (error) => {
          switch (error.error) {
            case 'Email already exists':
              this.emailExists = true;
              break;
            default:
              this.updateFailed = true;
              break;
          }
        }
      });
    } else {
      this.updateFailed = true;
    }
  }

  deleteUser() {
    if (this.userService.currentUser) {
      this.api.deleteUser(this.userService.currentUser.id).subscribe({
        next: () => {
          this.userService.logout();
          this.router.navigate(['/login'], { queryParams: { deleteSuccess: true } });
        },
        error: () => {
          this.updateFailed = true;
        }
      });
    }
  }
}
