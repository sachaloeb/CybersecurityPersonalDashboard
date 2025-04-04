import React, { useState } from 'react';

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
        <div className="p-6 max-w-xl mx-auto bg-white rounded-2xl shadow-2xl mt-10;">
            <h2 className="text-2xl font-bold mb-4 text-gray-800">🔍 Port Scanner</h2>

            <div className="space-y-3">
                <input
                    className="border border-gray-300 p-3 rounded w-full focus:ring-2 focus:ring-blue-500"
                    type="text"
                    placeholder="Target IP (e.g. 127.0.0.1)"
                    value={ip}
                    onChange={(e) => setIp(e.target.value)}
                />
                <div className="flex gap-3">
                    <input
                        className="border border-gray-300 p-3 rounded w-full focus:ring-2 focus:ring-blue-500"
                        type="number"
                        placeholder="Start Port"
                        value={startPort}
                        onChange={(e) => setStartPort(e.target.value)}
                    />
                    <input
                        className="border border-gray-300 p-3 rounded w-full focus:ring-2 focus:ring-blue-500"
                        type="number"
                        placeholder="End Port"
                        value={endPort}
                        onChange={(e) => setEndPort(e.target.value)}
                    />
                </div>

                <button
                    className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-4 rounded shadow-md transition duration-200"
                    onClick={handleScan}
                    disabled={loading}
                >
                    {loading ? 'Scanning...' : '🚀 Scan Ports'}
                </button>
            </div>

            {error && (
                <div className="mt-4 p-3 bg-red-100 text-red-800 rounded shadow">
                    ⚠️ {error}
                </div>
            )}

            <div className="mt-6">
                <h3 className="text-lg font-semibold text-gray-700 mb-2">🧭 Open Ports:</h3>
                {loading && <p className="text-gray-500 italic">Scanning, please wait...</p>}

                {!loading && results.length > 0 ? (
                    <ul className="list-disc pl-6 text-green-700 space-y-1 transition-all">
                        {results.map((port) => (
                            <li key={port}>Port {port} is open</li>
                        ))}
                    </ul>
                ) : (
                    !loading && (
                        <p className="text-gray-500 italic">No open ports found or scan not yet run.</p>
                    )
                )}
            </div>
        </div>
    );
};

export default PortScannerForm;