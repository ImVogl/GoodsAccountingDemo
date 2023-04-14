export interface IUserInfoDto {
    id: number;
    is_admin: boolean;
    shift_opened: boolean;
    name: string;
    token: string;
}

export class UserInfoDto implements IUserInfoDto {
    id!: number;
    is_admin!: boolean;
    shift_opened!: boolean;
    name!: string;
    token!: string;

    constructor(data?: IUserInfoDto) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.id = _data["id"];
            this.is_admin = _data["is_admin"];
            this.shift_opened = _data["shift_opened"];
            this.name = _data["name"];
            this.token = _data["token"];
        }
    }

    static fromJS(data: any): UserInfoDto {
        data = typeof data === 'object' ? data : {};
        let result = new UserInfoDto();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["id"] = this.id;
        data["is_admin"] = this.is_admin;
        data["shift_opened"] = this.shift_opened;
        data["name"] = this.name;
        data["token"] = this.token;
        return data; 
    }
}
