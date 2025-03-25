import React, { useEffect, useState } from 'react';
import axios from 'axios';
import './SecurityOverview.css';

function SecurityOverview() {
  const [data, setData] = useState(null);
  const [error, setError] = useState(null);

  // Fetch security overview data on component mount
  useEffect(() => {
    const fetchSecurityOverview = async () => {
      try {
        const response = await axios.get('http://0.0.0.0:5050/api/security_overview');
        if (response.headers['content-type'].includes('application/json')) {
          setData(response.data);
        } else {
          throw new Error('Unexpected response format');
        }
      } catch (err) {
        console.error('Error fetching security overview:', err);
        setError(err.message);
      }
    };
    fetchSecurityOverview();
  }, []);

  // Helper function to apply color-coded classes based on status
  const getStatusClass = (status) => {
    if (!status) return '';
    const normalized = status.toLowerCase();
    if (normalized.includes('enabled') || normalized.includes('active') || normalized.includes('on')) {
      return 'status-enabled';
    } else if (normalized.includes('disabled') || normalized.includes('inactive') || normalized.includes('off')) {
      return 'status-disabled';
    }
    return '';
  };

  // Render error state
  if (error) {
    return (
      <div className="security-overview-container">
        <div className="security-overview-card">
          <div className="security-overview-message">Error: {error}</div>
        </div>
      </div>
    );
  }

  // Render loading state
  if (!data) {
    return (
      <div className="security-overview-container">
        <div className="security-overview-card">
          <div className="security-overview-message">Loading...</div>
        </div>
      </div>
    );
  }

  // Render the security overview card
  return (
    <div className="security-overview-container">
      <div className="security-overview-card">
        <h2 className="security-overview-header">Security Overview</h2>

        <div className="security-overview-item">
          <span className="security-overview-label">System Uptime (Hours):</span>
          <span className="security-overview-value">{data.uptimeHours}</span>
        </div>

        <div className="security-overview-item">
          <span className="security-overview-label">Antivirus Status:</span>
          <span className={`security-overview-value ${getStatusClass(data.antivirusStatus)}`}>
            {data.antivirusStatus}
          </span>
        </div>

        <div className="security-overview-item">
          <span className="security-overview-label">Firewall Status:</span>
          <span className={`security-overview-value ${getStatusClass(data.firewallStatus)}`}>
            {data.firewallStatus}
          </span>
        </div>
      </div>
    </div>
  );
}

export default SecurityOverview;