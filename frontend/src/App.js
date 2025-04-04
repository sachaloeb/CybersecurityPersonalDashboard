import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Sidebar from './components/Sidebar';
import Topbar from './components/Topbar';
import ConnectionsTable from './pages/ConnectionsTable';
import SecurityOverview from './pages/SecurityOverview';
import PasswordChecker from "./pages/PasswordChecker";
import './App.css';
import PortScannerForm from "./pages/PortScannerForm";

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
                <Route path="/password-checker" element={<PasswordChecker />} />
              <Route path={"/port-scanner"} element={<PortScannerForm />} />
              {/* Add more routes as needed */}
            </Routes>
          </div>
        </div>
      </div>
    </Router>
  );
}

export default App;