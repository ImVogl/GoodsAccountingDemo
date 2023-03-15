import { logout } from '../redux/UserSlice';
import { cleanStore } from '../redux/store';

const TOKEN_KEY:string = "token";

class TokenService{
    private readonly _dispatcher: any;

    public constructor(dispatcher?: any){
        if (dispatcher !== null){
            this._dispatcher = dispatcher;
        }
    }

    public set(token: string){
        window.localStorage.setItem(TOKEN_KEY, token);
    }

    public getCurrentToken(): string
    {
        let token = window.localStorage.getItem(TOKEN_KEY);
        return token !== null ? token : "";
    }

    public reset(){
        window.localStorage.removeItem(TOKEN_KEY);
        this._dispatcher(logout());
        cleanStore();
    }
}

export default TokenService;