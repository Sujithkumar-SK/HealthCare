import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SurveyDataService } from 'src/app/services/survey-data.service';

@Component({
  selector: 'app-confirmation',
  templateUrl: './confirmation.component.html',
  styleUrls: ['./confirmation.component.css']
})
export class ConfirmationComponent implements OnInit {
  confirmationNumber: string = '';
  patientData: any;
  surveyData: any;
  constructor(
    private router: Router,
    private surveyDataService: SurveyDataService
  ) { }

  ngOnInit(): void {
    this.patientData = this.surveyDataService.getPatientData();
    this.surveyData = this.surveyDataService.getSurveyData();
    this.confirmationNumber = this.patientData?.id || this.generateConfirmationNumber();
  }

  generateConfirmationNumber(): string {
    return 'PV-' + Math.random().toString(36).substr(2, 9).toUpperCase();
  }

  onStartNew(): void {
    this.surveyDataService.clearAllData();
    this.router.navigate(['/']);
  }
  downloadPDF(): void {

    alert('PDF download feature coming soon!');
  }
}
