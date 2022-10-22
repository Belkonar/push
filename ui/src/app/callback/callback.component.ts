import { Component } from '@angular/core';

/**
 * So this component should never even be seen, it's just needed due to the way the
 * router is implemented. If you see the message below everything is broken and
 * sadness has come.
 */

@Component({
  template: '<span>Handling Callback</span>'
})
export class CallbackComponent { }
