import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ForumService } from './forum.service';
import { Subforum } from '../common/models/subforum';

@Component({
  selector: 'app-forums',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './forums.component.html',
  styleUrl: './forums.component.scss'
})
export class ForumsComponent implements OnInit {
  categories: string[] = [];
  forums: Map<string | null, Subforum[]> = new Map<string | null, Subforum[]>();

  constructor(private forumService: ForumService) {

  }

  ngOnInit(): void {
    this.forumService.getForums()
      .subscribe(response => { 
        this.categories = response.subcategories;
        for(let sf of response.subforums) {
          let group = this.forums.get(sf.category) || [];
          group.push(sf);
          this.forums.set(sf.category, group);
        }
      });
  };
}
