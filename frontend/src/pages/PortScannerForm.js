import React, {useEffect, useState} from 'react';
import './PortScannerForm.css';

const PortScannerForm = () => {
    const [ip, setIp] = useState('');
    const [startPort, setStartPort] = useState(20);
    const [endPort, setEndPort] = useState(1024);
    const [results, setResults] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [rows, setRows] = useState([]);

    useEffect(() => {
        fetch("https://localhost:7122/api/portscanner/history?take=10")
            .then(r => r.json())
            .then(setRows)
            .catch(console.error);
    }, []);

    //const handleScan = async () => {
    //     setLoading(true);
    //     setError(null);
    //     setResults([]);
    //     try {
    //         const response = await fetch('https://localhost:7122/api/portscanner/scan', {
    //             method: 'POST',
    //             headers: { 'Content-Type': 'application/json' },
    //             body: JSON.stringify({ ip, startPort, endPort })
    //         });
    //
    //         if (!response.ok) {
    //             throw new Error('Failed to scan ports');
    //         }
    //
    //         const data = await response.json();
    //         setResults(data.openPorts);
    //     } catch (err) {
    //         setError(err.message);
    //     }
    //     setLoading(false);
    // };
    function badgeColor(status) {
        switch (status) {
            case "Complete": return "#4caf50";
            case "Error":    return "#e53935";
            default:         return "#ffb300";   // Pending / Unknown
        }
    }
    const handleDetailedScan = async () => {
        setLoading(true);
        setError(null);
        try {
            const response = await fetch('https://localhost:7122/api/portscanner/scan-detailed', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ ip, startPort, endPort })
            });

            if (!response.ok) {
                throw new Error('Failed to scan ports');
            }

            const data = await response.json();
            console.log(data);          // The full object from the server
            setResults(data);           // Save the entire result object
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
                onClick={handleDetailedScan}
                disabled={loading}
            >
                {loading ? 'Scanning...' : 'Scan Ports'}
            </button>

            {error && <div className="error">{error}</div>}

            <div className="results">
                <h3>Scan Results:</h3>
                {loading && <p className="status">Scanning...</p>}

                {!loading && results && results.ports && results.ports.length > 0 ? (
                    <>
                        <p>IP: {results.ip}</p>
                        <p>Host Up: {results.isHostUp ? 'Yes' : 'No'}</p>
                        <p>Scan Duration: {results.scanDuration}</p>
                        <ul>
                            {results?.ports
                                ?.filter((p) => p.state === "Open")
                                .map((p) => (
                                    <li key={p.port}>
                                        Port {p.port} is {p.state}
                                    </li>
                                ))}
                        </ul>
                        <p>
                            Status:&nbsp;
                            <span style={{
                                background: badgeColor(results.status),
                                color: "white", padding: "2px 8px",
                                borderRadius: "4px", fontSize: "0.8rem"
                            }}>
                                {results.status}
                            </span>
                        </p>
                    </>
                ) : (
                    !loading && <p className="status">No data or scan not yet run.</p>
                )}
            </div>

            <div style={{marginTop: 40}}>
                <h3>Recent Scans</h3>
                <table style={{width: "100%", borderCollapse: "collapse"}}>
                    <thead>
                    <tr>
                        <th>Host</th>
                        <th>Ports</th>
                        <th>Started</th>
                        <th>Duration</th>
                        <th>Status</th>
                    </tr>
                    </thead>
                    <tbody>
                    {rows.map(r => (
                        <tr key={r.startedAt}>
                            <td>{r.host}</td>
                            <td>{r.startPort}-{r.endPort}</td>
                            <td>{new Date(r.startedAt).toLocaleTimeString()}</td>
                            <td>{r.duration ?? "—"}</td>
                            <td style={{color: badgeColor(r.status)}}>{r.status}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default PortScannerForm;