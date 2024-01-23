import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileTab } from '../../common/enums/profile-tab';

@Component({
  selector: 'app-account-security',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './account-security.component.html',
  styleUrl: './account-security.component.scss'
})
export class AccountSecurityComponent {
  @Output() changeTab = new EventEmitter<ProfileTab>();
  
  toChangePassword() {
    this.changeTab.emit(ProfileTab.ChangePassword);
  }
}
