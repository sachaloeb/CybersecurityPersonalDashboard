// src/components/ThreatSimulation.js
import React, { useState, useEffect } from "react";
import axios from "axios";
import Papa from "papaparse";
import { saveAs } from "file-saver";
import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";
import "jspdf-autotable";
import "./ThreatSimulation.css";

const api = "https://localhost:7122/api/threatsimulation";

const ThreatSimulation = () => {
    /* ----------------------- form state ----------------------- */
    const [attackType, setAttackType] = useState("BRUTE_FORCE");
    const [targetIp, setTargetIp] = useState("");
    const [username, setUsername] = useState("");
    const [attemptCount, setAttemptCount] = useState(5);
    const [passwordText, setPasswordText] = useState("");
    const [logs, setLogs] = useState([]);
    const [loading, setLoading] = useState(false);

    /* ----------------------- helpers -------------------------- */
    const runSimulation = async (e) => {
        e.preventDefault();
        setLoading(true);

        try {
            if (attackType === "BRUTE_FORCE") {
                const body = {
                    targetIp,
                    username,
                    attemptCount,
                    passwords: passwordText
                        .split("\n")
                        .map((p) => p.trim())
                        .filter(Boolean),
                };
                await axios.post(`${api}/brute`, body);
            } else {
                await axios.post(`${api}/xss`, {
                    targetUrl: targetIp,
                    payload: passwordText,
                });
            }
            await fetchLogs(); // refresh view
        } catch (err) {
            alert("Simulation failed");
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const fetchLogs = async () => {
        const res = await axios.get(`${api}/logs`);
        setLogs(res.data);
    };

    /* ----------------------- export --------------------------- */
    const exportCsv = () => {
        const csv = Papa.unparse(
            logs.map(({ timestamp, target, attackType, result }) => ({
                timestamp,
                target,
                attackType,
                result,
            }))
        );
        const blob = new Blob([csv], { type: "text/csv;charset=utf-8;" });
        saveAs(blob, "threat_logs.csv");
    };

    const exportPdf = (tableSelector = ".log-table") => {
        const doc = new jsPDF({ orientation: "landscape", unit: "pt" });

        // grab the header & rows from the DOM
        const head = [
            [...document.querySelectorAll(`${tableSelector} thead th`)].map(
                (th) => th.innerText
            ),
        ];
        const body = [...document.querySelectorAll(`${tableSelector} tbody tr`)].map(
            (tr) => [...tr.children].map((td) => td.innerText)
        );

        autoTable(doc, { head, body, startY: 40 });
        doc.save("threat-logs.pdf");
    };

    useEffect(() => {
        fetchLogs();
    }, []);

    /* ----------------------- UI ------------------------------- */
    return (
        <div className="container">
            <h1 className="title">Threat Simulation</h1>

            {/* form */}
            <form onSubmit={runSimulation} className="form">
                <select
                    className="input"
                    value={attackType}
                    onChange={(e) => setAttackType(e.target.value)}
                >
                    <option value="BRUTE_FORCE">Brute Force</option>
                    <option value="XSS">XSS</option>
                </select>

                <input
                    className="input"
                    placeholder="Target IP / URL"
                    value={targetIp}
                    onChange={(e) => setTargetIp(e.target.value)}
                    required
                />

                {attackType === "BRUTE_FORCE" && (
                    <>
                        <input
                            className="input"
                            placeholder="Username"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            required
                        />

                        <input
                            type="number"
                            min="1"
                            className="input"
                            placeholder="Attempt count"
                            value={attemptCount}
                            onChange={(e) => setAttemptCount(e.target.value)}
                        />

                        <textarea
                            className="textarea"
                            rows={4}
                            placeholder="Optional word-list (one pwd per line)"
                            value={passwordText}
                            onChange={(e) => setPasswordText(e.target.value)}
                        />
                    </>
                )}

                {attackType === "XSS" && (
                    <textarea
                        className="textarea"
                        rows={3}
                        placeholder="<script>alert('xss')</script>"
                        value={passwordText}
                        onChange={(e) => setPasswordText(e.target.value)}
                    />
                )}

                <button className="button" type="submit" disabled={loading}>
                    {loading ? "Running…" : "Simulate"}
                </button>
            </form>

            {/* table */}
            <div style={{ marginTop: 30 }}>
                <h3>Logs ({logs.length})</h3>
                <button className="button" onClick={exportCsv}>
                    Export CSV
                </button>{" "}
                <button className="button" onClick={() => exportPdf()}>
                    Export PDF
                </button>
                <table className="log-table">
                    <thead>
                    <tr>
                        <th onClick={() => setLogs([...logs].reverse())}>Timestamp ⬍</th>
                        <th>Target</th>
                        <th>Type</th>
                        <th>Result</th>
                    </tr>
                    </thead>
                    <tbody>
                    {logs.map((l) => (
                        <tr key={l.id}>
                            <td>{new Date(l.timestamp).toLocaleString()}</td>
                            <td>{l.target}</td>
                            <td>{l.attackType}</td>
                            <td>{l.result ? "✔" : "✘"}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default ThreatSimulation;