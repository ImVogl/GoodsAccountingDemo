import { configureStore } from '@reduxjs/toolkit';
import titleReducer from './TitleSlice';
import userReduser from './UserSlice';

export const store = configureStore({
  reducer: {
    manager: titleReducer,
    controler: userReduser
  }
});

export type Dispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;