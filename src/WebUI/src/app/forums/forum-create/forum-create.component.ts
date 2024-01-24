import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ForumCreate } from '../../common/models/forum-create';
import { ForumService } from '../forum.service';
import { HttpResponseHelper } from '../../common/helpers/http-response-helper';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-forum-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './forum-create.component.html',
  styleUrl: './forum-create.component.scss'
})
export class ForumCreateComponent implements OnInit {
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
      parentForumId: new FormControl(''),
      category: new FormControl('', [Validators.maxLength(50)]),
      description: new FormControl('', [Validators.maxLength(500)])
    })
  }

  onSubmit() {
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
      parentForumId: this.getParentForumId()
    };
  }

  private getParentForumId(): number | null {
    let parentForumId = parseInt(this.form?.controls['parentForumId'].value);
    return isNaN(parentForumId) ? null : parentForumId;
  }
}
