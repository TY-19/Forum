import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-username-validation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './username-validation.component.html',
  styleUrl: './username-validation.component.scss'
})
export class UsernameValidationComponent {
  @Input() form!: FormGroup;
  @Input() controlName: string = "userName";

  getInvalidCharacters(input: string) {
    const pattern = /[^a-zA-Z0-9\-._@+ абвгґдеєжзиіїйклмнопрстуфхцчшщьюяАБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ]/g;
    const regex = new RegExp(pattern);
    const matches = input.match(regex);
    return matches?.map(match => match[0]) || [];
  }
}
