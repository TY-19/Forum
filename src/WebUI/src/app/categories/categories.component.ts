import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Forum } from '../common/models/forum';
import { Category } from '../common/models/category';
import { Subforum } from '../common/models/subforum';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss'
})
export class CategoriesComponent implements OnInit {
  @Input() set forum(value: Forum) {
    this.populateCategories(value);
  }
  @Output() forumIdChanged = new EventEmitter<number>();

  categories: Category[] = [];
  subforums: Map<string | null, Subforum[]> = new Map<string | null, Subforum[]>();

  ngOnInit(): void {
    
  }

  reloadForum(id: number | null): void {
    if(typeof(id) === 'number') {
      this.categories = [];
      this.subforums = new Map<string | null, Subforum[]>();
      this.forumIdChanged.emit(id);
    }
  }

  populateCategories(forum: Forum): void {
    if(forum != null) {
      this.categories = forum.subcategories.sort((a, b) => a.position - b.position);
      for(let sf of forum.subforums) {
        if(sf.category == null) {
          sf.category = "Uncategorized"
        }
        let group = this.subforums.get(sf.category) || [];
        group.push(sf);
        this.subforums.set(sf.category, group);
      }
      if(this.subforums.get("Uncategorized") != undefined && this.subforums.get("Uncategorized")!.length > 0) {
        let otherCategory: Category = {
          id: 0,
          name: "Uncategorized",
          parentForumId: 0,
          position: 0
        };
        this.categories.push(otherCategory);
      }
    }
  }
}
