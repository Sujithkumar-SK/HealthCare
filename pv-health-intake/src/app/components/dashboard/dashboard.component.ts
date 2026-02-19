import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { ApiService } from 'src/app/services/api.service';
import { SurveyDataService } from 'src/app/services/survey-data.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  user: any = null;
  patients: any[] = [];
  loading = false;
  mfaEnabled = false;
  loadingMfaStatus = false;

  constructor(
    private authService: AuthService,
    private apiService: ApiService,
    private router: Router,
    private surveyDataService: SurveyDataService
  ) {}

  ngOnInit(): void {
    this.user = this.authService.getUser();
    if (!this.user) {
      this.router.navigate(['/']);
      return;
    }
    this.loadUserPatients();
    this.loadMfaStatus();
  }

  loadUserPatients(): void {
    this.loading = true;
    this.apiService.getPatientsByUserId(this.user.id).subscribe({
      next: (response) => {
        this.patients = response.data || [];
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading patients:', err);
        this.loading = false;
      }
    });
  }

  addNewPatient(): void {
    this.router.navigate(['/landing']);
  }

  fillSurvey(patient: any): void {
    this.surveyDataService.setPatientData(patient);
    this.router.navigate(['/survey']);
  }

  viewSurvey(patient: any): void {
    this.router.navigate(['/survey-details', patient.id]);
  }

  loadMfaStatus(): void {
    this.loadingMfaStatus = true;
    this.apiService.getMfaStatus(this.user.id).subscribe({
      next: (response) => {
        this.mfaEnabled = response.data?.mfaEnabled || false;
        this.loadingMfaStatus = false;
      },
      error: (err) => {
        console.error('Error loading MFA status:', err);
        this.loadingMfaStatus = false;
      }
    });
  }

  navigateToMfaSetup(): void {
    this.router.navigate(['/mfa-setup']);
  }

  disableMfa(): void {
    if (confirm('Are you sure you want to disable Two-Factor Authentication?')) {
      this.apiService.disableMfa(this.user.id).subscribe({
        next: () => {
          this.mfaEnabled = false;
          alert('Two-Factor Authentication disabled successfully');
        },
        error: (err) => {
          console.error('Error disabling MFA:', err);
          alert('Failed to disable MFA');
        }
      });
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
