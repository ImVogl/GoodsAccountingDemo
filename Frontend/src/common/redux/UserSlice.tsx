import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from './store'
import { Client, SignInDto } from '../utilites/SwaggerClient'
import { getBaseUrl } from '../utilites/Common';

export interface IUser{
    id: number;
    is_admin: boolean;
    shift_opened: boolean;
    name: string;
    logon: boolean;
    error: string;
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
    async (dto: SignInDto, { rejectWithValue }) => {
      try {
        let client = new Client(getBaseUrl())
        let response = await client.signin(dto);
        return response;
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
    },
    extraReducers: (builder) => {
        builder
          .addCase(signInAsync.pending, () => {
          })
          .addCase(signInAsync.fulfilled, (state, action) => {
            state.id = action.payload.id;
            state.name = action.payload.name;
            state.is_admin = action.payload.is_admin;
            state.shift_opened = action.payload.shift_opened;
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
          });
      }
  });
  
export const { setUser } = userSlice.actions;
export const selectUserLogon = (state: RootState) => state.controler.logon;
export const selectUserError = (state: RootState) => state.controler.error;
export default userSlice.reducer;
