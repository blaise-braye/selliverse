import {ChangeDetectionStrategy, Component} from '@angular/core';
import {UserService} from "./user.service";
import {interval, startWith, switchMap} from "rxjs";

@Component({
  selector: 'app-user-overview',
  templateUrl: './user-overview.component.html',
  styleUrls: ['./user-overview.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserOverviewComponent {

  users$ = interval(5000).pipe(
    startWith(this.userService.getUsers()), // Get the data when starting
    switchMap(() => this.userService.getUsers()))

  constructor(private userService: UserService) {
  }


}
