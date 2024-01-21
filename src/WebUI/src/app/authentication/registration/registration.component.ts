import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RegistrationRequest } from '../../common/models/registration-request';
import { AuthenticationService } from '../authentication.service';
import { Router } from '@angular/router';
import { fieldsEqual } from '../../common/validators/fields-equal';
import { HttpErrorResponse } from '@angular/common/http';
import { HttpResponseHelper } from '../../common/helpers/http-response-helper';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.scss'
})
export class RegistrationComponent {
  form!: FormGroup;
  registrationError: string | null = null;

  constructor(private router: Router,
    private authenticationService: AuthenticationService) {

  }

  ngOnInit(): void {
    this.initiateForm();
  }

  private initiateForm(): void {
    this.form = new FormGroup({
      userName: new FormControl('', [Validators.required, Validators.minLength(3),
        Validators.maxLength(100), Validators.pattern("^[a-zA-Z0-9.\-_@+ а-яґєіїА-ЯҐЄІЇ]+$")]),
      userEmail: new FormControl('', [Validators.required, Validators.email, Validators.maxLength(500)]),
      password: new FormControl('', [Validators.required, Validators.minLength(8), Validators.maxLength(50)]),
      passwordConfirm: new FormControl('', [Validators.required]),
    }, [ fieldsEqual("password", "passwordConfirm") ]);
  }

  onSubmit() {
    console.log(this.form);
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
      .subscribe({ 
        next: result => {
          if(result && result.status === 204) {
            this.router.navigate([''])
          }
        },
        error: result => {
          if(result && result.status === 400) {
            this.registrationError = HttpResponseHelper.getErrorFromBadRequest(result as HttpErrorResponse)
          }
        }
      });
  }

  protected getInvalidCharacters(input: string) {
    const pattern = /[^a-zA-Z0-9\-._@+ абвгґдеєжзиіїйклмнопрстуфхцчшщьюяАБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ]/g;
    const regex = new RegExp(pattern);
    const matches = input.match(regex);
    return matches?.map(match => match[0]) || [];
  }
}
