import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  username = '';
  password = '';
  loading = false;
  showPassword = false;

  constructor(
    private http: HttpClient,
    private router: Router,
    private toastr: ToastrService
  ) {}

  login() {
    if (!this.username || !this.password) {
      this.toastr.warning('Please fill in all fields!');
      return;
    }

    this.loading = true;

    const body = { username: this.username, password: this.password };

    this.http.post<any>('https://localhost:44313/api/Auth/login', body)
      .subscribe({
        next: (res) => {
          localStorage.setItem('token', res.token);
          this.toastr.success('Welcome back, Admin!');
          this.router.navigate(['/admin']);
        },
        error: () => {
          this.toastr.error('Invalid username or password!');
          this.loading = false;
        }
      });
  }
}
