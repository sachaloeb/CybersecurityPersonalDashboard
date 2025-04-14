import React, { useState, useEffect } from "react";
import axios from "axios";
import "./ThreatSimulation.css";

const ThreatSimulation = () => {
    // ------------------------
    // Form State
    // ------------------------
    const [attackType, setAttackType] = useState("BRUTE_FORCE");

    // Brute Force fields
    const [ip, setIp] = useState("");
    const [username, setUsername] = useState("");
    const [passwordText, setPasswordText] = useState("");
    const [file, setFile] = useState(null);

    // XSS fields
    const [targetUrl, setTargetUrl] = useState("");
    const [xssPayload, setXssPayload] = useState("");

    const [loading, setLoading] = useState(false);

    // ------------------------
    // Log Display + Filters
    // ------------------------
    const [logs, setLogs] = useState([]);
    const [startDate, setStartDate] = useState("");
    const [endDate, setEndDate] = useState("");
    const [filterAttackType, setFilterAttackType] = useState("");
    const [filterIp, setFilterIp] = useState("");

    // ------------------------
    // Attack Simulation
    // ------------------------
    const handleFileChange = (e) => {
        setFile(e.target.files[0]);
    };

    const simulateAttack = async (e) => {
        e.preventDefault();
        setLoading(true);

        try {
            if (attackType === "BRUTE_FORCE") {
                // 1) Possibly handle file-based simulation if user provided a file
                let combinedLogs = [];

                if (file) {
                    const formData = new FormData();
                    formData.append("ip", ip);
                    formData.append("username", username);
                    formData.append("file", file);

                    const fileResponse = await axios.post(
                        "https://localhost:7122/api/sshattack/simulate-from-file",
                        formData,
                        { headers: { "Content-Type": "multipart/form-data" } }
                    );
                    combinedLogs = fileResponse.data;
                }

                // 2) If user typed in password lines
                const textPasswords = passwordText
                    .split("\n")
                    .map((p) => p.trim())
                    .filter((p) => p.length > 0);

                if (textPasswords.length > 0) {
                    const textResponse = await axios.post(
                        "https://localhost:7122/api/sshattack/simulate",
                        {
                            ip,
                            username,
                            passwords: textPasswords,
                        }
                    );
                    combinedLogs = [...combinedLogs, ...textResponse.data];
                }

                // optional: show results or re-fetch from logs endpoint
                if (combinedLogs.length > 0) {
                    console.log("Brute Force results:", combinedLogs);
                }
            } else if (attackType === "XSS") {
                // Fake XSS attack
                const response = await axios.post("https://localhost:7122/api/threatsimulation/xss", {
                    targetUrl,
                    payload: xssPayload,
                });
                console.log("XSS result:", response.data);
            }

            // Finally, refresh the logs
            await fetchLogs();
        } catch (err) {
            alert("Simulation failed");
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    // ------------------------
    // Fetch Logs w/ Filters
    // ------------------------
    const fetchLogs = async () => {
        try {
            const params = {};
            if (startDate) params.startDate = startDate;
            if (endDate) params.endDate = endDate;
            if (filterAttackType) params.attackType = filterAttackType;
            if (filterIp) params.ip = filterIp;

            // This presumes you have an endpoint like /api/threatsimulation/logs
            const response = await axios.get("https://localhost:7122/api/threatsimulation/logs", { params });
            setLogs(response.data);
        } catch (err) {
            console.error("Failed to fetch logs:", err);
        }
    };

    useEffect(() => {
        fetchLogs();
        // eslint-disable-next-line
    }, []);

    // ------------------------
    // Render
    // ------------------------
    return (
        <div className="container">
            <h1 className="title">Threat Simulation</h1>

            {/* Attack Type Switch */}
            <div className="form">
                <label>Select Attack Type:</label>
                <select
                    className="input"
                    value={attackType}
                    onChange={(e) => setAttackType(e.target.value)}
                >
                    <option value="BRUTE_FORCE">Brute Force</option>
                    <option value="XSS">Fake XSS</option>
                </select>
            </div>

            <form onSubmit={simulateAttack} className="form">
                {attackType === "BRUTE_FORCE" && (
                    <>
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
                    </>
                )}

                {attackType === "XSS" && (
                    <>
                        <input
                            className="input"
                            type="text"
                            placeholder="Target URL"
                            value={targetUrl}
                            onChange={(e) => setTargetUrl(e.target.value)}
                            required
                        />
                        <textarea
                            className="textarea"
                            rows={3}
                            placeholder="<script>alert('xss');</script>"
                            value={xssPayload}
                            onChange={(e) => setXssPayload(e.target.value)}
                        />
                    </>
                )}

                <button className="button" type="submit" disabled={loading}>
                    {loading ? "Running..." : "Simulate Attack"}
                </button>
            </form>

            {/* Filter logs */}
            <div className="search-container">
                <h3>Filter Logs</h3>
                <div style={{ display: "flex", gap: "10px" }}>
                    <div>
                        <label>Start Date:</label>
                        <input
                            className="input"
                            type="date"
                            value={startDate}
                            onChange={(e) => setStartDate(e.target.value)}
                        />
                    </div>
                    <div>
                        <label>End Date:</label>
                        <input
                            className="input"
                            type="date"
                            value={endDate}
                            onChange={(e) => setEndDate(e.target.value)}
                        />
                    </div>
                    <div>
                        <label>Attack Type:</label>
                        <input
                            className="input"
                            type="text"
                            placeholder="BRUTE_FORCE or XSS"
                            value={filterAttackType}
                            onChange={(e) => setFilterAttackType(e.target.value)}
                        />
                    </div>
                    <div>
                        <label>IP:</label>
                        <input
                            className="input"
                            type="text"
                            placeholder="IP filter"
                            value={filterIp}
                            onChange={(e) => setFilterIp(e.target.value)}
                        />
                    </div>
                    <button className="button" onClick={fetchLogs}>
                        Apply Filter
                    </button>
                </div>
            </div>

            {/* Logs Table */}
            <div style={{ marginTop: "2rem" }}>
                <h3>Logs</h3>
                <table className="log-table">
                    <thead>
                    <tr>
                        <th>Timestamp</th>
                        <th>Target</th>
                        <th>Attack Type</th>
                        <th>Result</th>
                        <th>IP</th>
                    </tr>
                    </thead>
                    <tbody>
                    {logs.map((log) => (
                        <tr key={log.id}>
                            <td>{new Date(log.timestamp).toLocaleString()}</td>
                            <td>{log.target}</td>
                            <td>{log.attackType}</td>
                            <td>{log.result ? "✅" : "❌"}</td>
                            <td>{log.ip}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default ThreatSimulation;