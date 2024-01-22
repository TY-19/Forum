import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { User } from '../../common/models/user';
import { DisplayMode } from '../../common/enums/display-mode';

@Component({
  selector: 'app-account-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './account-view.component.html',
  styleUrl: './account-view.component.scss'
})
export class AccountViewComponent {
  @Input() profile: User | null = null;
  @Output() changeMode = new EventEmitter<DisplayMode>();

  changeToEditMode() {
    this.changeMode.emit(DisplayMode.Edit);
  }
}
