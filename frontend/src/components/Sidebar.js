import React from 'react';
import './Sidebar.css'; // If you prefer separate CSS
import { Link } from 'react-router-dom';

function Sidebar() {
  return (
    <div className="sidebar">
      <div className="sidebar-logo">
        {/* Replace with your logo or text */}
        <h1>Menu</h1>
      </div>
      <ul className="sidebar-menu">
        <li><Link to="/security-overview">Security Overview</Link></li>
        <li><Link to="/connections-table">Connections Table</Link></li>
        <li><Link to="/password-checker">Password Checker</Link></li>
        <li><Link to="/port-scanner">Port Scanner</Link></li>
        <li><Link to="/zap-alert-table">ZAP Alert Table</Link></li>
        {/* Add more links as needed */}
      </ul>
    </div>
  );
}

export default Sidebar;