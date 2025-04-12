import React, { useState } from 'react';
import axios from 'axios';
import './ThreatSimulation.css';

const ThreatSimulation = () => {
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

        let combinedLogs = [];

        try {
            // 1) If user chose a file, call /simulate-from-file
            if (file) {
                const formData = new FormData();
                formData.append('ip', ip);
                formData.append('username', username);
                formData.append('file', file);

                const fileResponse = await axios.post(
                    'https://localhost:7122/api/sshattack/simulate-from-file',
                    formData,
                    {
                        headers: { 'Content-Type': 'multipart/form-data' },
                    }
                );

                combinedLogs = fileResponse.data;
            }

            // 2) If user typed in password lines, call /simulate
            const textPasswords = passwordText
                .split('\n')
                .map((p) => p.trim())
                .filter((p) => p.length > 0);

            if (textPasswords.length > 0) {
                const textResponse = await axios.post(
                    'https://localhost:7122/api/sshattack/simulate',
                    {
                        ip,
                        username,
                        passwords: textPasswords,
                    }
                );
                combinedLogs = [...combinedLogs, ...textResponse.data];
            }

            // 3) If neither file nor text, combinedLogs remains empty
            setResult(combinedLogs);
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

                <label htmlFor="fileInput">Or upload a .txt file:</label>
                <input
                    id="fileInput"
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

export default ThreatSimulation;