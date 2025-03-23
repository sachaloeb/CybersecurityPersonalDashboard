import React from 'react';
import './App.css';

import Sidebar from './components/Sidebar';
import Topbar from './components/Topbar';
import ConnectionsTable from './components/ConnectionsTable';

function App() {
  return (
    <div className="app-container">
      <Sidebar />
      <div className="main-content">
        <Topbar />
        <div className="content-wrapper">
          <h2>Network Connections</h2>
          <ConnectionsTable />
        </div>
      </div>
    </div>
  );
}

export default App;
