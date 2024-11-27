import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParentForumSelectorComponent } from './parent-forum-selector.component';

describe('ParentForumSelectorComponent', () => {
  let component: ParentForumSelectorComponent;
  let fixture: ComponentFixture<ParentForumSelectorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ParentForumSelectorComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ParentForumSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
