import React, { useEffect, useState } from 'react';
import './ZapAlertTable.css';

const ZapAlertTable = () => {
    const [alerts, setAlerts] = useState([]);

    useEffect(() => {
        fetch('https://localhost:7122/api/vulnerability/scan')
            .then(res => res.json())
            .then(data => setAlerts(data))
            .catch(err => console.error('Error fetching ZAP alerts:', err));
    }, []);

    const riskColor = (risk) => {
        switch (risk.toLowerCase()) {
            case 'high':
                return 'text-red-600 font-bold';
            case 'medium':
                return 'text-yellow-600 font-semibold';
            case 'low':
                return 'text-green-600';
            default:
                return 'text-gray-600';
        }
    };

    return (
        <div className="zap-container">
            <h2 className="zap-title">Vulnerability Scan Results</h2>
            <table className="zap-table">
                <thead>
                <tr>
                    <th>Alert</th>
                    <th>Risk</th>
                    <th>URL</th>
                </tr>
                </thead>
                <tbody>
                {alerts.map((alert, index) => (
                    <tr key={index}>
                        <td>{alert.alert}</td>
                        <td className={riskColor(alert.risk)}>{alert.risk}</td>
                        <td>{alert.url}</td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
};

export default ZapAlertTable;