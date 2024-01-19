import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RegistrationRequest } from '../../common/models/registration-request';
import { AuthenticationService } from '../authentication.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.scss'
})
export class RegistrationComponent {
  form!: FormGroup;

  constructor(private router: Router,
    private authenticationService: AuthenticationService) {

  }

  ngOnInit(): void {
    this.initiateForm();
  }

  private initiateForm(): void {
    this.form = new FormGroup({
      userName: new FormControl('', Validators.required),
      userEmail: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', Validators.required),
      passwordConfirm: new FormControl('', Validators.required),
    });
  }

  onSubmit() {
    if (this.form.valid) {
      const registrationRequest: RegistrationRequest = {
        userName: this.form?.controls['userName'].value,
        userEmail: this.form?.controls['userEmail'].value,
        password: this.form?.controls['password'].value
      }
      this.registration(registrationRequest);
    } else {
      this.form.markAllAsTouched();
    }
  }

  private registration(registrationRequest: RegistrationRequest) {
    this.authenticationService
      .registration(registrationRequest)
      .subscribe(result => {
        if(result && result.status === 204) {
          this.router.navigate([''])
        }
    });
  }
}
