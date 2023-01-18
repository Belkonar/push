import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {HttpService} from "./http.service";

@Injectable({
  providedIn: 'root'
})
export class OrgService {

  constructor(private httpService: HttpService) { }

  getOrgs(): Observable<any[]> {
    return this.httpService.get('/organization');
  }

  createOrg(name: string): Observable<void> {
    return this.httpService.post('/organization', {
      name
    })
  }
}
