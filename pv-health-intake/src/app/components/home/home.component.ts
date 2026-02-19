import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  loggingIn = false;
  loggingInGitHub = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
      return;
    }
    
    const urlParams = new URLSearchParams(window.location.search);
    const code = urlParams.get('code');
    if (code) {
      this.loggingInGitHub = true;
      this.authService.authenticateWithGitHubCode(code).subscribe({
        next: (response) => {
          this.loggingInGitHub = false;
          if (response.requiresMfa) {
            this.router.navigate(['/mfa-verify']);
          } else {
            this.router.navigate(['/dashboard']);
          }
        },
        error: (err) => {
          console.error('GitHub login failed:', err);
          this.loggingInGitHub = false;
          alert('GitHub login failed. Please try again.');
          window.location.href = '/';
        }
      });
    }
  }

  loginWithGoogle(): void {
    this.loggingIn = true;
    this.authService.loginWithGoogle().subscribe({
      next: (response) => {
        this.loggingIn = false;
        if (response.requiresMfa) {
          this.router.navigate(['/mfa-verify']);
        } else {
          this.router.navigate(['/dashboard']);
        }
      },
      error: (err) => {
        console.error('Login failed:', err);
        this.loggingIn = false;
        alert('Login failed. Please try again.');
      }
    });
  }

  loginWithGitHub(): void {
    this.loggingInGitHub = true;
    window.location.href = `https://github.com/login/oauth/authorize?client_id=Ov23liHfrASvCLSLNoJM&redirect_uri=http://localhost:4200&scope=read:user user:email`;
  }
}
