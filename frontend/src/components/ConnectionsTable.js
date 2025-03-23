import React, { useEffect, useState } from 'react';
import axios from 'axios';
import './ConnectionsTable.css';

function ConnectionsTable() {
  const [connections, setConnections] = useState([]);

  useEffect(() => {
    const fetchConnections = async () => {
      try {
        console.log('Fetching connections...');
        const response = await axios.get('http://0.0.0.0:5001/api/connections');
        setConnections(response.data);
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

  return (
    <div className="connections-table-wrapper">
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
              <td>{conn.local_address}</td>
              <td>{conn.local_port}</td>
              <td>{conn.remote_address}</td>
              <td>{conn.remote_port}</td>
              <td>{conn.protocol}</td>
              <td>{conn.status}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default ConnectionsTable;