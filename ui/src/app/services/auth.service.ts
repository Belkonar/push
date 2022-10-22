import { Injectable } from '@angular/core';
import createAuth0Client, { Auth0Client } from '@auth0/auth0-spa-js';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private authClient = createAuth0Client({
    domain: 'dev-sl9gv5xa.us.auth0.com',
    client_id: '08m7ezAU3Y16MQCFX7dLmMdr5F8bi3HD',
    audience: 'uri:deployer',
    redirect_uri: `${window.location.origin}/callback`
  })

  constructor() { }

  async IsLoggedIn(): Promise<boolean> {
    let client = await this.authClient;

    return await client.isAuthenticated();
  }

  async HandleCallback(): Promise<void> {
    let client = await this.authClient;

    await client.handleRedirectCallback();
  }

  async doRedirect(): Promise<void> {
    let client = await this.authClient;

    await client.loginWithRedirect();
  }
}
