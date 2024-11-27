import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { CurrentUserService } from '../authentication/current-user.service';
import { AuthenticationService } from '../authentication/authentication.service';

@Component({
  selector: 'app-nav-menu',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './nav-menu.component.html',
  styleUrl: './nav-menu.component.scss'
})
export class NavMenuComponent {
  
  private isAuthenticated: boolean = false;
  protected get isLoggedIn() {
      return this.isAuthenticated;
  }
  constructor(private authService: AuthenticationService,
    private router: Router) {
    
  }

  ngOnInit(): void {
    this.isAuthenticated = this.authService.isAuthenticated();
    this.subscribeToLoginStatusChanges();
  }
    
  subscribeToLoginStatusChanges() {
    this.authService.authStatus
        .subscribe(response => this.isAuthenticated = response);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
