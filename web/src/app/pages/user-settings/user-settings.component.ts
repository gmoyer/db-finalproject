import { Component } from '@angular/core';
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
export class UserSettingsComponent {
  updateForm: FormGroup;
  usernameExists: boolean = false;
  updateFailed: boolean = false;
  updateSuccess: boolean = false;
  confirmDelete: boolean = false;

  constructor(private formBuilder: FormBuilder, private router: Router, private userService: UserService, private api: ApiService) {
    this.updateForm = this.formBuilder.group({
      name: [''],
      username: [''],
      password: ['']
    });
  }

  get name() {
    return this.updateForm.get('name');
  }

  get username() {
    return this.updateForm.get('username');
  }

  get password() {
    return this.updateForm.get('password');
  }

  onSubmit() {
    this.updateFailed = false;
    this.usernameExists = false;
    this.updateSuccess = false;
    if (this.updateForm.valid && this.userService.currentUser) {
      const formData = this.updateForm.value;
      const user: User = {
        id: this.userService.currentUser.id,
        name: formData.name,
        username: formData.username,
        password: formData.password,
        role: ''
      };

      this.api.updateUser(user).subscribe({
        next: () => {
          this.updateSuccess = true;
        },
        error: (error) => {
          switch (error.error) {
            case 'Username already exists':
              this.usernameExists = true;
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
