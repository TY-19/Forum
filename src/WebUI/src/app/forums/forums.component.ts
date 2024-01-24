import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-forums',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './forums.component.html',
  styleUrl: './forums.component.scss'
})
export class ForumsComponent {

}
