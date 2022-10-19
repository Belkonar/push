import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

// TODO: Move to services folder

@Injectable({
  providedIn: 'root'
})
export class MonacoService {
  isLoaded = false;
  loaded: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor() { }

  public load() {
    if (this.isLoaded) return;

    // load the assets
    const baseUrl = './assets' + '/monaco-editor/min/vs';

    if (typeof (<any>window).monaco === 'object') {
      this.isLoaded = true;
      this.loaded.next(true);
      return;
    }

    const onGotAmdLoader: any = () => {
      // load Monaco
      (<any>window).require.config({ paths: { vs: `${baseUrl}` } });
      (<any>window).require([`vs/editor/editor.main`], () => {
        this.isLoaded = true;
        this.loaded.next(true);
      });
    };

    // load AMD loader, if necessary
    if (!(<any>window).require) {
      const loaderScript: HTMLScriptElement = document.createElement('script');
      loaderScript.type = 'text/javascript';
      loaderScript.src = `${baseUrl}/loader.js`;
      loaderScript.addEventListener('load', onGotAmdLoader);
      document.body.appendChild(loaderScript);
    } else {
      onGotAmdLoader();
    }
  }
}
