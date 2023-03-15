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
    TokenDto,
    EditGoodsListDto,
    GoodsItemDto,
    SignInDto,
    GoodsSuppliesDto,
    AddUserDto,
    NewUserDto,
    SoldGoodsDto,
    ReducedSnapshotDto,
    ShiftSnapshotDto
} from './SwaggerClient';

interface IUserInfo{
    id: number;
    is_admin: boolean;
    shift_opened: boolean;
    name: string;
    token: string;
    expired: number;
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
    
    @jsonProperty(String)
    token!: string;

    @jsonProperty(Number)
    expired!: number;
}

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
    };

    public addNewUser(dto: AddUserDto): Promise<NewUserDto>{
        return this._add.user(this.getToken(), dto);
    }

    public closeShiftForOtherUser(targetUserId: number, cash: number) : Promise<void>{
        return this._close.other(targetUserId, cash);
    }

    public initWorkingShift(id: number): Promise<void>{
        return this._init.shift(id, this.getToken());
    }

    public removeUser(id: number): Promise<void>{
        return this._remove.user(id, this.getToken());
    }

    public soldGoods(dto: SoldGoodsDto): Promise<void>{
        return this._sold.goods(this.getToken(), dto);
    }

    public getShiftDays(id: number): Promise<Date[]>{
        return this._shift.days(id, this.getToken());
    }

    public getStatistics(id: number, day: Date): Promise<ReducedSnapshotDto[]>{
        return this._sold.statistics(id, day, this.getToken());
    }

    public getFullStatistics(id: number, day: Date): Promise<ShiftSnapshotDto[]>{
        return this._statistics.full(id, day, this.getToken());
    }

    public async updateToken(): Promise<UserInfo>{
        let response = await this._update.token(this.getToken());
        let info = new UserInfo();
        info.id = response.id;
        info.name = response.name;
        info.token = response.token.token;
        info.expired = Date.parse(response.token.expired.toUTCString());
        info.is_admin = response.is_admin;
        info.shift_opened = response.shift_opened;

        return info;
    }

    public changePassword(oldPassword: string, password: string): Promise<TokenDto>{
        return this._base.change(oldPassword, password, this.getToken());
    }

    public closeWorkingShift(userId: number, cash: number): Promise<void>{
        return this._base.close(userId, cash, this.getToken());
    }

    public editGoodsList(dto: EditGoodsListDto): Promise<void>{
        return this._base.edit(this.getToken(), dto);
    }

    public getAllGoods(): Promise<GoodsItemDto[]>{
        return this._base.goods();
    }

    public revision(dto: GoodsRevisionDto): Promise<void>{
        return this._base.revision(this.getToken(), dto);
    }

    public async signin(login: string, password: string): Promise<UserInfo>{
        let dto = new SignInDto()
        dto.login = login;
        dto.password = password;
        let response = await this._base.signin("", dto);
        let info = new UserInfo();
        info.id = response.id;
        info.name = response.name;
        info.token = response.token.token;
        info.expired = Date.parse(response.token.expired.toUTCString());
        info.is_admin = response.is_admin;
        info.shift_opened = response.shift_opened;

        return info;
    }

    public signout(id: number): Promise<TokenDto>{
        return this._base.signout(id, this.getToken());
    }

    public updateSupplySate(dto: GoodsSuppliesDto): Promise<void>{
        return this._base.supplies(this.getToken(), dto);
    }

    private getToken(): string{
        return "Bearer ".concat(TokenService.getCurrentToken());
    }
}

export default ApiClientWrapper;