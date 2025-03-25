import time
import platform
import psutil
import subprocess
from flask import Blueprint, jsonify, Flask

security_bp = Blueprint('security_bp', __name__)


def get_system_uptime():
    """Return system uptime in seconds."""
    boot_time = psutil.boot_time()
    return time.time() - boot_time


def get_firewall_status():
    """Return a string representing the firewall status.

    - On Linux, checks ufw status.
    - On macOS, checks the Application Firewall status.
    - On Windows, checks the status using netsh.
    Adjust for your environment or omit as needed.
    """
    current_os = platform.system().lower()
    try:
        if "linux" in current_os:
            # Check if ufw is installed and enabled
            result = subprocess.run(["sudo","ufw", "status"], capture_output=True, text=True)
            # Typical response might be "Status: active" or "Status: inactive"
            if "active" in result.stdout.lower():
                return "Enabled"
            else:
                return "Disabled"
        elif "darwin" in current_os:  # macOS
            # macOS firewall states:
            # 0 = Off, 1 = On for specific services, 2 = On for essential services
            result = subprocess.run(
                ["/usr/libexec/ApplicationFirewall/socketfilterfw", "--getglobalstate"],
                capture_output=True, text=True
            )
            print("Result:",result)
            state = result.stdout.strip()
            print("State:",state)
            if "Firewall is enabled" in state:
                return "Enabled"
            elif "Firewall is disabled" in state:
                return "Disabled"
            else:
                return "Unknown"
        elif "windows" in current_os:
            # Windows firewall status using netsh
            result = subprocess.run(
                ["netsh", "advfirewall", "show", "allprofiles"],
                capture_output=True, text=True
            )
            if "State ON" in result.stdout:
                return "Enabled"
            else:
                return "Disabled"
        else:
            return "Unknown or Unsupported OS"
    except FileNotFoundError:
        # e.g., ufw not installed or command not found
        return "Firewall command not found"
    except Exception:
        return "Error checking firewall status"


def get_antivirus_status():
    """Return antivirus status. This is highly OS- and product-specific.

    - On Windows, uses wmic to check for antivirus.
    - On macOS, checks for known antivirus processes.
    - On Linux, checks for known antivirus processes or services.
    """
    current_os = platform.system().lower()
    try:
        if "windows" in current_os:
            # Windows: Use wmic to check for antivirus
            result = subprocess.run(
                ["wmic", "/namespace:\\\\root\\SecurityCenter2", "path", "AntivirusProduct", "get", "displayName"],
                capture_output=True, text=True
            )
            if result.returncode == 0 and result.stdout.strip():
                return result.stdout.strip().split('\n')[1:]  # Skip the header line
            else:
                return "No antivirus found"
        elif "darwin" in current_os:  # macOS
            # macOS: Check for known antivirus processes
            known_antivirus = ["com.symantec.symdaemon", "com.mcafee.ssm.ScanManager"]
            for av in known_antivirus:
                result = subprocess.run(["pgrep", av], capture_output=True, text=True)
                if result.returncode == 0:
                    return f"Antivirus running: {av}"
            return "No antivirus found"
        elif "linux" in current_os:
            # Linux: Check for known antivirus processes or services
            known_antivirus = ["clamd", "freshclam", "sav-protect"]
            for av in known_antivirus:
                result = subprocess.run(["pgrep", av], capture_output=True, text=True)
                if result.returncode == 0:
                    return f"Antivirus running: {av}"
            return "No antivirus found"
        else:
            return "Unknown or Unsupported OS"
    except Exception as e:
        return f"Error checking antivirus status: {str(e)}"


@security_bp.route('/api/security_overview', methods=['GET'])
def security_overview():
    uptime = get_system_uptime()
    firewall = get_firewall_status()
    antivirus = get_antivirus_status()

    # Convert uptime to hours (or any format you like)
    uptime_hours = round(uptime / 3600, 2)

    return jsonify({
        "uptimeHours": uptime_hours,
        "antivirusStatus": antivirus,
        "firewallStatus": firewall
    })

if __name__ == '__main__':
    app = Flask(__name__)
    app.register_blueprint(security_bp)

    with app.app_context():
        print(security_overview().get_json())