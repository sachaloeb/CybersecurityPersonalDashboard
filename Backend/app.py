from flask import Flask, jsonify
import psutil
import socket
from flask_cors import CORS

app = Flask(__name__)
CORS(app)

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

@app.route('/api/connections', methods=['GET'])
def get_connections():
    return jsonify(get_connections_info())

@app.route('/', methods=['GET'])
def home():
    return "Welcome to the Cybersecurity Personal Dashboard API"

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=5001)