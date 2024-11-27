import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ForumService } from '../../forum.service';
import { ForumsStructure } from '../../../common/models/forums-structure';

@Component({
  selector: 'app-parent-forum-selector',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './parent-forum-selector.component.html',
  styleUrl: './parent-forum-selector.component.scss'
})
export class ParentForumSelectorComponent implements OnInit {
  @ViewChild('selector') selector!: ElementRef;
  options: ParentOption[] = [];
  
  constructor(private forumService: ForumService) {

  }

  ngOnInit(): void {
    this.forumService.getForumsStructure()
    .subscribe(response => {
      this.getOptions(response);
    });
  }

  private getOptions(structure: ForumsStructure[]): void {
    for(let elem of structure) {
      this.options.push({ 
        id: elem.id,
        name: elem.name,
        level: 0
      });
      
      if(elem.subElements) {
        this.getSubOptions(elem, 1);
      }
    }
  }

  private getSubOptions(forumStructure: ForumsStructure, level: number): void {
    for(let se of forumStructure.subElements) {
      this.options.push({ 
        id: se.id,
        name: se.name,
        level: level
      });
      if(se.subElements) {
        this.getSubOptions(se, level + 1);
      }
    }
  }

  getName(opt: ParentOption): string {
    return this.getPrefix(opt.level) + opt.name;
  }

  private getPrefix(level: number): string {
    let prefix: string = "";
    if(level <= 0) {
      return prefix;
    }
    
    for(let i = 0; i < level; i++) {
      prefix += "|";
    }
    prefix += "-";
    return prefix;
  }

  getParentForumId(): number | null {
    let parentForumId = parseInt(this.selector.nativeElement.value);
    return isNaN(parentForumId) ? null : parentForumId;
  }
}

interface ParentOption {
  id: number,
  name: string,
  level: number
}
