import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ForumService } from './forum.service';
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

  constructor(private forumService: ForumService
    ) {

  }

  ngOnInit(): void {
    this.forumService.getForums()
      .subscribe(response => {
        this.forum = response;
      });
  };

  dragged: HTMLElement | null = null;
  dragOverId: string | null = null;
  isDraggingItem: boolean = false;
  
  dragedCategory: Node | null = null;
  dragOverCategoryId: string | null = null;
  isDraggingCategory: boolean = false;


  onDragStartCategory(event: DragEvent): void {
    this.dragedCategory = (event.target as HTMLElement).parentNode as Node;
    this.isDraggingCategory = true;
  }
  onDragEnterCategory(event: DragEvent, category: string): void {
    if(!this.isDraggingCategory) {
      return;
    }
    event.preventDefault();
    this.dragOverCategoryId = category;

  }
  onDragOverCategory(event: DragEvent): void {
    event.preventDefault();
  }
  onDragLeaveCategory(event: DragEvent): void {
    event.preventDefault();
  }
  onDropCategory(event: DragEvent): void {
    if(!this.isDraggingCategory) {
      return;
    }
    event.preventDefault();
  }
  onDragEndCategory(event: DragEvent): void {
    event.preventDefault();
    if(this.dragOverCategoryId != null) {
      let target = document.getElementById(this.dragOverCategoryId);
      if(target !== this.dragedCategory) {
        if(target && target.parentNode
          && this.dragedCategory && this.dragedCategory.parentNode) {
            // TODO: make a call to the backend
            console.log((this.dragedCategory as HTMLElement).id);
            console.log(target.id);
            target.parentNode.insertBefore(this.dragedCategory, target);
        }
      }
    }
    this.dragOverCategoryId = null;
    this.dragedCategory = null;
    this.isDraggingCategory = false;
  }

  onDragStartItem(event: DragEvent): void {
    this.dragged = event.target as HTMLElement;
    this.isDraggingItem = true;
  }

  onDragEnterItem(event: DragEvent): void {
    event.preventDefault();
    this.dragOverId = (event.target as HTMLElement).id;
  }
  onDragOverItem(event: DragEvent): void {
    event.preventDefault();
  }
  onDragLeaveItem(event: DragEvent): void {
    event.preventDefault();
  }

  onDragEndItem(event: DragEvent): void {
    event.preventDefault();
    if(this.dragOverId !== null) {
      let target = document.getElementById(this.dragOverId);
      if(target !== this.dragged) {
        if(target && target.parentNode
          && this.dragged && this.dragged.parentNode) {
            // TODO: make a call to the backend
            console.log(this.dragged.id);
            console.log((target.parentNode as HTMLElement).id);
            console.log(target.id);
            target.parentNode.insertBefore(this.dragged, target);
        }
      }
    }
    this.dragOverId = null;
    this.isDraggingItem = false;
    this.dragged = null;
  }

  onDropItem(event: DragEvent): void {
    event.preventDefault();
  }
}
