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

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error fetching security overview: {error.message}</div>;
  }

  if (!overview || !overview.value) {
    return <div>No data available</div>;
  }

  return (
      <div>
        <h1>Security Overview</h1>
        <p>Uptime Hours: {overview.value.uptimeHours}</p>
        <p>Firewall Status: {overview.value.firewallStatus}</p>
        <p>Antivirus Status: {overview.value.antivirusStatus}</p>
        <p>Logged In Users: {overview.value.loggedInUsers ? overview.value.loggedInUsers.join(', ') : 'No users logged in'}</p>
      </div>
  );
};

export default SecurityOverview;