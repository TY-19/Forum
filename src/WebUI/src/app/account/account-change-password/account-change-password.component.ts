import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { fieldsEqual } from '../../common/validators/fields-equal';
import { PasswordConfirmValidationComponent } from '../../validation/password-confirm-validation/password-confirm-validation.component';
import { PasswordValidationComponent } from '../../validation/password-validation/password-validation.component';
import { DisplayMode } from '../../common/enums/display-mode';
import { ProfileTab } from '../../common/enums/profile-tab';
import { AccountService } from '../account.service';
import { ChangePasswordModel } from '../../common/models/change-password';
import { HttpErrorResponse } from '@angular/common/http';
import { HttpResponseHelper } from '../../common/helpers/http-response-helper';

@Component({
  selector: 'app-account-change-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,
    PasswordValidationComponent, PasswordConfirmValidationComponent],
  templateUrl: './account-change-password.component.html',
  styleUrl: './account-change-password.component.scss'
})
export class AccountChangePasswordComponent implements OnInit {
  @Output() changeTab = new EventEmitter<ProfileTab>();
  form!: FormGroup;
  responseMessage: string | null = null;
  succeeded: boolean | null = null;

  constructor(private accountService: AccountService) {

  }

  ngOnInit(): void {
    this.initiateForm();
  }

  private initiateForm(): void {
    this.form = new FormGroup({
      oldPassword: new FormControl('', [Validators.required, Validators.minLength(8), Validators.maxLength(50)]),
      password: new FormControl('', [Validators.required, Validators.minLength(8), Validators.maxLength(50)]),
      passwordConfirm: new FormControl('', [Validators.required]),
    }, [ fieldsEqual("password", "passwordConfirm") ]);
  }

  onSubmit(): void {
    if(this.form.valid) {
      let passwordModel: ChangePasswordModel = {
        currentPassword: this.form?.controls['oldPassword'].value,
        newPassword: this.form?.controls['password'].value
      }
      this.changePassword(passwordModel);
    } else {
      this.form.markAllAsTouched();
    }
  }

  private changePassword(passwordModel: ChangePasswordModel): void {
    this.accountService.changePassword(passwordModel)
      .subscribe({ 
        next: result => {
          if(result && result.status === 204) {
            this.succeeded = true;
            this.responseMessage = "Password was successfully changed.";
            this.initiateForm();
          }
        },
        error: result => {
          if(result && result.status === 400) {
            this.succeeded = false;
            this.responseMessage = HttpResponseHelper.getErrorFromBadRequest(result as HttpErrorResponse);
          }
        }
      });
  }

  backToSecurityTab() {
    this.changeTab.emit(ProfileTab.Security);
  }
}
