import { jsonProperty, Serializable } from "ts-serializable";
import 'reflect-metadata'       // It's nessesary for "jsonProperty" function
import { getBaseUrl } from './Common'
import TokenService from '../utilites/TokenService';
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

class ApiClientWrapper{
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

    constructor(){
        this._add = new AddClient(getBaseUrl());
        this._base = new Client(getBaseUrl());
        this._close = new CloseClient(getBaseUrl());
        this._init = new InitClient(getBaseUrl());
        this._remove = new RemoveClient(getBaseUrl());
        this._shift = new ShiftClient(getBaseUrl());
        this._sold = new SoldClient(getBaseUrl());
        this._statistics = new StatisticsClient(getBaseUrl());
        this._update = new UpdateClient(getBaseUrl());
        this._tokenService = new TokenService();
    };

    public addNewUser(dto: AddUserDto): Promise<NewUserDto>{
        return this._add.user(this.getToken(), dto).catch(async error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return new NewUserDto();
            }

            await this.updateToken().catch(error => {
                console.error(error);
                this._tokenService.reset();
            });

            return await this._add.user(this.getToken(), dto);
        });
    }

    public closeShiftForOtherUser(targetUserId: number, cash: number) : Promise<void>{
        return this._close.other(targetUserId, cash).catch(async error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            await this.updateToken().catch(error => {
                console.error(error);
                this._tokenService.reset();
            });

            return await this._close.other(targetUserId, cash);
        });
    }

    public initWorkingShift(id: number): Promise<void>{
        return this._init.shift(id, this.getToken()).catch(async error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            await this.updateToken().catch(error => {
                console.error(error);
                this._tokenService.reset();
            });

            return await this._init.shift(id, this.getToken());
        });
    }

    public removeUser(id: number): Promise<void>{
        return this._remove.user(id, this.getToken()).catch(async error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            await this.updateToken().catch(error => {
                console.error(error);
                this._tokenService.reset();
            });

            return await this._remove.user(id, this.getToken());
        });
    }

    public soldGoods(dto: SoldGoodsDto): Promise<void>{
        return this._sold.goods(this.getToken(), dto).catch(async error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            await this.updateToken().catch(error => {
                console.error(error);
                this._tokenService.reset();
            });

            return await this._sold.goods(this.getToken(), dto);
        });
    }

    public getShiftDays(id: number): Promise<Date[]>{
        return this._shift.days(id, this.getToken()).catch(async error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return [];
            }

            await this.updateToken().catch(error => {
                console.error(error);
                this._tokenService.reset();
            });

            return await this._shift.days(id, this.getToken());
        });
    }

    public getStatistics(id: number, day: Date): Promise<ReducedSnapshotDto[]>{
        return this._sold.statistics(id, day, this.getToken()).catch(async error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return [];
            }

            await this.updateToken().catch(error => {
                console.error(error);
                this._tokenService.reset();
            });

            return await this._sold.statistics(id, day, this.getToken());
        });
    }

    public getFullStatistics(id: number, day: Date): Promise<ShiftSnapshotDto[]>{
        return this._statistics.full(id, day, this.getToken()).catch(async error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return [];
            }

            await this.updateToken().catch(error => {
                console.error(error);
                this._tokenService.reset();
            });

            return await this._statistics.full(id, day, this.getToken());
        });
    }

    public async updateToken(): Promise<UserInfo>{
        let response = await this._update.token();
        let info = new UserInfo();
        info.id = response.id;
        info.name = response.name;
        info.is_admin = response.is_admin;
        info.shift_opened = response.shift_opened;

        this._tokenService.set(response.token);
        return info;
    }

    public changePassword(oldPassword: string, password: string): Promise<string>{
        return this._base.change(oldPassword, password, this.getToken()).catch(async error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return "";
            }

            await this.updateToken().catch(error => {
                console.error(error);
                this._tokenService.reset();
            });

            return await this._base.change(oldPassword, password, this.getToken());
        });
    }

    public closeWorkingShift(userId: number, cash: number): Promise<void>{
        return this._base.close(userId, cash, this.getToken()).catch(async error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            await this.updateToken().catch(error => {
                console.error(error);
                this._tokenService.reset();
            });

            return this._base.close(userId, cash, this.getToken());
        });
    }

    public editGoodsList(dto: EditGoodsListDto): Promise<void>{
        return this._base.edit(this.getToken(), dto).catch(async error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            await this.updateToken().catch(error => {
                console.error(error);
                this._tokenService.reset();
            });

            return this._base.edit(this.getToken(), dto);
        });
    }

    public getAllGoods(): Promise<GoodsItemDto[]>{
        return this._base.goods().catch(async error => {
            console.error(error);
            return [];
        });
    }

    public revision(dto: GoodsRevisionDto): Promise<void>{
        return this._base.revision(this.getToken(), dto).catch(async error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            await this.updateToken().catch(error => {
                console.error(error);
                this._tokenService.reset();
            });

            return this._base.revision(this.getToken(), dto);
        });
    }

    public async signin(login: string, password: string): Promise<UserInfo>{
        let dto = new SignInDto()
        dto.login = login;
        dto.password = password;
        return await this._base.signin("", dto).then((response) => {
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
        return this._base.supplies(this.getToken(), dto).catch(async error => {
            let apiError  = error as ApiException;
            if (apiError === null || apiError.status !== Unauthorized){
                console.error(error);
                this._tokenService.reset();
                return;
            }

            await this.updateToken().catch(error => {
                console.error(error);
                this._tokenService.reset();
            });

            return this._base.supplies(this.getToken(), dto);
        });
    }

    private getToken(): string{
        return "Bearer ".concat(this._tokenService.getCurrentToken());
    }
}

export default ApiClientWrapper;