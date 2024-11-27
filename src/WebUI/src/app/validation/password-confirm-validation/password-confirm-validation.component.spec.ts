import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PasswordConfirmValidationComponent } from './password-confirm-validation.component';

describe('PasswordConfirmValidationComponent', () => {
  let component: PasswordConfirmValidationComponent;
  let fixture: ComponentFixture<PasswordConfirmValidationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PasswordConfirmValidationComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PasswordConfirmValidationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
