import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { baseUrl } from './app.config';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, HttpClientModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'WebUI';
  
  public response: object = null!;
  constructor(private http: HttpClient) {

  }

  ngOnInit(): void {
    this.http.get(baseUrl + "/api/test")
      .subscribe((result) => this.response = result);
  }
}
