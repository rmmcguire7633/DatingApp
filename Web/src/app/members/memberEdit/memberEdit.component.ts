import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/user';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-memberEdit',
  templateUrl: './memberEdit.component.html',
  styleUrls: ['./memberEdit.component.css']
})
export class MemberEditComponent implements OnInit {
  user: User;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user']
    });
  }

}
