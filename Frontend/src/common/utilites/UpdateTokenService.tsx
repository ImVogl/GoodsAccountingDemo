import { UpdateClient } from './SwaggerClient';
import { getBaseUrl } from './Common';
import { updateToken } from '../redux/UserSlice'

const TOKEN_KEY:string = "token";

export class TokenUpdater{
    private readonly timeout:number = 30000;
    private readonly _client: UpdateClient;
    private readonly _dispatcher: any;

    private _tokenExpire: ReturnType<typeof setTimeout>;

    public constructor(dispatcher?: any, client?: UpdateClient){
        if (client === null || client === undefined){
            this._client = new UpdateClient(getBaseUrl());
        }
        else{
            this._client = client;
        }

        this._tokenExpire = setTimeout(async() => {}, 0);
        if (dispatcher !== null){
            this._dispatcher = dispatcher;
        }
    }


    public prepare(token: string){
        window.localStorage.setItem(TOKEN_KEY, token);
    }

    public async startTokenUpdater(){
        clearTimeout(this._tokenExpire);
        let tokenInStorage = window.localStorage.getItem(TOKEN_KEY);
        if (tokenInStorage == null){
            return;
        }

        let tokenResponse = await this._client.token(tokenInStorage);
        console.log(tokenResponse);
        if (this._dispatcher !== null && this._dispatcher !== undefined){
            this._dispatcher(updateToken(tokenResponse[TOKEN_KEY]));
        }

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