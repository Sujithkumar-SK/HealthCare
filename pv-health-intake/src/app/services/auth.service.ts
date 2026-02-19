import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, from } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

declare const google: any;

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl;
  private client: any;

  constructor(private http: HttpClient) {}

  async initialize(): Promise<void> {
    await this.loadGoogleScript();
    return new Promise((resolve) => {
      this.client = google.accounts.oauth2.initTokenClient({
        client_id: environment.googleClientId,
        scope: 'email profile openid',
        callback: () => {}
      });
      resolve();
    });
  }

  private loadGoogleScript(): Promise<void> {
    return new Promise((resolve) => {
      if (typeof google !== 'undefined') {
        resolve();
        return;
      }
      const script = document.createElement('script');
      script.src = 'https://accounts.google.com/gsi/client';
      script.onload = () => resolve();
      document.head.appendChild(script);
    });
  }

  loginWithGoogle(): Observable<any> {
    return new Observable(observer => {
      this.client.callback = (response: any) => {
        if (response.access_token) {
          this.http.get('https://www.googleapis.com/oauth2/v3/userinfo', {
            headers: { Authorization: `Bearer ${response.access_token}` }
          }).subscribe({
            next: (userInfo: any) => {
              const idToken = response.access_token;
              this.http.post<any>(`${this.apiUrl}/auth/google`, { idToken }).subscribe({
                next: (authResponse: any) => {
                  if (authResponse.requiresMfa) {
                    sessionStorage.setItem('mfa_temp_user', JSON.stringify(authResponse.user));
                    sessionStorage.setItem('mfa_temp_token', authResponse.token);
                  } else {
                    localStorage.setItem('jwt_token', authResponse.token);
                    localStorage.setItem('user', JSON.stringify(authResponse.user));
                  }
                  observer.next(authResponse);
                  observer.complete();
                },
                error: (err) => {
                  observer.error(err);
                }
              });
            },
            error: (err) => observer.error(err)
          });
        } else {
          observer.error('No access token received');
        }
      };
      this.client.requestAccessToken();
    });
  }

  loginWithGitHub(): Observable<any> {
    const clientId = environment.githubClientId;
    const redirectUri = 'http://localhost:4200';
    const scope = 'read:user user:email';
    
    const authUrl = `https://github.com/login/oauth/authorize?client_id=${clientId}&redirect_uri=${redirectUri}&scope=${scope}`;
    
    return new Observable(observer => {
      const popup = window.open(authUrl, 'GitHub Login', 'width=600,height=700');
      
      const checkPopup = setInterval(() => {
        try {
          if (popup?.closed) {
            clearInterval(checkPopup);
            observer.error('Login cancelled');
          }
          
          if (popup?.location.href.includes('code=')) {
            const url = new URL(popup.location.href);
            const code = url.searchParams.get('code');
            popup.close();
            clearInterval(checkPopup);
            
            if (code) {
              this.http.post<any>(`${this.apiUrl}/auth/github`, { code }).subscribe({
                next: (response) => {
                  if (response.requiresMfa) {
                    sessionStorage.setItem('mfa_temp_user', JSON.stringify(response.user));
                    sessionStorage.setItem('mfa_temp_token', response.token);
                  } else {
                    localStorage.setItem('jwt_token', response.token);
                    localStorage.setItem('user', JSON.stringify(response.user));
                  }
                  observer.next(response);
                  observer.complete();
                },
                error: (err) => observer.error(err)
              });
            }
          }
        } catch (e) {
          // Cross-origin error, popup still on GitHub
        }
      }, 500);
    });
  }

  authenticateWithGitHubCode(code: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/auth/github`, { code }).pipe(
      tap((response) => {
        if (response.requiresMfa) {
          sessionStorage.setItem('mfa_temp_user', JSON.stringify(response.user));
          sessionStorage.setItem('mfa_temp_token', response.token);
        } else {
          localStorage.setItem('jwt_token', response.token);
          localStorage.setItem('user', JSON.stringify(response.user));
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('user');
    if (typeof google !== 'undefined' && google.accounts?.id) {
      google.accounts.id.disableAutoSelect();
    }
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('jwt_token');
  }

  getToken(): string | null {
    return localStorage.getItem('jwt_token');
  }

  getUser(): any {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }
}
