import { Injectable } from '@angular/core';

// TODO: Move to services folder

export interface SiteConfig {
  apiRoot: string;
}

@Injectable({
  providedIn: 'root'
})
export class ConfigService {

  constructor() { }

  public static getConfig(): SiteConfig {
    return {
      apiRoot: 'http://localhost:5183'
    }
  }
}


//http://localhost:5251/Organization

