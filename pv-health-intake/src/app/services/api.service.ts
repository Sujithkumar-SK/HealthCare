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

  createPatientWithSurvey(patientData: any, surveyData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/patientintake`, {
      patient: patientData,
      survey: surveyData
    });
  }

  getSurveysByUserId(userId: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/patients/user/${userId}/surveys`);
  }

  getPatientsByUserId(userId: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/patients/user/${userId}`);
  }
  getMfaSetup(userId: string): Observable<any> {
  return this.http.get(`${this.apiUrl}/mfa/setup/${userId}`);
}

enableMfa(userId: string, code: string): Observable<any> {
  return this.http.post(`${this.apiUrl}/mfa/enable/${userId}`, { code });
}

enableMfaWithSecret(userId: string, code: string, secret: string): Observable<any> {
  return this.http.post(`${this.apiUrl}/mfa/enable/${userId}`, { code, secret });
}

disableMfa(userId: string): Observable<any> {
  return this.http.post(`${this.apiUrl}/mfa/disable/${userId}`, {});
}

verifyMfaCode(userId: string, code: string): Observable<any> {
  return this.http.post(`${this.apiUrl}/mfa/verify`, { userId, code });
}

verifyBackupCode(userId: string, code: string): Observable<any> {
  return this.http.post(`${this.apiUrl}/mfa/verify-backup`, { userId, code });
}

getMfaStatus(userId: string): Observable<any> {
  return this.http.get(`${this.apiUrl}/mfa/status/${userId}`);
}

}
