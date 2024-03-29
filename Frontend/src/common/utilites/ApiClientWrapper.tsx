import { jsonProperty, Serializable } from "ts-serializable";
import 'reflect-metadata'       // It's nessesary for "jsonProperty" function
import { getBaseUrl } from './Common'
import TokenService from '../utilites/TokenService';
import { IUser, setUser } from '../../common/redux/UserSlice';
import { AxiosError } from 'axios';
import { BadRequest } from'./exceptions';
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
    ApiException,
    IEditGoodsListDto
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

const BAD_REQUEST: number = 400;
const UNAUTHORIZED: number = 401;
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
                if (this.isBadRequest(error)){
                    throw new BadRequest("Не удалось получить список пользователей: некорректные данные.")
                }

                if (!this.isUnautorize(error)){
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
                if (this.isBadRequest(error)){
                    throw new BadRequest("Не удалось нового пользователя: некорректные данные.")
                }

                if (!this.isUnautorize(error)){
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
            if (this.isBadRequest(error)){
                throw new BadRequest("Не удалось закрыть чужую смену: некорректные данные.")
            }

            if (!this.isUnautorize(error)){
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
            if (this.isBadRequest(error)){
                throw new BadRequest("Не удалось открыть новую смену: некорректные данные.")
            }

            if (!this.isUnautorize(error)){
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
            if (this.isBadRequest(error)){
                throw new BadRequest("Не удалось удалить продавца: некорректные данные.")
            }

            if (!this.isUnautorize(error)){
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
            if (this.isBadRequest(error)){
                throw new BadRequest("Не удалось отправить список проданных товаров: некорректные данные.")
            }

            if (!this.isUnautorize(error)){
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
            if (this.isBadRequest(error)){
                throw new BadRequest("Не удалось получить список дней, когда работали пользователи: некорректные данные.")
            }

            if (!this.isUnautorize(error)){
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
            if (this.isBadRequest(error)){
                throw new BadRequest(`Не удалось получить статистику на ${day}: некорректные данные.`)
            }

            if (!this.isUnautorize(error)){
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

    public getFullStatistics(day: Date): Promise<ShiftSnapshotDto[]>{
        let locDate = new Date(day.getFullYear(), day.getMonth(), day.getDate(), 4);
        return this._statistics.full(-1, locDate, this.getToken()).then(response => {
            return this.updateUser().then(() => response);
        }).catch(error => {
            if (this.isBadRequest(error)){
                throw new BadRequest(`Не удалось получить статистику на ${day}: некорректные данные.`);
            }

            if (!this.isUnautorize(error)){
                console.error(error);
                this._tokenService.reset();
                return [];
            }

            return this.updateToken()
                .then(() => this._statistics.full(-1, locDate, this.getToken()))
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
            if (this.isBadRequest(error)){
                throw new BadRequest("Не удалось изменить пароль: некорректные данные.");
            }

            if (!this.isUnautorize(error)){
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
            if (this.isBadRequest(error)){
                throw new BadRequest("Не удалось закрыть рабочую смену: некорректные данные.")
            }

            if (!this.isUnautorize(error)){
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

    public addNewGoodsItem(user_id: number, itemName: string, category: string, storage: number, w_price: number, r_price: number): Promise<void>{
        let data: IEditGoodsListDto = {
            user_id: user_id,
            new: true,
            remove: false,
            restore: false,
            name: itemName,
            category: category,
            store: storage,
            r_price: r_price,
            w_price: w_price
        }

        const dto = new EditGoodsListDto(data);
        return this.editGoodsList(dto).catch(error => {
            if (error instanceof BadRequest){
                throw new BadRequest("Не удалось добавить товар: некорректные данные.");
            }

            throw error;
        });
    }
    
    public removeGoodsItem(user_id: number, itemId: string): Promise<void>{
        let data: IEditGoodsListDto = {
            user_id: user_id,
            id: itemId,
            new: false,
            remove: true,
            restore: false,
            name: '',
            category: '',
            store: 0,
            r_price: 0,
            w_price: 0
        }

        const dto = new EditGoodsListDto(data);
        return this.editGoodsList(dto).catch(error => {
            if (error instanceof BadRequest){
                throw new BadRequest("Не удалось удалить товар: некорректные данные.");
            }

            throw error;
        });
    }
    
    public restoreGoodsItem(user_id: number, itemId: string): Promise<void>{
        let data: IEditGoodsListDto = {
            user_id: user_id,
            id: itemId,
            new: false,
            remove: false,
            restore: true,
            name: '',
            category: '',
            store: 0,
            r_price: 0,
            w_price: 0
        }

        const dto = new EditGoodsListDto(data);
        return this.editGoodsList(dto).catch(error => {
            if (error instanceof BadRequest){
                throw new BadRequest("Не удалось восстановить товар: некорректные данные.");
            }

            throw error;
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
            if (this.isBadRequest(error)){
                throw new BadRequest("Не удалось отправить сведения по результатами ревизии: некорректные данные.")
            }

            if (!this.isUnautorize(error)){
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
            if (this.isBadRequest(error)){
                throw new BadRequest("Не удалось обновить сведения по поставкам: некорректные данные.")
            }

            if (!this.isUnautorize(error)){
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
                if (this.isBadRequest(error)){
                    throw new BadRequest("Не удалось обновить сведения о пользователе: некорректные данные.")
                }

                if (!this.isUnautorize(error)){
                    console.error(error);
                    this._tokenService.reset();
                    return;
                }

                return this.updateToken()
                    .then(() => this._update.user(this.getToken())
                        .then(response => {
                            let info: IUser = {
                                id: response.id,
                                is_admin: response.is_admin,
                                shift_opened: response.shift_opened,
                                name: response.name,
                                logon: true,
                                error: ""
                            }

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

    private editGoodsList(dto: EditGoodsListDto): Promise<void>{
        return this._base.edit(this.getToken(), dto).then(() => {
            return this.updateUser().then(() => {});
        }).catch(error => {
            if (this.isBadRequest(error)){
                throw new BadRequest('');
            }

            if (!this.isUnautorize(error)){
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

    private isBadRequest(exception: any): boolean{
        let apiError  = exception as ApiException;
        if (apiError !== null && apiError.status === BAD_REQUEST){
            return true;
        }

        let axiosError = exception as AxiosError;
        if (axiosError !== null && axiosError.status === BAD_REQUEST){
            return true;
        }

        if (!exception.hasOwnProperty('error')){
            return false;
        }

        return exception.error === 'resourceExists' || exception.error === 'resourceNotFound' || exception.error === 'invalidDto';
    }

    isUnautorize(exception: any): boolean{
        let apiError  = exception as ApiException;
        if (apiError !== null && apiError.status === UNAUTHORIZED){
            return true;
        }

        let axiosError = exception as AxiosError;
        return axiosError !== null && (axiosError.code === ERROR_CODE || axiosError.status === UNAUTHORIZED);
    }

    private getToken(): string{
        return "Bearer ".concat(this._tokenService.getCurrentToken());
    }
}

export default ApiClientWrapper;