import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-email-validation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './email-validation.component.html',
  styleUrl: './email-validation.component.scss'
})
export class EmailValidationComponent {
  @Input() form!: FormGroup;
  @Input() controlName: string = "userEmail";
}
