from flask import Flask, jsonify, request
import hashlib
from zxcvbn import zxcvbn
import psutil
import socket
from flask_cors import CORS
from security_overview.routes import security_bp

app = Flask(__name__)
app.register_blueprint(security_bp)
CORS(app)

@app.route('/api/check_password', methods=['POST'])
def check_password():
    data = request.json
    password = data.get('password', '')

    # 1) Hash the password (SHA-256 in this example)
    #    In a production environment, you would use a salt to
    #    prevent rainbow table attacks.
    hashed_password = hashlib.sha256(password.encode()).hexdigest()

    # 2) Assess strength with zxcvbn
    #    The zxcvbn function returns a dictionary with the 'score' (0-4)
    #    and a 'feedback' object containing warnings & suggestions.
    strength_info = zxcvbn(password)

    score = strength_info['score']
    feedback = strength_info['feedback']

    # 3) Create a human-readable assessment for demonstration
    #    (You can customize this however you prefer.)
    #    Typical zxcvbn meaning of score:
    #       0 -> Very Weak
    #       1 -> Weak
    #       2 -> Fair
    #       3 -> Strong
    #       4 -> Very Strong
    if score == 0:
        strength_label = "Very Weak"
    elif score == 1:
        strength_label = "Weak"
    elif score == 2:
        strength_label = "Fair"
    elif score == 3:
        strength_label = "Strong"
    else:
        strength_label = "Very Strong"

    # Combine zxcvbn feedback into a single string (warning + suggestions).
    feedback_text = feedback['warning'] if feedback['warning'] else ''
    if feedback['suggestions']:
        feedback_text += " " + " ".join(feedback['suggestions'])

    return jsonify({
        'hashed_password': hashed_password,
        'score': score,
        'strength_label': strength_label,
        'feedback': feedback_text.strip()
    })

@app.route('/api/connections', methods=['GET'])
def get_connections():
    return jsonify(get_connections_info())

@app.route('/', methods=['GET'])
def home():
    return "Welcome to the Cybersecurity Personal Dashboard API"

def get_connections_info():
    connections_info = []
    for conn in psutil.net_connections():
        laddr = conn.laddr.ip if conn.laddr else "N/A"
        lport = conn.laddr.port if conn.laddr else "N/A"
        raddr = conn.raddr.ip if conn.raddr else "N/A"
        rport = conn.raddr.port if conn.raddr else "N/A"

        if conn.type == socket.SOCK_STREAM:
            protocol = "TCP"
        elif conn.type == socket.SOCK_DGRAM:
            protocol = "UDP"
        else:
            protocol = "Other"

        connection_data = {
            "local_address": laddr,
            "local_port": lport,
            "remote_address": raddr,
            "remote_port": rport,
            "protocol": protocol,
            "status": conn.status
        }
        connections_info.append(connection_data)
    return connections_info

if __name__ == '__main__':
    app.run(debug=True,host='0.0.0.0',port=5050)