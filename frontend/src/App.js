import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Sidebar from './components/Sidebar';
import Topbar from './components/Topbar';
import ConnectionsTable from './components/ConnectionsTable';
import SecurityOverview from './components/SecurityOverview';
import './App.css';

function App() {
  return (
    <Router>
      <div className="app-container">
        <Sidebar />
        <div className="main-content">
          <Topbar />
          <div className="content-wrapper">
            <Routes>
                <Route path="/" element={<SecurityOverview />} />
              <Route path="/security-overview" element={<SecurityOverview />} />
              <Route path="/connections-table" element={<ConnectionsTable />} />
              {/* Add more routes as needed */}
            </Routes>
          </div>
        </div>
      </div>
    </Router>
  );
}

export default App;