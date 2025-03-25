# Personal Cybersecurity Dashboard

**Personal Cybersecurity Dashboard** is a web-based application designed to help users monitor, manage, and secure their personal devices and networks. This project consolidates key cybersecurity functionalities—such as network traffic monitoring, vulnerability scanning, password strength checks, and incident logging—into a single, user-friendly dashboard. It is developed in a modular fashion with a test-driven approach, ensuring reliability and maintainability.

---

## Table of Contents
1. [Features](#features)
2. [Tech Stack](#tech-stack)
3. [Project Structure](#project-structure)
4. [Installation](#installation)
5. [Usage](#usage)
6. [Contributing](#contributing)

---

## Features

1. **Network Traffic Monitoring**  
   Displays active network connections (IP, ports, connection status) and flags potential anomalies.

2. **Vulnerability Scanner**  
   Scans local or remote targets for open ports, outdated software, or weak configurations.

3. **Password Strength Checker**  
   Evaluates password strength using libraries like `zxcvbn` without storing actual passwords.

4. **Threat Intelligence Integration**  
   Fetches live data on known threats/vulnerabilities from public APIs (e.g., VirusTotal).

5. **Incident Log**  
   Records detected threats and incidents, allowing users to attach evidence (logs, screenshots) and mark resolutions.

---

## Tech Stack

- **Frontend**: [React.js](https://reactjs.org/)  
- **Backend**: Python ([Flask](https://flask.palletsprojects.com/))
- **Database**: [PostgreSQL](https://www.postgresql.org/)  
- **Deployment**: Docker (with optional deployment on Heroku, AWS, or personal servers)

> **Note**: The application is being developed and tested on macOS. Other platforms are supported but may require different configurations.

---

## Project Structure

```bash
PersonalCybersecurityDashboard/
├── backend/
│ ├── app.py # Main Flask/Django entry point
│ ├── requirements.txt # Python dependencies
│ ├── tests/ # TDD: Backend tests
│ └── ... # Additional modules (e.g., vulnerability_scanner, log_parser)
├── frontend/
│ ├── package.json # Node.js dependencies
│ ├── src/
│ │ ├── App.js # Main React component
│ │ ├── pages/ # Shared pages (DashboardLayout, etc.)
│ │ ├── pages/ # Feature pages (NetworkMonitor, PasswordChecker, etc.)
│ │ └── tests/ # TDD: Frontend tests
│ └── ...
├── README.md # Project documentation
```

## Installation
1. Clone the Repository
   ```bash
   git clone https://github.com/sachaloeb/CybersecurityPersonalDashboard.git
   ```
2. Backend Setup
   - Install Python dependencies.
     ```bash
     pip install -r requirements.txt
     ```
3. Frontend Setup
```bash
cd frontend
npm install
```

## Usage

### Local Development

1. **Backend**  
   ```bash
   cd backend
   sudo gunicorn -w 4 -b 0.0.0.0:5050 app:app
   ```
   By default, the backend should start at http://0.0.0.0:5050.
2. **Frontend**
   ```bash
   cd frontend
   npm start
   ```
   The frontend should be running at http://localhost:3000.
### Access the Dashboard
Open your browser and navigate to http://localhost:3000. You should see the login screen or the main dashboard page (if no authentication is configured yet).
### Testing (TDD)
1. **Backend Tests:**
   ```bash
   cd backend
   pytest
   ```
2. **Frontend Tests:**
   ```bash
   cd frontend
   npm test
   ```

## Contributing

Contributions are welcome! Please open an issue to discuss what you would like to change or submit a pull request directly. Remember to:

1. Fork the repo and create your branch from main.
2. Add tests for any new or changed functionality.
3. Ensure that all tests pass before merging.

## Contact

- [GitHub](https://github.com/sachaloeb/)
- [LinkedIn](https://www.linkedin.com/in/sacha-loeb-5365682ba)

> **Disclaimer**: This dashboard is intended as a learning tool and personal project. It should not be used as a substitute for professional security services or comprehensive security solutions. Always test responsibly and with permission on systems you own or have explicit permission to analyze.
