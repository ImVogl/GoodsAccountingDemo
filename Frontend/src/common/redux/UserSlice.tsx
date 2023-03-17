import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from './store';
import ApiClientWrapper, { UserInfo } from '../utilites/ApiClientWrapper';

export interface IToken{
  token: string,
  expired: Date;
}

export interface IUser{
    id: number;
    is_admin: boolean;
    shift_opened: boolean;
    name: string;
    logon: boolean;
    error: string;
}

interface ISignIn{
  login: string;
  password: string;
}

export class SignIn implements ISignIn{
  login!: string;
  password!: string;
}

interface IErrorMessage{
  error:string;
}

const initialState: IUser = {
    id: 0,
    is_admin: false,
    shift_opened: false,
    name: '',
    logon: false,
    error: ''
};

export const signInAsync = createAsyncThunk(
    'controler/signin',
    async (dto: ISignIn, { rejectWithValue }) => {
      try {
        let client = new ApiClientWrapper();
        let response = await client.signin(dto.login, dto.password);
        return JSON.stringify(response);
      } catch (err) {
        if (!err) {
          throw err
      }
      
      return rejectWithValue(err)
    }
  }
);

export const updateUserDataAsync = createAsyncThunk(
  'controler/update',
  async (_:string, { rejectWithValue }) => {
    try {
      let client = new ApiClientWrapper()
      let response = await client.updateToken();
      return JSON.stringify(response);
    } catch (err) {
      if (!err) {
        throw err
    }
  
    return rejectWithValue(err)
  }
}
);

export const userSlice = createSlice({
    name: 'controler',
    initialState,
    reducers: {
      setUser: (state, action: PayloadAction<IUser>) => {
        state.id = action.payload.id;
        state.name = action.payload.name;
        state.is_admin = action.payload.is_admin;
        state.shift_opened = action.payload.shift_opened;
        state.logon = action.payload.logon;
      },
      logout:(state) =>{
        state.id = 0;
        state.name = "";
        state.is_admin = false;
        state.shift_opened = false;
        state.logon = false;
      }
    },
    extraReducers: (builder) => {
        builder
          .addCase(signInAsync.pending, () => {})
          .addCase(signInAsync.fulfilled, (state, action) => {
            let info = new UserInfo().fromJSON(JSON.parse(action.payload));
            state.id = info.id;
            state.name = info.name;
            state.is_admin = info.is_admin;
            state.shift_opened = info.shift_opened;
            state.logon = true;
            state.error = "";
          })
          .addCase(signInAsync.rejected, (state, action) => {
            state.logon = false;
            let errorMessage = action.payload as IErrorMessage
            if (errorMessage === null){
              state.error = "Не удалось авторизоваться, неизветсная ошибка."
              return;
            }
            if (errorMessage.error === "invalidDto"){
              state.error = "Не удалось авторизоваться, неверные данные."
            } else if (errorMessage.error === "unknownError"){
              state.error = "Не удалось авторизоваться, неизветсная ошибка."
            } else{
              state.error = "Не удалось авторизоваться, не получилось авторизоваться."
            }
          })
          .addCase(updateUserDataAsync.pending, () => {})
          .addCase(updateUserDataAsync.fulfilled, (state, action) => {
            let info = new UserInfo().fromJSON(JSON.parse(action.payload));
            state.id = info.id;
            state.is_admin = info.is_admin;
            state.shift_opened = info.shift_opened;
            state.name = info.name;
            state.logon = true;
            state.error = "";
          })
          .addCase(updateUserDataAsync.rejected, (state, action) => {
            let errorMessage = action.payload as IErrorMessage
            if (errorMessage === null){
              state.error = "Не удалось обновить сведения, неизветсная ошибка."
              return;
            }
            if (errorMessage.error === "invalidDto"){
              state.error = "Не удалось обновить сведения, неверные данные."
            } else if (errorMessage.error === "unknownError"){
              state.error = "Не удалось обновить сведения, неизветсная ошибка."
            } else{
              state.error = "Не удалось обновить сведения, не получилось авторизоваться."
            }
          });
      }
  });
  
export const { setUser, logout } = userSlice.actions;
export const selectUserLogon = (state: RootState) => state.controler.logon;
export const selectUserError = (state: RootState) => state.controler.error;
export const selectShiftUser = (state: RootState) => state.controler.shift_opened;
export const selectUserName = (state: RootState) => state.controler.name;
export const selectUserIdentifier = (state: RootState) => state.controler.id;
export const selectUserIsAdmin = (state: RootState) => state.controler.is_admin;
export default userSlice.reducer;
