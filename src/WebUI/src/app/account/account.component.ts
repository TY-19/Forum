import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountService } from './account.service';
import { User } from '../common/models/user';
import { AccountViewComponent } from './account-view/account-view.component';
import { DisplayMode } from '../common/enums/display-mode';
import { AccountEditComponent } from './account-edit/account-edit.component';
import { AccountSecurityComponent } from './account-security/account-security.component';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [CommonModule, AccountViewComponent, AccountEditComponent, AccountSecurityComponent],
  templateUrl: './account.component.html',
  styleUrl: './account.component.scss'
})

export class AccountComponent implements OnInit {
  protected ProfileTab = ProfileTab;
  protected DisplayMode = DisplayMode;
  profileTab: ProfileTab = ProfileTab.General;
  mode: DisplayMode = DisplayMode.View;
  
  profile: User | null = null;
  
  constructor(private accountService: AccountService) {

  }

  ngOnInit(): void {
    this.getProfile();
  }

  private getProfile(): void {
    this.accountService.getProfile()
      .subscribe(response => this.profile = response);
  }

  changeTab(newTab: ProfileTab): void {
    this.profileTab = newTab;
  }

  changeMode(newMode: DisplayMode): void {
    this.mode = newMode;
  }

  getUpdatedProfile(hasBeenUpdated: boolean): void {
    if(hasBeenUpdated) {
      this.getProfile();
    }
  }
}

enum ProfileTab {
  General = 0,
  Security = 1
}