import { UpdateClient } from './SwaggerClient';
import { getBaseUrl } from './Common';

const TOKEN_KEY:string = "token";

export class TokenUpdater{
    public constructor(client?: UpdateClient){
        if (client === null || client === undefined){
            this._client = new UpdateClient(getBaseUrl());
        }
        else{
            this._client = client;
        }

        this._tokenExpire = setTimeout(async() => {}, 0);
    }

    private readonly  timeout:number = 30000;
    _tokenExpire: ReturnType<typeof setTimeout>;
    _client: UpdateClient;


    public prepare(token: string){
        window.localStorage.setItem(TOKEN_KEY, token);
    }

    public async startTokenUpdater(){
        clearTimeout(this._tokenExpire);
        let tokenInStorage = window.localStorage.getItem(TOKEN_KEY);
        if (tokenInStorage == null){
            return;
        }

        let tokenResponse = await this._client.token();
        window.localStorage.setItem(TOKEN_KEY, tokenResponse[TOKEN_KEY]);
        this._tokenExpire = setTimeout(async() => await this.startTokenUpdater(), this.timeout);
    }

    public reset(){
        window.localStorage.removeItem(TOKEN_KEY);
    }
}

export function getCurrentToken(): string
{
    let token = window.localStorage.getItem(TOKEN_KEY);
    return token !== null ? token : "";
}