import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-survey-details',
  templateUrl: './survey-details.component.html',
  styleUrls: ['./survey-details.component.css']
})
export class SurveyDetailsComponent implements OnInit {
  patientId: string = '';
  patient: any = null;
  survey: any = null;
  loading = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: ApiService
  ) {}

  ngOnInit(): void {
    this.patientId = this.route.snapshot.paramMap.get('id') || '';
    if (this.patientId) {
      this.loadSurveyDetails();
    }
  }

  loadSurveyDetails(): void {
    this.loading = true;
    this.apiService.getPatientById(this.patientId).subscribe({
      next: (patientResponse) => {
        this.patient = patientResponse.data;
        
        this.apiService.getSurveyByPatientId(this.patientId).subscribe({
          next: (surveyResponse) => {
            this.survey = surveyResponse.data;
            this.loading = false;
          },
          error: (err) => {
            console.error('Error loading survey:', err);
            this.loading = false;
          }
        });
      },
      error: (err) => {
        console.error('Error loading patient:', err);
        this.loading = false;
        this.router.navigate(['/dashboard']);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }

  getObjectKeys(obj: any): string[] {
    return obj ? Object.keys(obj) : [];
  }

  formatKey(key: string): string {
    return key.replace(/([A-Z])/g, ' $1').replace(/^./, str => str.toUpperCase());
  }

  formatValue(value: any): string {
    if (typeof value === 'boolean') {
      return value ? 'Yes' : 'No';
    }
    if (Array.isArray(value)) {
      return value.join(', ');
    }
    if (typeof value === 'object' && value !== null) {
      return JSON.stringify(value, null, 2);
    }
    return value?.toString() || 'N/A';
  }
}
