import React, { useState } from 'react';
import './ZapAlertTable.css';

const ZapAlertTable = () => {
    const [alerts, setAlerts] = useState([]);
    const [url, setUrl] = useState('');
    const [loading, setLoading] = useState(false);
    const [expandedIndex, setExpandedIndex] = useState(null);

    const getRiskClass = (risk) => {
        switch (risk.toLowerCase()) {
            case 'high':
                return 'risk-high';
            case 'medium':
                return 'risk-medium';
            case 'low':
                return 'risk-low';
            case 'informational':
                return 'risk-informational';
            default:
                return 'risk-unknown';
        }
    };

    const handleScan = async () => {
        if (!url) return;

        setLoading(true);
        setAlerts([]);

        try {
            const response = await fetch('https://localhost:7122/api/vulnerability/scan', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(url)
            });

            const result = await response.json();
            setAlerts(result || []);
        } catch (err) {
            console.error('Error triggering ZAP scan:', err);
        } finally {
            setLoading(false);
        }
    };

    const toggleExpand = (index) => {
        setExpandedIndex(index === expandedIndex ? null : index);
    };

    return (
        <div className="zap-container">
            <h2 className="zap-title">Vulnerability Scanner</h2>

            <div className="zap-input-group">
                <input
                    type="text"
                    value={url}
                    onChange={(e) => setUrl(e.target.value)}
                    placeholder="Enter a URL to scan (e.g. http://example.com)"
                    className="zap-url-input"
                />
                <button onClick={handleScan} disabled={loading || !url} className="zap-scan-button">
                    {loading ? 'Scanning...' : 'Scan'}
                </button>
            </div>

            {alerts.length > 0 && <h3 className="zap-results-title">Scan Results</h3>}

            {alerts.length === 0 && !loading && <p className="zap-empty">No alerts to display.</p>}

            {alerts.map((alert, index) => (
                <div key={index} className="zap-alert-card">
                    <div className="zap-alert-header" onClick={() => toggleExpand(index)}>
                        <span className="zap-alert-title">{alert.alert}</span>
                        <span className={`zap-risk ${getRiskClass(alert.risk)}`}>{alert.risk}</span>
                    </div>

                    {expandedIndex === index && (
                        <div className="zap-alert-details">
                            <p><strong>URL:</strong> {alert.url}</p>
                            <p><strong>Description:</strong> {alert.description}</p>
                            <p><strong>Solution:</strong> {alert.solution}</p>
                            {alert.reference && (
                                <p>
                                    <strong>Reference:</strong>{' '}
                                    <a href={alert.reference.split('\n')[0]} target="_blank" rel="noopener noreferrer">
                                        {alert.reference.split('\n')[0]}
                                    </a>
                                </p>
                            )}
                            {alert.tags && (
                                <div className="zap-tags">
                                    <strong>Tags:</strong>
                                    <ul>
                                        {Object.entries(alert.tags).map(([key, value]) => (
                                            <li key={key}>
                                                <a href={value} target="_blank" rel="noopener noreferrer">{key}</a>
                                            </li>
                                        ))}
                                    </ul>
                                </div>
                            )}
                        </div>
                    )}
                </div>
            ))}
        </div>
    );
};

export default ZapAlertTable;