import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SurveyDataService {

  constructor() { }
  private patientData: any ={};
  private surveyData: any={};

  setPatientData(data: any){
    this.patientData = data;
  }

  getPatientData(){
    return this.patientData;
  }

  setSurveyData(data:any){
    this.surveyData = data;
  }

  getSurveyData(){
    return this.surveyData;
  }

  getAllData(){
    return {
      patient: this.patientData,
      survey: this.surveyData
    };
  }

  clearAllData(){
    this.patientData={};
    this.surveyData={};
  }
}
