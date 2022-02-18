import {Component} from '@angular/core';
import {delay, mapTo, Observable, startWith} from "rxjs";
import {Router} from "@angular/router";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {

  isLoaded$ = new Observable<boolean>().pipe(startWith(false), delay(4000), mapTo(true))

  constructor(private router: Router) {
  }

  connect() {
    this.router.navigate(['game-host']);
  }

}
