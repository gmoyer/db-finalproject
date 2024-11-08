import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {

  user: User | null = null;

  constructor(private userService: UserService, private router: Router) { }

  ngOnInit(): void {
    this.userService.user$.subscribe(user => this.user = user);
  }

  logout(): void {
    this.userService.logout();
    this.router.navigate(['/login']);
  }

}
