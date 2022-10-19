import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from '../config.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PolicyService {

  constructor(private httpClient: HttpClient) { }

  getPolicies(): Observable<any[]> {
    return this.httpClient.get<any[]>(ConfigService.getConfig().apiRoot + `/policy`)
  }

  updatePolicy(key: string, policy: string): Observable<void> {
    throw("not implemented")
  }

}
