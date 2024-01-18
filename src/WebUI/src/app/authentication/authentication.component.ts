import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthenticationService } from './authentication.service';
import { LoginRequest } from '../common/models/login-request';
import { LoginResponse } from '../common/models/login-response';
import { Router } from '@angular/router';
import { baseUrl } from '../app.config';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-authentication',
  standalone: true,
  imports: [CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './authentication.component.html',
  styleUrl: './authentication.component.scss'
})
export class AuthenticationComponent implements OnInit {
  form!: FormGroup;
  loginResponse!: LoginResponse;

  constructor(private router: Router, private http: HttpClient,
    private authenticationService: AuthenticationService) {

  }

  ngOnInit(): void {
    this.initiateForm();
  }

  private initiateForm(): void {
    this.form = new FormGroup({
      userName: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required),
    });
  }

  onSubmit() {
    if (this.form.valid) {
      const loginRequest: LoginRequest = {
        userName: this.form?.controls['userName'].value,
        password: this.form?.controls['password'].value
      }
      this.login(loginRequest);
    } else {
      this.form.markAllAsTouched();
    }
  }

  private login(loginRequest: LoginRequest) {
    this.authenticationService.login(loginRequest)
        .subscribe({
          next: result => {
            this.loginResponse = result;
            if (result.succeeded && result.token) {
              this.router.navigate(["/"]);
            }}
      });
  }


}
