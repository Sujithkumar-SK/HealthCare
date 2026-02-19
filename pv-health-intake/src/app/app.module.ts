import { NgModule, APP_INITIALIZER } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SurveyModule } from 'survey-angular-ui';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LandingComponent } from './components/landing/landing.component';
import { SurveyFormComponent } from './components/survey-form/survey-form.component';
import { ReviewComponent } from './components/review/review.component';
import { ConfirmationComponent } from './components/confirmation/confirmation.component';
import { AuthService } from './services/auth.service';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { HomeComponent } from './components/home/home.component';
import { SurveyDetailsComponent } from './components/survey-details/survey-details.component';
import { MfaSetupComponent } from './components/mfa-setup/mfa-setup.component';
import { MfaVerifyComponent } from './components/mfa-verify/mfa-verify.component';

export function initializeGoogle(authService: AuthService) {
  return () => authService.initialize();
}

@NgModule({
  declarations: [
    AppComponent,
    LandingComponent,
    SurveyFormComponent,
    ReviewComponent,
    ConfirmationComponent,
    DashboardComponent,
    HomeComponent,
    SurveyDetailsComponent,
    MfaSetupComponent,
    MfaVerifyComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    SurveyModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: initializeGoogle,
      deps: [AuthService],
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
