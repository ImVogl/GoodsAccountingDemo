import './App.css';
import { FC } from 'react';
import { BrowserRouter, Route, Routes } from 'react-router-dom'

import { useAppSelector } from '../../common/redux/hooks';
import { selectUserLogon } from '../../common/redux/UserSlice';
import SellPage from '../sell/SellPage';
import HomePage from '../home/HomePage';
import ForbiddenPage from '../forbidden/ForbiddenPage';
import NavigationBar from '../navigation/NavigationBar';
import { SELLS, INV, ACCOUNT } from '../../common/utilites/Paths';

const App: FC = () => {
  const logon = useAppSelector(selectUserLogon);
  let sellsComponent = logon ? <SellPage /> : <ForbiddenPage />;
  let inventarisation = <ForbiddenPage />;
  let account = <ForbiddenPage />;
  return (
    <div className='root'>
      <header><NavigationBar /></header>
      <BrowserRouter>
        <Routes>
            <Route index element={<HomePage />}></Route>
            <Route path={SELLS} element={sellsComponent} ></Route>
            <Route path={INV} element={inventarisation} ></Route>
            <Route path={ACCOUNT} element={account} ></Route>
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
