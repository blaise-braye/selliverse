import {Component} from '@angular/core';
import {delay, map, mapTo, Observable, startWith} from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Selliverse';

  isLoaded$ = new Observable<boolean>().pipe(startWith(false), delay(4000), mapTo(true))

  connect() {
    window.location.href = '/game';
  }
}


