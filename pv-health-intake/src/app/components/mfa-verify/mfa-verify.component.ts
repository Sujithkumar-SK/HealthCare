import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-mfa-verify',
  templateUrl: './mfa-verify.component.html',
  styleUrls: ['./mfa-verify.component.css']
})
export class MfaVerifyComponent implements OnInit {
  code: string = '';
  loading = false;
  useBackupCode = false;
  userId: string = '';

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const tempUser = sessionStorage.getItem('mfa_temp_user');
    if (!tempUser) {
      this.router.navigate(['/']);
      return;
    }
    this.userId = JSON.parse(tempUser).id;
  }

  verify(): void {
    if (!this.code || this.code.length < 6) {
      alert('Please enter a valid code');
      return;
    }

    this.loading = true;
    const verifyMethod = this.useBackupCode 
      ? this.apiService.verifyBackupCode(this.userId, this.code)
      : this.apiService.verifyMfaCode(this.userId, this.code);

    verifyMethod.subscribe({
      next: (response) => {
        if (response.success) {
          const tempUser = sessionStorage.getItem('mfa_temp_user');
          const tempToken = sessionStorage.getItem('mfa_temp_token');
          
          if (tempUser && tempToken) {
            localStorage.setItem('user', tempUser);
            localStorage.setItem('jwt_token', tempToken);
            sessionStorage.removeItem('mfa_temp_user');
            sessionStorage.removeItem('mfa_temp_token');
            
            this.router.navigate(['/dashboard']);
          }
        } else {
          this.loading = false;
          alert('Invalid code. Please try again.');
        }
      },
      error: (err) => {
        console.error('Verification failed:', err);
        this.loading = false;
        alert('Verification failed. Please try again.');
      }
    });
  }

  toggleBackupCode(): void {
    this.useBackupCode = !this.useBackupCode;
    this.code = '';
  }
}
