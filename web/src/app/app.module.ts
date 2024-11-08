import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { HttpClient, HttpClientModule, provideHttpClient, withInterceptors } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

// prime-ng modules
import { PasswordModule } from 'primeng/password';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { authInterceptor } from './interceptors/auth.interceptor';
import { HeaderComponent } from './components/header/header.component';
import { UnauthorizedComponent } from './pages/unauthorized/unauthorized.component';
import { AdminComponent } from './pages/admin/admin.component';
import { RegisterComponent } from './pages/register/register.component';
import { UserSettingsComponent } from './pages/user-settings/user-settings.component';
import { ConfirmComponent } from './components/confirm/confirm.component';

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
    ConfirmComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    ReactiveFormsModule,
    PasswordModule,
    InputTextModule,
    ButtonModule
  ],
  providers: [
    provideHttpClient(withInterceptors([authInterceptor])),
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
