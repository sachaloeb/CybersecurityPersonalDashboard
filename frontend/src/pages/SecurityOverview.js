import React, { useEffect, useState } from 'react';
import axios from 'axios';
import "./SecurityOverview.css";

const SecurityOverview = () => {
  const [overview, setOverview] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchSecurityOverview = async () => {
      try {
        const response = await axios.get('https://localhost:7122/api/security-overview');
        console.log(response.data);
        setOverview(response.data);
      } catch (err) {
        setError(err);
      } finally {
        setLoading(false);
      }
    };

    fetchSecurityOverview();
  }, []);

  const getStatusClass = (status) => {
    if (typeof status !== 'string') return '';
    const normalized = status.toLowerCase();
    if (normalized.includes('enabled') || normalized.includes('active') || normalized.includes('on')) {
      return 'status-enabled';
    } else if (normalized.includes('disabled') || normalized.includes('inactive') || normalized.includes('off')) {
      return 'status-disabled';
    }
    return '';
  };
  
  if (loading) {
    return (
        <div className="security-overview-container">
          <div className="security-overview-card">
            <div className="security-overview-message">Loading...</div>
          </div>
        </div>
    );
  }

  if (error) {
    return (
        <div className="security-overview-container">
          <div className="security-overview-card">
            <div className="security-overview-message">Error: {error}</div>
          </div>
        </div>
    );
  }

  if (!overview || !overview.value) {
    return (
        <div className="security-overview-container">
          <div className="security-overview-card">
            <div className="security-overview-message">No data available</div>
          </div>
        </div>
    );
  }

  return (
      <div className="security-overview-container">
        <div className="security-overview-card">
          <h2 className="security-overview-header">Security Overview</h2>

          <div className="security-overview-item">
            <span className="security-overview-label">System Uptime (Hours):</span>
            <span className="security-overview-value">{overview.value.uptimeHours}</span>
          </div>

          <div className="security-overview-item">
            <span className="security-overview-label">Antivirus Status:</span>
            <span className={`security-overview-value ${getStatusClass(overview.value.antivirusStatus)}`}>
            {overview.value.antivirusStatus}
          </span>
          </div>

          <div className="security-overview-item">
            <span className="security-overview-label">Firewall Status:</span>
            <span className={`security-overview-value ${getStatusClass(overview.value.firewallStatus)}`}>
            {overview.value.firewallStatus}
          </span>
          </div>

          <div className="security-overview-item">
            <span className="security-overview-label">Logged in user:</span>
            <span className={`security-overview-value ${getStatusClass(overview.value.loggedInUsers)}`}>
            {overview.value.loggedInUsers}
          </span>
          </div>
        </div>
      </div>
  );
};

export default SecurityOverview;