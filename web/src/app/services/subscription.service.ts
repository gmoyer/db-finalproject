import { Injectable } from '@angular/core';
import { SubscriptionPeriod } from '../models/subscription-period';
import { ApiService } from './api.service';
import { BehaviorSubject, filter, Observable } from 'rxjs';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root'
})
abstract class AbstractSubscriptionService {
  private currentPeriodSubject = new BehaviorSubject<SubscriptionPeriod | null | undefined>(undefined);
  private nextPeriodSubject = new BehaviorSubject<SubscriptionPeriod | null | undefined>(undefined);

  currentPeriod$ = this.currentPeriodSubject.asObservable().pipe(
    filter(period => period !== undefined)
  );
  nextPeriod$ = this.nextPeriodSubject.asObservable().pipe(
    filter(period => period !== undefined)
  );

  constructor(protected api: ApiService, private userService: UserService) {
    this.populatePeriods();

    this.userService.user$.subscribe(user => {
      if (user === null || user === undefined) {
        this.currentPeriodSubject.next(undefined);
        this.nextPeriodSubject.next(undefined);
      } else {
        this.populatePeriods();
      }
    });
  }

  populatePeriods() {
    this.currentPeriodSubject.next(undefined);
    this.nextPeriodSubject.next(undefined);
    this.apiCall().subscribe(data => {
      this.currentPeriodSubject.next(data.find(period => 
        new Date(period.startDate) <= new Date() && 
        new Date(period.endDate) > new Date()) ?? null);
      
      this.nextPeriodSubject.next(data.find(period => 
        new Date(period.startDate) > new Date()) ?? null);
    });
  }

  abstract apiCall(): Observable<SubscriptionPeriod[]>;
}

@Injectable({
  providedIn: 'root'
})
export class SubscriptionService extends AbstractSubscriptionService {
  apiCall() { return this.api.getSubscriptionPeriods() };
}

@Injectable({
  providedIn: 'root'
})
export class AdminSubscriptionService extends AbstractSubscriptionService {
  apiCall() { return this.api.getAllSubscriptionPeriods() };
}
