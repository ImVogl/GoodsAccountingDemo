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
}

const initialState: IUser = {
    id: 0,
    is_admin: false,
    shift_opened: false,
    name: '',
    logon: false
};

export const signInAsync = createAsyncThunk(
    'controler/signin',
    async (dto: SignInDto) => {
        let client = new Client(getBaseUrl())
        let response = await client.signin(dto);
        debugger;
        return response;
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
          })
          .addCase(signInAsync.rejected, (state) => {
            state.logon = false;
          });
      }
  });
  
export const { setUser } = userSlice.actions;
export const selectUserLogon = (state: RootState) => state.controler.logon;
export default userSlice.reducer;
