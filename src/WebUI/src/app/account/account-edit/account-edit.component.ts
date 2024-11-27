import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { User } from '../../common/models/user';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DisplayMode } from '../../common/enums/display-mode';
import { UsernameValidationComponent } from '../../validation/username-validation/username-validation.component';
import { EmailValidationComponent } from '../../validation/email-validation/email-validation.component';
import { AccountService } from '../account.service';
import { ProfileUpdate } from '../../common/models/profile-update';

@Component({
  selector: 'app-account-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,
    UsernameValidationComponent, EmailValidationComponent],
  templateUrl: './account-edit.component.html',
  styleUrl: './account-edit.component.scss'
})
export class AccountEditComponent implements OnInit {
  @Input() profile: User | null = null;
  @Output() changeMode = new EventEmitter<DisplayMode>();
  @Output() hasBeenUpdated = new EventEmitter<boolean>();

  form!: FormGroup

  constructor(private accountService: AccountService) {
    
  }

  ngOnInit(): void {
    this.initiateForm();
  }

  private initiateForm(): void {
    this.form = new FormGroup({
      userName: new FormControl(this.profile?.userName ?? '', [Validators.required, Validators.minLength(3),
        Validators.maxLength(100), Validators.pattern("^[a-zA-Z0-9.\-_@+ а-яґєіїА-ЯҐЄІЇ]+$")]),
      userEmail: new FormControl(this.profile?.email ?? '', [ Validators.required, 
        Validators.email, Validators.maxLength(500)]),
    })
  }

  onSubmit(): void {
    if(this.form.valid) {
      let profileUpdate: ProfileUpdate = {
        userId: this.profile?.id ?? "",
        userName: this.profile?.userName ?? "",
        updatedName: this.form?.controls['userName'].value,
        updatedEmail: this.form?.controls['userEmail'].value
      }
      this.accountService.updateProfile(profileUpdate)
        .subscribe(() => {
          this.hasBeenUpdated.emit(true);
          this.changeToViewMode();
        });
    } else {
      this.form.markAllAsTouched();
    }
  }

  changeToViewMode() {
    this.changeMode.emit(DisplayMode.View);
  }
}
