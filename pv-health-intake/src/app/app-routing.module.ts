import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LandingComponent } from './components/landing/landing.component';
import { SurveyFormComponent } from './components/survey-form/survey-form.component';
import { ReviewComponent } from './components/review/review.component';
import { ConfirmationComponent } from './components/confirmation/confirmation.component';

const routes: Routes = [
  {path: '', component:LandingComponent },
  {path: 'survey', component:SurveyFormComponent},
  {path: 'review', component:ReviewComponent},
  {path: 'confirmation', component:ConfirmationComponent},
  {path: '**', redirectTo:''}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
