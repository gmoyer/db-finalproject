import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { User } from '../models/user';
import { PaymentMethod } from '../models/payment-method';
import { Payment } from '../models/payment';
import { SubscriptionPeriod } from '../models/subscription-period';

const API_URL = 'https://localhost:7141/api';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  constructor(private http: HttpClient) {}

  // Custom error handler
  handleError(msg: string) {
    return (error: HttpErrorResponse): Observable<never> => {
      console.error(msg, error);
      return throwError(() => error);
    };
  }

  // ===== Auth Endpoints =====
  getStatus(): Observable<any> {
    return this.http.get<any>(`${API_URL}/auth/status`).pipe(
      catchError(this.handleError('Failed to get auth status'))
    );
  }

  login(username: string, password: string): Observable<User> {
    return this.http.post<User>(`${API_URL}/auth/login`, { username, password }).pipe(
      catchError(this.handleError('Failed to login'))
    );
  }

  logout(): Observable<string> {
    return this.http.post<string>(`${API_URL}/auth/logout`, {}, { responseType: 'text' as 'json' }).pipe(
      catchError(this.handleError('Failed to logout'))
    );
  }

  // ===== User Endpoints =====
  getMe(): Observable<User> {
    return this.http.get<User>(`${API_URL}/users/me`).pipe(
      catchError(this.handleError('Failed to get user'))
    );
  }

  addUser(user: User): Observable<User> {
    return this.http.post<User>(`${API_URL}/users`, user).pipe(
      catchError(this.handleError('Failed to add user'))
    );
  }

  updateUser(user: User): Observable<User> {
    return this.http.put<User>(`${API_URL}/users/${user.id}`, user).pipe(
      catchError(this.handleError('Failed to update user'))
    );
  }

  deleteUser(id: number): Observable<string> {
    return this.http.delete<string>(`${API_URL}/users/${id}`, { responseType: 'text' as 'json' }).pipe(
      catchError(this.handleError('Failed to delete user'))
    );
  }

  // ===== Payment Endpoints =====
  getPaymentMethods(): Observable<PaymentMethod[]> {
    return this.http.get<PaymentMethod[]>(`${API_URL}/PaymentMethods`).pipe(
      catchError(this.handleError('Failed to get payment methods'))
    );
  }

  addPayment(payment: any): Observable<Payment> {
    return this.http.post<Payment>(`${API_URL}/payments`, payment).pipe(
      catchError(this.handleError('Failed to add payment'))
    );
  }

  getPayments(): Observable<Payment[]> {
    return this.http.get<Payment[]>(`${API_URL}/payments`).pipe(
      catchError(this.handleError('Failed to get payments'))
    );
  }

  updatePayment(payment: Payment): Observable<Payment> {
    return this.http.put<Payment>(`${API_URL}/payments/${payment.id}`, payment).pipe(
      catchError(this.handleError('Failed to update payment'))
    );
  }

  // ===== Subscription Endpoints =====
  getSubscriptionPeriods(): Observable<SubscriptionPeriod[]> {
    return this.http.get<SubscriptionPeriod[]>(`${API_URL}/SubscriptionPeriods`).pipe(
      catchError(this.handleError('Failed to get subscription periods'))
    );
  }
  addSubscription(subscription: any): Observable<any> {
    return this.http.post<any>(`${API_URL}/SubscriptionPeriods`, subscription).pipe(
      catchError(this.handleError('Failed to add subscription'))
    );
  }


  // ===== Invoice Endpoints =====
  addInvoice(invoice: any): Observable<any> {
    return this.http.post<any>(`${API_URL}/invoices`, invoice).pipe(
      catchError(this.handleError('Failed to add invoice'))
    );
  }
}
