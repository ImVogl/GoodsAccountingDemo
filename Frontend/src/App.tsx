import React from 'react';
import HomePage from './components/home/HomePage'
import NavigationBar from './components/navigation/NavigationBar'

function App() {
  return (
    <div >
    <header><NavigationBar /></header>
      <HomePage />
    </div>
  );
}

export default App;
