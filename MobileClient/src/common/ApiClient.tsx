import { NativeModules } from 'react-native';
import { getBaseUrl } from './Utilites';
import { UserInfoDto } from './types/UserInfoDto';

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
        }).catch((error:any) => {
            console.error(error);
            return false;
        });
    }
}

export default ApiClient;