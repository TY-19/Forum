import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ForumService } from '../forum.service';
import { Forum } from '../../common/models/forum';
import { CategoriesComponent } from "../../categories/categories.component";

@Component({
    selector: 'app-forum-view',
    standalone: true,
    templateUrl: './forum-view.component.html',
    styleUrl: './forum-view.component.scss',
    imports: [CommonModule, RouterLink, CategoriesComponent]
})
export class ForumViewComponent implements OnInit {
  forumId!: number;
  forum!: Forum;

  constructor(private forumService: ForumService,
    private activatedRoute: ActivatedRoute) {

  }

  ngOnInit(): void {
    this.forumId = Number.parseInt(this.activatedRoute.snapshot.paramMap.get("forumId") ?? "");
    this.getForum();
  }

  loadForum(id: number) {
    this.forumId = id;
    this.getForum();
  }

  private getForum(): void {
    this.forumService.getForum(this.forumId)
      .subscribe(response => {
        this.forum = response;
      });
  }
}
