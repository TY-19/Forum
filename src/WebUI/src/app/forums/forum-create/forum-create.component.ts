import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ForumCreate } from '../../common/models/forum-create';
import { ForumService } from '../forum.service';
import { HttpResponseHelper } from '../../common/helpers/http-response-helper';
import { HttpErrorResponse } from '@angular/common/http';
import { ForumsStructure } from '../../common/models/forums-structure';
import { ParentForumSelectorComponent } from './parent-forum-selector/parent-forum-selector.component';

@Component({
  selector: 'app-forum-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, ParentForumSelectorComponent],
  templateUrl: './forum-create.component.html',
  styleUrl: './forum-create.component.scss'
})
export class ForumCreateComponent implements OnInit {
  @ViewChild(ParentForumSelectorComponent) parentSelector!: ParentForumSelectorComponent;
  form!: FormGroup;
  serverError: string | null = null;

  constructor(private forumService: ForumService,
    private router: Router) {

  }

  ngOnInit(): void {
    this.initiateForm();
  }

  private initiateForm(): void {
    this.form = new FormGroup({
      name: new FormControl('', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]),
      category: new FormControl('', [Validators.maxLength(50)]),
      description: new FormControl('', [Validators.maxLength(500)])
    })
  }

  onSubmit(): void {
    if(this.form.valid) {
      this.forumService.createForum(this.getForumFromForm())
        .subscribe({ 
          next: (response) => {
            if(response.status === 201 && response) {
              this.router.navigate(["forums", response.body?.id]);
            }
          }, 
          error: response => {
            this.serverError = HttpResponseHelper.getErrorFromBadRequest(response as HttpErrorResponse);
          }
      });
    } else {
      this.form.markAllAsTouched();
    }
  }
  private getForumFromForm(): ForumCreate {
    return {
      name: this.form?.controls['name'].value,
      category: this.form?.controls['category'].value,
      description: this.form?.controls['description'].value,
      parentForumId: this.parentSelector.getParentForumId()
    };
  }

}
