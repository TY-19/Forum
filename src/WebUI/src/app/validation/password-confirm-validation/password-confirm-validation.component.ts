import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-password-confirm-validation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './password-confirm-validation.component.html',
  styleUrl: './password-confirm-validation.component.scss'
})
export class PasswordConfirmValidationComponent {
  @Input() form!: FormGroup;
  @Input() controlName: string = "passwordConfirm";
}
