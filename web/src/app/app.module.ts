import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

// prime-ng modules
import { PasswordModule } from 'primeng/password';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectButtonModule } from 'primeng/selectbutton';
import { CheckboxModule } from 'primeng/checkbox';
import { TableModule } from 'primeng/table';
import { CalendarModule } from 'primeng/calendar';

import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { authInterceptor } from './interceptors/auth.interceptor';
import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { HeaderComponent } from './components/header/header.component';
import { UnauthorizedComponent } from './pages/unauthorized/unauthorized.component';
import { AdminComponent } from './pages/admin/admin.component';
import { RegisterComponent } from './pages/register/register.component';
import { UserSettingsComponent } from './pages/user-settings/user-settings.component';
import { DialogComponent } from './components/dialog/dialog.component';
import { PaymentComponent } from './pages/payment/payment.component';
import { StatusComponent } from './components/status/status.component';
import { NewSubscriptionComponent } from './pages/new-subscription/new-subscription.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    HomeComponent,
    HeaderComponent,
    UnauthorizedComponent,
    AdminComponent,
    RegisterComponent,
    UserSettingsComponent,
    DialogComponent,
    PaymentComponent,
    StatusComponent,
    NewSubscriptionComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    ReactiveFormsModule,
    PasswordModule,
    InputTextModule,
    ButtonModule,
    InputNumberModule,
    SelectButtonModule,
    CheckboxModule,
    TableModule,
    CalendarModule
  ],
  providers: [
    provideHttpClient(withInterceptors([authInterceptor])),
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
