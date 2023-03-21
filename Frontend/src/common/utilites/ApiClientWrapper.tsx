import { jsonProperty, Serializable } from "ts-serializable";
import 'reflect-metadata'       // It's nessesary for "jsonProperty" function
import { getBaseUrl } from './Common'
import TokenService from '../utilites/TokenService';
import { IUser, setUser } from '../../common/redux/UserSlice';
import { AxiosError } from 'axios';
import 
{ 
    Client,
    StatisticsClient,
    CloseClient,
    InitClient,
    SoldClient,
    UpdateClient,
    AddClient,
    RemoveClient,
    ShiftClient,
    AllClient,
    GoodsRevisionDto,
    EditGoodsListDto,
    GoodsItemDto,
    SignInDto,
    GoodsSuppliesDto,
    AddUserDto,
    NewUserDto,
    SoldGoodsDto,
    ReducedSnapshotDto,
    ShiftSnapshotDto,
    UserLoginDto,
    ApiException
} from './SwaggerClient';

interface IUserInfo{
    id: number;
    is_admin: boolean;
    shift_opened: boolean;
    name: string;
}

export class UserInfo extends Serializable implements IUserInfo{
    @jsonProperty(Number)
    id!: number;

    @jsonProperty(Boolean)
    is_admin!: boolean;
    
    @jsonProperty(Boolean)
    shift_opened!: boolean;
    
    @jsonProperty(String)
    name!: string;
}

const Unauthorized: number = 401;
const ERROR_CODE: string = "ERR_NETWORK";

class ApiClientWrapper{
    private readonly _all: AllClient;
    private readonly _add: AddClient;
    private readonly _base: Client;
    private readonly _close: CloseClient;
    private readonly _init: InitClient;
    private readonly _remove: RemoveClient;
    private readonly _shift: ShiftClient;
    private readonly _sold: SoldClient;
    private readonly _statistics: StatisticsClient;
    private readonly _update: UpdateClient;
    private readonly _tokenService: TokenService;
    private readonly _dispatcher: any;

    constructor(dispatcher?: any){
        this._all = new AllClient(getBaseUrl());
        this._add = new AddClient(getBaseUrl());
        this._base = new Client(getBaseUrl());
        this._close = new CloseClient(getBaseUrl());
        this._init = new InitClient(getBaseUrl());
        this._remove = new RemoveClient(getBaseUrl());
        this._shift = new ShiftClient(getBaseUrl());
        this._sold = new SoldClient(getBaseUrl());
        this._statistics = new StatisticsClient(getBaseUrl());
        this._update = new UpdateClient(getBaseUrl());
        this._tokenService = new TokenService(dispatcher);
        this._dispatcher = dispatcher;
    };

    public getAllUsers(): Promise<UserLoginDto[]>{
        return this._all.users(this.getToken())
            .catch(error => {
                let apiError  = error as ApiException;
                let axiosError = error as AxiosError;
                if ((apiError === null || apiError.status !== Unauthorized) && (axiosError === null || axiosError.code !== ERROR_CODE)){
                    console.error(error);
                    this._tokenService.reset();
                    return [];
                }

                return this.updateToken()
                    .then(() => this._all.users(this.getToken()))
                    .catch(error => {
                        console.error(error);
                        this._tokenService.reset();
                        return [];
                    });
            })
            .then(response => response);
    }

    public addNewUser(id: number, name: string, surname: string, date: Date): Promise<NewUserDto>{
        let dto = new AddUserDto({ id: id, name: name, surname: surname, date: date });
        return this._add.user(this.getToken(), dto)
            .then(response => this.updateUser().then(() => response))
            .catch(error => {
                let apiError  = error as ApiException;

                if (apiError === null || apiError.status !== Unauthorized){
                    console.error(error);
                    this._tokenService.reset();
                    return new NewUserDto();
                }

                return this.updateToken()
                    .then(() => this._add.user(this.getToken(), dto))
                    .catch(error => {
                        console.error(error);
                        this._tokenService.reset();
                        return new NewUserDto();
                    });
            });
    }

    public closeShiftForOtherUser(targetUserId: number, cash: number) : Promise<void>{
        return this._close.other(targetUserId, cash).then(response => {
            return this.updateUser().then(() => response);
        }).catch(error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            return this.updateToken()
                .then(() => this._close.other(targetUserId, cash))
                .catch(error => {
                    console.error(error);
                    this._tokenService.reset();
                });
        });
    }

    public initWorkingShift(id: number): Promise<void>{
        return this._init.shift(id, this.getToken()).then(response => {
            return this.updateUser().then(() => response);
        }).catch(error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            return this.updateToken()
                .then(() => this._init.shift(id, this.getToken()))
                .catch(error => {
                    console.error(error);
                    this._tokenService.reset();
                });
        });
    }

    public removeUser(id: number): Promise<void>{
        return this._remove.user(id, this.getToken()).then(response => {
            return this.updateUser().then(() => response);
        }).catch(error => {
            let apiError  = error as ApiException;
            let axiosError = error as AxiosError;
            if ((apiError === null || apiError.status !== Unauthorized) && (axiosError === null || axiosError.code !== ERROR_CODE)){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            return this.updateToken()
                .then(() => this._remove.user(id, this.getToken()))
                .catch(error => {
                    console.error(error);
                    this._tokenService.reset();
                });
        });
    }

    public soldGoods(dto: SoldGoodsDto): Promise<void>{
        return this._sold.goods(this.getToken(), dto).then(response => {
            return this.updateUser().then(() => response);
        }).catch(error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            return this.updateToken()
                .then(() => this._sold.goods(this.getToken(), dto))
                .catch(error => {
                    console.error(error);
                    this._tokenService.reset();
                });
        });
    }

    public getShiftDays(id: number): Promise<Date[]>{
        return this._shift.days(id, this.getToken()).then(response => {
            return this.updateUser().then(() => response);
        }).catch(error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return [];
            }

            return this.updateToken()
                .then(() => this._shift.days(id, this.getToken()))
                .catch(error => {
                    console.error(error);
                    this._tokenService.reset();
                    return [];
                });
        });
    }

    public getStatistics(id: number, day: Date): Promise<ReducedSnapshotDto[]>{
        let locDate = new Date(day.getFullYear(), day.getMonth(), day.getDay());
        return this._sold.statistics(id, locDate, this.getToken()).then(response => {
            return this.updateUser().then(() => response);
        }).catch(error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return [];
            }

            return this.updateToken()
                .then(() => this._sold.statistics(id, day, this.getToken()))
                .catch(error => {
                    console.error(error);
                    this._tokenService.reset();
                    return [];
                });
        });
    }

    public getFullStatistics(id: number, day: Date): Promise<ShiftSnapshotDto[]>{
        let locDate = new Date(day.getFullYear(), day.getMonth(), day.getDate(), 4);
        return this._statistics.full(id, locDate, this.getToken()).then(response => {
            return this.updateUser().then(() => response);
        }).catch(error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return [];
            }

            return this.updateToken()
                .then(() => this._statistics.full(id, day, this.getToken()))
                .catch(error => {
                    console.error(error);
                    this._tokenService.reset();
                    return [];
                });
        });
    }

    public updateToken(): Promise<void>{
        return this._update.token()
            .then(token => {
                this._tokenService.set(token);
            })
            .catch(error => {
                console.error(error);
                this._tokenService.reset();
            });
    }

    public changePassword(oldPassword: string, password: string): Promise<string>{
        return this._base.change(oldPassword, password, this.getToken()).then(response => {
            return this.updateUser().then(() => response);
        }).catch(error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return "";
            }

            return this.updateToken()
                .then(() => this._base.change(oldPassword, password, this.getToken()))
                .catch(error => {
                    console.error(error);
                    this._tokenService.reset();
                    return "";
                });
        });
    }

    public closeWorkingShift(userId: number, cash: number): Promise<void>{
        return this._base.close(userId, cash, this.getToken()).then(() => {
            return this.updateUser().then(() => {});
        })
        .catch(error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            return this.updateToken()
                .then(() => this._base.close(userId, cash, this.getToken()))
                .catch(error => {
                    console.error(error);
                    this._tokenService.reset();
                });
        });
    }

    public editGoodsList(dto: EditGoodsListDto): Promise<void>{
        return this._base.edit(this.getToken(), dto).then(() => {
            return this.updateUser().then(() => {});
        }).catch(error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            return this.updateToken()
                .then(() => this._base.edit(this.getToken(), dto))
                .catch(error => {
                    console.error(error);
                    this._tokenService.reset();
                });
        });
    }

    public getAllGoods(): Promise<GoodsItemDto[]>{
        return this._base.goods().catch(error => {
            console.error(error);
            return [];
        });
    }

    public revision(dto: GoodsRevisionDto): Promise<void>{
        return this._base.revision(this.getToken(), dto).then(() => {
            return this.updateUser().then(() => {});
        }).catch(error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            return this.updateToken()
                .then(() => this._base.revision(this.getToken(), dto))
                .catch(error => {
                    console.error(error);
                    this._tokenService.reset();
                });
        });
    }

    public signin(login: string, password: string): Promise<UserInfo>{
        let dto = new SignInDto()
        dto.login = login;
        dto.password = password;
        return this._base.signin("", dto)
            .then((response) => {
                this._tokenService.set(response.token);
                let info = new UserInfo();
                info.id = response.id;
                info.name = response.name;
                info.is_admin = response.is_admin;
                info.shift_opened = response.shift_opened;
                return info;
            }).catch(error =>{
                console.error(error);
                this._tokenService.reset();
                return new UserInfo();
            });
    }

    public signout(id: number): Promise<void>{
        return this._base.signout(id).then(() =>{
            this._tokenService.reset();
        }).catch(error => {
            console.error(error);
            this._tokenService.reset();
        });
    }

    public updateSupplySate(dto: GoodsSuppliesDto): Promise<void>{
        return this._base.supplies(this.getToken(), dto).then(() => {
            return this.updateUser().then(() => {});
        }).catch(error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            return this.updateToken()
                .then(() => this._base.supplies(this.getToken(), dto))
                .catch(error => {
                    console.error(error);
                    this._tokenService.reset();
                });
        });
    }

    public updateUser(): Promise<void>{
        return this._update.user(this.getToken())
            .then(response => {
                let info: IUser = { id: response.id, is_admin: response.is_admin, shift_opened: response.shift_opened, name: response.name, logon: true, error: "" }
                this._dispatcher(setUser(info));
            })
            .catch(error => {
                let apiError  = error as ApiException;
                if (apiError === null || apiError.status !== Unauthorized){
                    console.error(error);
                    this._tokenService.reset();
                    return;
                }

                return this.updateToken()
                    .then(() => this._update.user(this.getToken())
                        .then(response => {
                            let info: IUser = { id: response.id, is_admin: response.is_admin, shift_opened: response.shift_opened, name: response.name, logon: true, error: "" }
                            this._dispatcher(setUser(info));
                        })
                        .catch(error => {
                            console.error(error);
                            this._tokenService.reset();
                        }))
                    .catch(error => {
                        console.error(error);
                        this._tokenService.reset();
                    });
        });
    }

    private getToken(): string{
        return "Bearer ".concat(this._tokenService.getCurrentToken());
    }
}

export default ApiClientWrapper;