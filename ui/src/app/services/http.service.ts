import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { map, Observable, switchMap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from '../config.service';

@Injectable({
  providedIn: 'root'
})
export class HttpService {

  constructor(private authService: AuthService, private httpClient: HttpClient) { }

  get<TVal>(path: string): Observable<TVal> {
    return this.authService.getConfig()
      .pipe(switchMap(config => {
        return this.httpClient.get<TVal>(ConfigService.getConfig().apiRoot + path, config);
      }));
  }

  put<TVal>(path: string, body: unknown): Observable<TVal> {
    return this.authService.getConfig()
      .pipe(switchMap(config => {
        return this.httpClient.put<TVal>(ConfigService.getConfig().apiRoot + path, body,config);
      }));
  }
}
