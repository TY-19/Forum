import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsernameValidationComponent } from './username-validation.component';

describe('UsernameValidationComponent', () => {
  let component: UsernameValidationComponent;
  let fixture: ComponentFixture<UsernameValidationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UsernameValidationComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(UsernameValidationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
