import React, { useState } from 'react';
import './PortScannerForm.css';

const PortScannerForm = () => {
    const [ip, setIp] = useState('');
    const [startPort, setStartPort] = useState(20);
    const [endPort, setEndPort] = useState(1024);
    const [results, setResults] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const handleScan = async () => {
        setLoading(true);
        setError(null);
        setResults([]);
        try {
            const response = await fetch('https://localhost:7122/api/portscanner/scan', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ ip, startPort, endPort })
            });

            if (!response.ok) {
                throw new Error('Failed to scan ports');
            }

            const data = await response.json();
            setResults(data.openPorts);
        } catch (err) {
            setError(err.message);
        }
        setLoading(false);
    };

    return (
        <div className="portscanner-container">
            <h2 className="title">Port Scanner</h2>

            <input
                className="input"
                type="text"
                placeholder="Target IP"
                value={ip}
                onChange={(e) => setIp(e.target.value)}
            />
            <div className="input-group">
                <input
                    className="input"
                    type="number"
                    placeholder="Start Port"
                    value={startPort}
                    onChange={(e) => setStartPort(e.target.value)}
                />
                <input
                    className="input"
                    type="number"
                    placeholder="End Port"
                    value={endPort}
                    onChange={(e) => setEndPort(e.target.value)}
                />
            </div>
            <button
                className="scan-button"
                onClick={handleScan}
                disabled={loading}
            >
                {loading ? 'Scanning...' : 'Scan Ports'}
            </button>

            {error && <div className="error">{error}</div>}

            <div className="results">
                <h3>Open Ports:</h3>
                {loading && <p className="status">Scanning...</p>}
                {!loading && results.length > 0 ? (
                    <ul>
                        {results.map((port) => (
                            <li key={port}>Port {port} is open</li>
                        ))}
                    </ul>
                ) : (
                    !loading && <p className="status">No open ports found or scan not yet run.</p>
                )}
            </div>
        </div>
    );
};

export default PortScannerForm;