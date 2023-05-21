import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';
import { Pagination } from '../_models/pagination';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members: Member[] | undefined
  predicate = 'liked'
  pageNumber = 1
  pageSize = 5
  pagination: Pagination | undefined


  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    this.loadLike()
  }

  loadLike() {
    this.memberService.GetLikes(this.predicate, this.pageNumber, this.pageSize).subscribe({
      next: response => {
        this.members = response.result;
        this.pagination = response.pagination;
      }
    })
  }
  pageChanged(event: PageChangedEvent) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page
      this.loadLike()
    }
  }


}
