import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { baseUrl } from '../app.config';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  logoUrl: string = baseUrl + "/design/logo.png"
}
