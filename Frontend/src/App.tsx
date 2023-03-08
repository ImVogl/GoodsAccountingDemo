import React from 'react';
import LayoutBase from './components/layouts/base/BaseLayout'
import NavigationBar from './components/navigation/NavigationBar'

// import { Counter } from './features/counter/Counter';

function App() {
  return (
    <div >
    <header><NavigationBar /></header>
      <LayoutBase>
        <div>Hello, world</div>
      </LayoutBase>
    </div>
  );
}

export default App;
