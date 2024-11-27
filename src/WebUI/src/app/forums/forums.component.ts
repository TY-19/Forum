import { Component, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ForumService } from './forum.service';
import { CategoriesComponent } from "../categories/categories.component";
import { Forum } from '../common/models/forum';
import { Category } from '../common/models/category';
import { Subforum } from '../common/models/subforum';

@Component({
    selector: 'app-forums',
    standalone: true,
    templateUrl: './forums.component.html',
    styleUrl: './forums.component.scss',
    imports: [CommonModule, RouterLink, CategoriesComponent]
})
export class ForumsComponent implements OnInit {
  forum!: Forum;

  constructor(private forumService: ForumService
    ) {

  }

  ngOnInit(): void {
    this.forumService.getForums()
      .subscribe(response => {
        this.forum = response;
      });

      this.populateCategories();
  };

  categories: Category[] = [{
    "id": 1,
    "name": "Backend",
    "parentForumId": null,
    "position": 2
},
{
    "id": 2,
    "name": "General",
    "parentForumId": null,
    "position": 1
},
{
    "id": 3,
    "name": "Entertainment",
    "parentForumId": null,
    "position": 4
},
{
    "id": 4,
    "name": "News",
    "parentForumId": null,
    "position": 5
},
{
    "id": 5,
    "name": "Hobby",
    "parentForumId": null,
    "position": 6
}];
  sf: Subforum[] = [{
    "id": 2,
    "name": "Backend top level",
    "parentForumId": null,
    "category": "Backend",
    "description": "The backend forum",
    "position": 1,
    "isClosed": false,
    "isUnread": false,
    "subforumsCount": 0,
    "topicsCount": 0
},
{
    "id": 3,
    "name": "Backend top level",
    "parentForumId": null,
    "category": "General",
    "description": "The backend forum",
    "position": 2,
    "isClosed": false,
    "isUnread": false,
    "subforumsCount": 0,
    "topicsCount": 0
},
{
    "id": 4,
    "name": "Backend top level",
    "parentForumId": null,
    "category": "News",
    "description": "The backend forum",
    "position": 3,
    "isClosed": false,
    "isUnread": false,
    "subforumsCount": 0,
    "topicsCount": 0
},
];
  subforums: Map<string | null, Subforum[]> = new Map<string | null, Subforum[]>();
  reloadForum(id: string|number) {}

  dropAction: EventEmitter<DropAction> = new EventEmitter<DropAction>;

  populateCategories(): void {
      for(let sf of this.sf) {
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

















  isDraggingItem: boolean = false;
  isDraggingCategory: boolean = false;
  
  draggedObj: Node | null = null;
  dragOverId: string | null = null;

  onDragStart(event: DragEvent, isCategory: boolean = false, isItem: boolean = false): void {
    if(isCategory) {
      this.draggedObj = (event.target as HTMLElement).parentNode as Node;
      this.isDraggingCategory = true;
    } else if(isItem) {
      this.draggedObj = event.target as Node;
      this.isDraggingItem = true;
    }
  }
  onDragEnter(event: DragEvent, category: string | null): void {
    event.preventDefault();
    if(this.isDraggingCategory) {
      this.dragOverId = category;
    }
    else if(this.isDraggingItem && category == null) {
      console.log(event.target);
      this.dragOverId = (event.target as HTMLElement).id;
    }
  }
  onDragEnd(event: DragEvent): void {
    console.log(event);
    console.log(this.dragOverId);
    event.preventDefault();
    if(this.dragOverId != null) {
      let target = document.getElementById(this.dragOverId);
      console.log(target);
      if(target !== this.draggedObj && target && target.parentNode
        && this.draggedObj && this.draggedObj.parentNode) {
          target.parentNode.insertBefore(this.draggedObj, target);

          if(this.isDraggingCategory) {
            this.dropAction.emit({

              sourceId: "",
              targetId: ""
            })
            // TODO: make a call to the backend
            console.log((this.draggedObj as HTMLElement).id);
            console.log(target.id);
          } else if(this.isDraggingItem) {
            // TODO: make a call to the backend
            console.log((this.draggedObj as HTMLElement).id);
            console.log((target.parentNode as HTMLElement).id);
            console.log(target.id);
          }
      }
    } 

    this.isDraggingItem = false;
    this.isDraggingCategory = false;
    this.dragOverId = null;
    this.draggedObj = null;
  }
}

export interface DropAction {
 sourceId: string;
 targetId: string; 
}