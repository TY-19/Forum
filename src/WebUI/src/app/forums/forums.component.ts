import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ForumService } from './forum.service';
import { Subforum } from '../common/models/subforum';
import { Category } from '../common/models/category';
import { CategoriesComponent } from "../categories/categories.component";
import { Forum } from '../common/models/forum';

@Component({
    selector: 'app-forums',
    standalone: true,
    templateUrl: './forums.component.html',
    styleUrl: './forums.component.scss',
    imports: [CommonModule, RouterLink, CategoriesComponent]
})
export class ForumsComponent implements OnInit {
  forum!: Forum;

  constructor(private forumService: ForumService) {

  }

  ngOnInit(): void {
    this.forumService.getForums()
      .subscribe(response => {
        this.forum = response;
      });
  };
}
