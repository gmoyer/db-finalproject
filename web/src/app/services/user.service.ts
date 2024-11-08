import { Injectable, OnInit } from '@angular/core';
import { BehaviorSubject, catchError, filter, map, Observable, of, tap } from 'rxjs';
import { User } from '../models/user';
import { HttpClient } from '@angular/common/http';
import { ApiService } from './api.service';
import { Route, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private userSubject = new BehaviorSubject<User | null>(null);
  user$ = this.userSubject.asObservable().pipe(
    filter(user => user !== undefined)
  );

  constructor(private http: HttpClient, private api: ApiService, private router: Router) {
    this.tryGetMe();
  }

  // Call the login endpoint and store user data on success
  login(username: string, password: string): Observable<User> {
    return this.api.login(username, password).pipe(map(user => {
        this.userSubject.next(user);
        return user;
    }));
  }

  // Call the logout endpoint and clear user data on success
  logout() {
    this.api.logout().subscribe(() => {
      this.userSubject.next(null);
    });
  }

  tryGetMe() {
    this.api.getStatus().subscribe(resp => {
        if (resp.authenticated) {
          this.api.getMe().subscribe(user => this.userSubject.next(user));
        }
    });
  }

  // Method to retrieve the current user synchronously
  get currentUser(): User | null {
    return this.userSubject.value;
  }
}
