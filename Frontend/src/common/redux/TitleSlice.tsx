import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from './store'

export interface TiteState{
    title: string;
}

const initialState: TiteState = {
    title: "Кафе 'У мойки'",
  };
  
export const titleSlice = createSlice({
    name: 'manager',
    initialState,
    reducers: {
      setTitle: (state, action: PayloadAction<string>) => {
        state.title = action.payload;
      },
    }
  });
  
export const { setTitle } = titleSlice.actions;
export const selectTitle = (state: RootState) => state.manager.title;
export default titleSlice.reducer;
