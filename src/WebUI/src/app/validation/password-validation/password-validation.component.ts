import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-password-validation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './password-validation.component.html',
  styleUrl: './password-validation.component.scss'
})
export class PasswordValidationComponent {
  @Input() form!: FormGroup;
  @Input() controlName: string = "password";
}
