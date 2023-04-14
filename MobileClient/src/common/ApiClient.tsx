import { NativeModules } from 'react-native';
import { getBaseUrl, getCertificatePath, getSecretKeyPath } from './Utilites';
import { UserInfoDto } from './types/UserInfoDto';

class ApiClient{
    private readonly _client: any;

    constructor(){
        this._client = NativeModules["ApiClientModule"];
        this._client.init(getBaseUrl(), getCertificatePath(), getSecretKeyPath());
    }

    // Siging in user
    public signin(login: string, password: string): Promise<boolean> {
        return this._client.signin(login, password).then((response: string) => {
            let result = UserInfoDto.fromJS(response);
            console.log(result);
            return true;
        }).catch((error:any) => {
            console.error(error);
            return false;
        });
    }

    // Logout user.
    public signout(id: number): Promise<void> {
        return this._client.signout(id)
            .then(() => true)
            .catch((error:any) => {
            console.error(error);
            return false;
        });
    }

    // Selling item with targer GUID.
    public sell_item(user_id: number, sold_item: string): Promise<void> {
        return this._client.sell(user_id, sold_item)
            .then(() => true)
            .catch((error:any) => {
            console.error(error);
            return false;
        });
    }
}

export default ApiClient;