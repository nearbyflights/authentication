## authentication

Application and configuration for the login system.

This repo contains:

- [ORY Hydra](https://github.com/ory/hydra) configuration for building the login system.
- A consent application (`hydra-login-consent-aspnet`) that supports the login system where you can signup and login.
- A test utility for the consent application (`hydra-login-test-aspnet`).

### Development

#### Requirements

This assumes you have running on localhost in the default ports:

- ORY Hydra.

  - You should have a test client that uses `client_secret_post` as the [`token_endpoint_auth_methood`](https://tools.ietf.org/html/rfc7591#section-2). You can insert the client like this in the Hydra database:

  - ```bash
    $ docker-compose -f quickstart.yml exec hydra \
        hydra clients create \
        --endpoint http://127.0.0.1:4445 \
        --id test-client \
        --secret secret \
        --grant-types authorization_code,refresh_token \
        --response-types code,id_token \
        --scope openid,offline \
        --callbacks http://localhost:15000/callback
        --token-endpoint-auth-method client_secret_post
    ```

- Redis for storing the users.

#### Running

Run both `hydra-login-consent-aspnet` and `hydra-login-test-aspnet` from Visual Studio and go to http://localhost:15001/Home/Secure in the test application to be redirected to the consent application (http://localhost:15000/). In the consent application you will be able to signup or login and then be redirected back to the test application to see your token.

