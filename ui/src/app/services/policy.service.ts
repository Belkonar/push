import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from '../config.service';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';

@Injectable({
  providedIn: 'root'
})
export class PolicyService {

  constructor(private httpClient: HttpService) { }

  getPolicies(): Observable<any[]> {
    return this.httpClient.get<any[]>(`/policy`)
  }

  /***
   *
   * @param key
   * @param policy
   */
  updatePolicy(key: string, policy: string): Observable<any> {
    return this.httpClient.put<any[]>(`/policy/${key}`, {
      policy
    })
  }

  createPolicy(key: string): Observable<any> {
    return this.httpClient.post<any[]>(`/policy/${key}`)
  }

}
