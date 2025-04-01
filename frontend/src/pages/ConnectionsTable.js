import React, { useEffect, useState } from 'react';
import axios from 'axios';
import './ConnectionsTable.css';

function ConnectionsTable() {
  const [connections, setConnections] = useState([]);

  useEffect(() => {
    const fetchConnections = async () => {
      try {
        console.log('Fetching connections...');
        const response = await axios.get('https://localhost:7122/api/status');
        console.log('API response:', response.data);
        setConnections(response.data.value); // Extract the value property
      } catch (error) {
        console.error('Error fetching connections:', error);
      }
    };

    // Fetch connections initially
    fetchConnections();

    // Set up interval to fetch connections every 5 seconds
    const intervalId = setInterval(fetchConnections, 5000);

    // Clean up interval on component unmount
    return () => clearInterval(intervalId);
  }, []);

  console.log('Connections state:', connections);

  if (!Array.isArray(connections)) {
    return <div>No connections available</div>;
  }

  return (
      <div className="connections-table-wrapper">
        <h2>Connections Table</h2>
        <table className="connections-table">
          <thead>
          <tr>
            <th>Local Address</th>
            <th>Local Port</th>
            <th>Remote Address</th>
            <th>Remote Port</th>
            <th>Protocol</th>
            <th>Status</th>
          </tr>
          </thead>
          <tbody>
          {connections.map((conn, index) => (
              <tr key={index}>
                <td>{conn.local}</td>
                <td>{conn.localPort}</td>
                <td>{conn.remote}</td>
                <td>{conn.remotePort}</td>
                <td>{conn.protocol}</td>
                <td>{conn.state}</td>
              </tr>
          ))}
          </tbody>
        </table>
      </div>
  );
}

export default ConnectionsTable;