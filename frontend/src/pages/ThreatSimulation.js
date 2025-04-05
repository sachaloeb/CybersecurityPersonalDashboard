import React, { useState } from 'react';
import axios from 'axios';
import './ThreatSimulation.css';

const ThreatSimulationUpload = () => {
    const [ip, setIp] = useState('');
    const [username, setUsername] = useState('');
    const [passwordText, setPasswordText] = useState('');
    const [file, setFile] = useState(null);
    const [result, setResult] = useState(null);
    const [loading, setLoading] = useState(false);

    const handleFileChange = (e) => {
        setFile(e.target.files[0]);
    };

    const handleSimulate = async (e) => {
        e.preventDefault();
        setLoading(true);

        const formData = new FormData();
        formData.append('ip', ip);
        formData.append('username', username);
        if (file) formData.append('file', file);

        try {
            const response = await axios.post('https://localhost:7122/api/sshattack/simulate-from-file', formData, {
                headers: { 'Content-Type': 'multipart/form-data' },
            });

            const textPasswords = passwordText
                .split('\n')
                .map(p => p.trim())
                .filter(p => p.length > 0);

            if (textPasswords.length > 0) {
                const textResponse = await axios.post('https://localhost:7122/api/sshattack/simulate', {
                    ip,
                    username,
                    passwords: textPasswords,
                });

                setResult([...response.data, ...textResponse.data]);
            } else {
                setResult(response.data);
            }
        } catch (err) {
            alert('Simulation failed');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container">
            <h1 className="title">SSH Threat Detection Simulation</h1>

            <form onSubmit={handleSimulate} className="form">
                <input
                    className="input"
                    type="text"
                    placeholder="IP Address"
                    value={ip}
                    onChange={(e) => setIp(e.target.value)}
                    required
                />
                <input
                    className="input"
                    type="text"
                    placeholder="Username"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    required
                />
                <textarea
                    className="textarea"
                    rows={5}
                    placeholder="Enter passwords, one per line"
                    value={passwordText}
                    onChange={(e) => setPasswordText(e.target.value)}
                />
                <input
                    className="input"
                    type="file"
                    accept=".txt"
                    onChange={handleFileChange}
                />
                <button className="button" type="submit" disabled={loading}>
                    {loading ? 'Running...' : 'Simulate Attack'}
                </button>
            </form>

            {result && (
                <div style={{ marginTop: '2rem' }}>
                    <h3>Results</h3>
                    <table className="log-table">
                        <thead>
                        <tr>
                            <th>IP</th>
                            <th>Username</th>
                            <th>Password</th>
                            <th>Success</th>
                            <th>Timestamp</th>
                        </tr>
                        </thead>
                        <tbody>
                        {result.map((log, idx) => (
                            <tr key={idx}>
                                <td>{log.ipAddress}</td>
                                <td>{log.username}</td>
                                <td>{log.password}</td>
                                <td>{log.success ? '✅' : '❌'}</td>
                                <td>{new Date(log.timestamp).toLocaleString()}</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
};

export default ThreatSimulationUpload;