import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { from, Observable, of, scheduled } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiService } from '../services/api.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private http: HttpClient, private router: Router, private api: ApiService) {}

  canActivate(): Observable<boolean> {
    return this.api.getStatus().pipe(map((resp) => {
      if (resp.authenticated) {
        return true;
      } else {
        this.router.navigate(['/login']);
        return false;
      }
    }));
  }
}