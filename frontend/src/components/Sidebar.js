import React from 'react';
import './Sidebar.css'; // If you prefer separate CSS

function Sidebar() {
  return (
    <div className="sidebar">
      <div className="sidebar-logo">
        {/* Replace with your logo or text */}
        <h1>SecDash</h1>
      </div>
      <ul className="sidebar-menu">
        <li>Dashboard</li>
        <li>Connections</li>
        <li>Scans</li>
        <li>Settings</li>
      </ul>
    </div>
  );
}

export default Sidebar;