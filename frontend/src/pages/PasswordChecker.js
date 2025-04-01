import React, { useState } from 'react';
import './PasswordChecker.css'; // Our custom CSS

function PasswordChecker() {
  const [password, setPassword] = useState('');
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);

  const checkPassword = async () => {
    setError(null);
    setResult(null);

    try {
      const response = await fetch('https://localhost:7122/api/evaluate', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ password })
      });

      if (!response.ok) {
        throw new Error('Failed to check password');
      }

      const data = await response.json();
      setResult(data);
    } catch (err) {
      setError(err.message);
    }
  };

  // A helper to map score (0-4) to a percentage for the progress bar
  const getProgressPercentage = (score) => {
    return ((score + 1) / 5) * 100; 
  };

  // A helper to map score (0-4) to a color. Customize to your preference!
  const getProgressColor = (score) => {
    switch(score) {
      case 0: return '#d9534f'; // red
      case 1: return '#f0ad4e'; // orange
      case 2: return '#ffd700'; // gold
      case 3: return '#5bc0de'; // light-blue
      case 4: return '#5cb85c'; // green
      default: return '#ddd';
    }
  };

  return (
    <div className="password-checker-container">
      <div className="password-card">
        <h2 className="title">Password Strength Checker</h2>

        <label className="label" htmlFor="passwordInput">
          Enter your password:
        </label>
        <input
          id="passwordInput"
          type="password"
          className="input"
          placeholder="Type password here"
          value={password}
          onChange={e => setPassword(e.target.value)}
        />

        <button className="btn" onClick={checkPassword}>
          Check Strength
        </button>

        {error && (
          <div className="error-msg">
            <strong>Error:</strong> {error}
          </div>
        )}

        {result && (
          <div className="results">
            <div className="progress-bar-background">
              <div
                className="progress-bar-fill"
                style={{
                  width: `${getProgressPercentage(result.score)}%`,
                  backgroundColor: getProgressColor(result.score),
                }}
              />
            </div>

            <p className="hashed">
              <strong>Hashed Password:</strong> {result.hashed_password}
            </p>

            <p>
              <strong>Strength Score:</strong> {result.score} (0 = Weakest, 4 = Strongest)
            </p>

            <p>
              <strong>Strength Label:</strong> {result.strength_label}
            </p>

            <p>
              <strong>Feedback:</strong> {result.feedback}
            </p>
          </div>
        )}
      </div>
    </div>
  );
}

export default PasswordChecker;