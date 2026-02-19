import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import * as QRCode from 'qrcode';

@Component({
  selector: 'app-mfa-setup',
  templateUrl: './mfa-setup.component.html',
  styleUrls: ['./mfa-setup.component.css']
})
export class MfaSetupComponent implements OnInit {
  user: any = null;
  qrCodeUrl: string = '';
  secret: string = '';
  backupCodes: string[] = [];
  verificationCode: string = '';
  loading = false;
  step: 'qr' | 'verify' | 'backup' = 'qr';

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.user = this.authService.getUser();
    if (!this.user) {
      this.router.navigate(['/']);
      return;
    }
    this.loadMfaSetup();
  }

  loadMfaSetup(): void {
    this.loading = true;
    this.apiService.getMfaSetup(this.user.id).subscribe({
      next: (response) => {
        this.secret = response.data.secret;
        this.backupCodes = response.data.backupCodes;
        this.generateQRCode(response.data.qrCodeUrl);
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading MFA setup:', err);
        this.loading = false;
        alert('Failed to load MFA setup');
      }
    });
  }

  generateQRCode(url: string): void {
    QRCode.toDataURL(url, { width: 300 }).then((dataUrl) => {
      this.qrCodeUrl = dataUrl;
    });
  }

  verifyAndEnable(): void {
    if (!this.verificationCode || this.verificationCode.length !== 6) {
      alert('Please enter a valid 6-digit code');
      return;
    }

    this.loading = true;
    // Send both code and secret to backend
    this.apiService.enableMfaWithSecret(this.user.id, this.verificationCode, this.secret).subscribe({
      next: (response) => {
        this.loading = false;
        this.step = 'backup';
      },
      error: (err) => {
        console.error('Error enabling MFA:', err);
        this.loading = false;
        alert('Invalid code. Please try again.');
      }
    });
  }

  downloadBackupCodes(): void {
    const content = this.backupCodes.join('\n');
    const blob = new Blob([content], { type: 'text/plain' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'pv-health-backup-codes.txt';
    a.click();
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }

  finish(): void {
    this.router.navigate(['/dashboard']);
  }
}
