import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SurveyDataService } from 'src/app/services/survey-data.service';

@Component({
  selector: 'app-review',
  templateUrl: './review.component.html',
  styleUrls: ['./review.component.css']
})
export class ReviewComponent implements OnInit {
  patientData: any;
  surveyData: any;
  constructor(
    private router: Router,
    private surveyDataService: SurveyDataService
  ) { }

  ngOnInit(): void {
    const allData = this.surveyDataService.getAllData();
    this.patientData = allData.patient;
    this.surveyData = allData.survey;

    if(!this.patientData || !this.surveyData){
      this.router.navigate(['/']);
    }
  }
  onEdit(){
    this.router.navigate(['/survey']);
  }
  onSubmit(){
    this.router.navigate(['/dashboard']);
  }
}
