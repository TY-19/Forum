import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ForumService } from '../forum.service';
import { Forum } from '../../common/models/forum';
import { Subforum } from '../../common/models/subforum';

@Component({
  selector: 'app-forum-view',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './forum-view.component.html',
  styleUrl: './forum-view.component.scss'
})
export class ForumViewComponent implements OnInit {
  forumId!: number;
  forum!: Forum;
  subforums: Map<string | null, Subforum[]> = new Map<string | null, Subforum[]>();

  constructor(private forumService: ForumService,
    private activatedRoute: ActivatedRoute) {

  }

  ngOnInit(): void {
    this.forumId = Number.parseInt(this.activatedRoute.snapshot.paramMap.get("forumId") ?? "");
    this.getForum();
  }

  private getForum(): void {
    this.forumService.getForum(this.forumId)
      .subscribe(response => {
        this.forum = response;
        for(let sf of response.subforums) {
          let group = this.subforums.get(sf.category) || [];
          group.push(sf);
          this.subforums.set(sf.category, group);
        }
      });
  }

  viewForum(id: number): void {
    this.forumId = id;
    this.subforums = new Map<string | null, Subforum[]>();
    this.getForum();
  }
}
