import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { LandingComponent } from './components/landing/landing.component';
import { SurveyFormComponent } from './components/survey-form/survey-form.component';
import { ReviewComponent } from './components/review/review.component';
import { ConfirmationComponent } from './components/confirmation/confirmation.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { SurveyDetailsComponent } from './components/survey-details/survey-details.component';
import { AuthGuard } from './guards/auth.guard';
import { MfaSetupComponent } from './components/mfa-setup/mfa-setup.component';
import { MfaVerifyComponent } from './components/mfa-verify/mfa-verify.component';

const routes: Routes = [
  {path: '', component:HomeComponent },
  {path: 'dashboard', component:DashboardComponent, canActivate:[AuthGuard]},
  {path: 'landing', component:LandingComponent, canActivate:[AuthGuard]},
  {path: 'survey', component:SurveyFormComponent, canActivate:[AuthGuard]},
  {path: 'survey-details/:id', component:SurveyDetailsComponent, canActivate:[AuthGuard]},
  {path: 'review', component:ReviewComponent, canActivate:[AuthGuard]},
  {path: 'confirmation', component:ConfirmationComponent, canActivate:[AuthGuard]},
  {path: 'mfa-setup', component: MfaSetupComponent,canActivate:[AuthGuard]},
  {path: 'mfa-verify', component:MfaVerifyComponent},
  {path: '**', redirectTo:''}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
