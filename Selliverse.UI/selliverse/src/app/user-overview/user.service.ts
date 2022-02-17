import {Injectable} from '@angular/core';

import {Observable} from 'rxjs';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private http: HttpClient) {
  }

  getUsers(): Observable<string[]> {
    return this.http.get<string[]>(environment.base_url + '/player')
  }
}
