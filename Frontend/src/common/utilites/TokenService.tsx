import { updateUserDataAsync, logout } from '../redux/UserSlice';
import { cleanStore } from '../redux/store';
import { sleep_ms } from './Common';

const TOKEN_KEY:string = "token";
const EXPIRED_KEY:string = "expored";

class TokenService{
    private readonly timeout:number = 30000;
    private readonly _dispatcher: any;

    public constructor(dispatcher?: any){
        if (dispatcher !== null){
            this._dispatcher = dispatcher;
        }
    }

    public set(token: string, expired: number ){
        window.localStorage.setItem(TOKEN_KEY, token);
        window.localStorage.setItem(EXPIRED_KEY, expired.toString());
    }

    public async update(){
        await sleep_ms(this.timeout);
        let tokenInStorage = window.localStorage.getItem(TOKEN_KEY);
        let tokenExpired = window.localStorage.getItem(EXPIRED_KEY);
        if (tokenInStorage == null || tokenExpired === null){
            return;
        }

        let expiredNumber = 0;
        try{
            expiredNumber = parseInt(tokenExpired);
        }
        catch{
            this.reset();
            return;
        }
        if (expiredNumber < Date.parse(new Date().toUTCString())){
            this.reset();
            return;
        }

        if (this._dispatcher !== null && this._dispatcher !== undefined){
            this._dispatcher(updateUserDataAsync(""));
        }
    }

    public static getCurrentToken(): string
    {
        let token = window.localStorage.getItem(TOKEN_KEY);
        return token !== null ? token : "";
    }

    public reset(){
        window.localStorage.removeItem(TOKEN_KEY);
        window.localStorage.removeItem(EXPIRED_KEY);
        this._dispatcher(logout());
        cleanStore();
    }
}

export default TokenService;