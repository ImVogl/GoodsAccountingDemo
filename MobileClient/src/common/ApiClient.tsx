import { NativeModules } from 'react-native';
import { getBaseUrl } from './Utilites';
import { UserInfoDto } from './types/UserInfoDto';

const SUCCESS: number = 200;
const UNAUTORIZE: number = 401;
const BAD_REQUEST: number = 400;
const TIMEOUT: number = 10000;

class ApiClient{
    private readonly _client: any;

    constructor(){
        this._client = NativeModules["ApiClientModule"];
        this._client.init(getBaseUrl(), "I:\\Visual Studio 2017\\DemostrationProjects\\GoodsAccountingDemo\\Certificate\\server.crt", "I:\\Visual Studio 2017\\DemostrationProjects\\GoodsAccountingDemo\\Certificate\\server.key");
    }

    // Siging in user
    public signin(login: string, password: string): Promise<boolean> {
        return this._client.signin(login, password).then((response: string) => {
            console.log(response);
            let result = UserInfoDto.fromJS(response);
            console.log(result);
            return true;
        }).catch(() => {
            console.error();
            return false;
        });
    }
}

export default ApiClient;