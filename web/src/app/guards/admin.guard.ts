import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { from, Observable, of, scheduled } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiService } from '../services/api.service';
import { UserService } from '../services/user.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(private router: Router, private userService: UserService) {}

  canActivate(): Observable<boolean> {
    return this.userService.user$.pipe(map(user => {
      if (user && user.role === 'Admin') {
        return true;
      } else {
        this.router.navigate(['/unauthorized']);
        return false;
      }
    }));
  }
}