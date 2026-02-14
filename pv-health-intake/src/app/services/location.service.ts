import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LocationService {
  private apiUrl = 'http://localhost:5247/api/location';
  constructor(private http: HttpClient) { }
  getCountries(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/countries`);
  }
  getStates(countryCode: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/countries/${countryCode}/states`).pipe(
      tap(() => this.prefetchCountryData(countryCode))
    );
  }
  getCities(countryCode: string, stateCode: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/countries/${countryCode}/states/${stateCode}/cities`);
  }

  private prefetchCountryData(countryCode: string): void {
    this.http.post(`${this.apiUrl}/countries/${countryCode}/prefetch`, {}).subscribe();
  }
}
