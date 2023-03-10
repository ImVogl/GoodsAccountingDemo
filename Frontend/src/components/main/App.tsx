import './App.css';
import React from 'react';
import HomePage from '../home/HomePage';
import NavigationBar from '../navigation/NavigationBar';

function App() {
  return (
    <div className='root'>
    <header><NavigationBar /></header>
      <HomePage />
    </div>
  );
}

export default App;
