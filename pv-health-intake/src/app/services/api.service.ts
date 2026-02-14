import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'http://localhost:5247/api';

  constructor(private http: HttpClient) { }

  createPatient(patientData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/patients`, patientData);
  }

  getPatientById(id: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/patients/${id}`);
  }

  getAllPatients(): Observable<any> {
    return this.http.get(`${this.apiUrl}/patients`);
  }


  createSurvey(surveyData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/surveys`, surveyData);
  }

  getSurveyById(id: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/surveys/${id}`);
  }

  getSurveyByPatientId(patientId: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/surveys/patient/${patientId}`);
  }
}
