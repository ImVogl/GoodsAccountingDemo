import { configureStore } from '@reduxjs/toolkit';
import titleReducer from './TitleSlice';
import userReduser from './UserSlice';

function saveToLocalStorage(state:RootState){
  try{
    window.localStorage.setItem('state', JSON.stringify(state));
  } catch (error) {
      console.error(error);
  }
}

function loadFromLocalStorage()
{
  try {
    const stateStr = window.localStorage.getItem('state');
    return stateStr ? JSON.parse(stateStr) : undefined;
  }
  catch (error) {
      console.error(error);
      return undefined;
  }
}

export const store = configureStore({
  reducer: {
    manager: titleReducer,
    controler: userReduser
  },
  preloadedState: loadFromLocalStorage()
});

export function cleanStore(){
  window.localStorage.removeItem('state');
}

store.subscribe(() => {
  saveToLocalStorage(store.getState());
});

export type Dispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;