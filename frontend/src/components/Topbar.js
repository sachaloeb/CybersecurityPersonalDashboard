import React from 'react';
import './Topbar.css';

function Topbar() {
  return (
    <div className="topbar">
      <h2>Personal Cybersecurity Dashboard</h2>
      {/* Right-aligned icons or profile */}
      <div className="topbar-actions">
        {/* Example placeholders: notifications, user avatar, etc. */}
        <span className="icon-bell">🔔</span>
        <span className="avatar">JD</span>
      </div>
    </div>
  );
}

export default Topbar;